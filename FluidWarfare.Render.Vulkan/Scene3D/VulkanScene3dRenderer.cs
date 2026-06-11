using System.Diagnostics;
using System.Runtime.InteropServices;
using FluidWarfare.Render.Vulkan.Camera;
using FluidWarfare.Render.Vulkan.Shaders;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D;

/// <summary>
/// 完整 3D 场景渲染器。
/// 加载 GLSL 着色器（SPIR-V），创建 Graphics Pipeline、Vertex Buffer，
/// 绘制 3D 地面网格和单位占位物，并提交 Present。
///
/// 全链路探测模式：
///   Instance → Surface → Device → Swapchain → ImageViews → RenderPass
///   → ShaderModules → PipelineLayout → GraphicsPipelines → Framebuffers
///   → VertexBuffers → CommandPool → CommandBuffer → Submit → Present → Cleanup
///
/// 本轮不创建 Depth Buffer、Texture、Material、Index Buffer。
/// </summary>
public static unsafe class VulkanScene3dRenderer
{
    // ─── 背景清屏颜色 ──────────────────────────────────────────────
    private const float ClearR = 0.03f;
    private const float ClearG = 0.08f;
    private const float ClearB = 0.18f;
    private const float ClearA = 1.0f;

    /// <summary>
    /// 执行一次完整的 3D 场景渲染。
    /// </summary>
    public static VulkanScene3dInfo RenderWindows(
        nint hinstance, nint hwnd, uint reqW, uint reqH,
        VulkanCameraInfo camera,
        ReadOnlySpan<VulkanScene3dVertex> gridVertices,
        ReadOnlySpan<VulkanScene3dVertex> unitVertices)
    {
        var sw = Stopwatch.StartNew();
        Vk? vk = null;
        Silk.NET.Vulkan.Instance inst = default;
        SurfaceKHR surf = default;
        Silk.NET.Vulkan.Device dev = default;
        SwapchainKHR swapchain = default;
        ImageView[] imageViews = [];
        RenderPass renderPass = default;
        Framebuffer[] framebuffers = [];
        CommandPool cmdPool = default;
        CommandBuffer cmdBuf = default;
        Silk.NET.Vulkan.Semaphore semAvail = default, semFin = default;
        Fence fence = default;
        ShaderModule vertModule = default, fragModule = default;
        PipelineLayout pipelineLayout = default;
        Pipeline gridPipeline = default, unitPipeline = default;
        Silk.NET.Vulkan.Buffer gridBuffer = default, unitBuffer = default;
        DeviceMemory gridMemory = default, unitMemory = default;

        bool instOk = false, surfOk = false, devOk = false, scOk = false;
        bool rpOk = false, poolOk = false, syncOk = false;
        bool vertModOk = false, fragModOk = false, layoutOk = false;
        bool gridPipeOk = false, unitPipeOk = false;
        bool gridBufOk = false, unitBufOk = false;

        nint fnDestroySurface = 0, fnDestroySwapchain = 0;
        var drawCalls = 0;

        try
        {
            vk = Vk.GetApi();
            if (hinstance == 0 || hwnd == 0)
                return Fail("句柄不可用。", sw);

            // 1. Instance
            if (!CreateInstance(vk, out inst)) return Fail("Instance 创建失败。", sw);
            instOk = true;
            fnDestroySurface = LoadProc(vk, inst, "vkDestroySurfaceKHR");

            // 2. Surface
            if (!CreateSurface(vk, inst, hinstance, hwnd, out surf)) return Fail("Surface 创建失败。", sw);
            surfOk = true;

            // 3. Physical Device
            if (!SelectDevice(vk, inst, surf, out var pd, out var qi, out _))
                return Fail("未找到 Graphics+Present 队列。", sw);

            // 4. Logical Device
            if (!CreateDevice(vk, pd, qi, out dev)) return Fail("Device 创建失败。", sw);
            devOk = true;

            // Instance-level function pointers
            var fnGetCaps = LoadProc(vk, inst, "vkGetPhysicalDeviceSurfaceCapabilitiesKHR");
            var fnGetFmts = LoadProc(vk, inst, "vkGetPhysicalDeviceSurfaceFormatsKHR");
            var fnGetModes = LoadProc(vk, inst, "vkGetPhysicalDeviceSurfacePresentModesKHR");
            if (fnGetCaps == 0 || fnGetFmts == 0 || fnGetModes == 0)
                return Fail("无法加载 Surface 查询函数。", sw);

            // Device-level function pointers
            fnDestroySwapchain = LoadDeviceProc(vk, dev, "vkDestroySwapchainKHR");
            var fnCreateSwapchain = LoadDeviceProc(vk, dev, "vkCreateSwapchainKHR");
            var fnGetImages = LoadDeviceProc(vk, dev, "vkGetSwapchainImagesKHR");
            var fnAcquire = LoadDeviceProc(vk, dev, "vkAcquireNextImageKHR");
            var fnQueuePresent = LoadDeviceProc(vk, dev, "vkQueuePresentKHR");
            if (fnCreateSwapchain == 0 || fnDestroySwapchain == 0 || fnGetImages == 0 ||
                fnAcquire == 0 || fnQueuePresent == 0)
                return Fail("无法加载 Swapchain 设备扩展函数。", sw);

            // 5. Surface Capabilities
            var caps = QueryCaps(pd, surf, fnGetCaps);
            var formats = QueryFormats(pd, surf, fnGetFmts);
            if (formats.Length == 0) return Fail("无可用 Surface 格式。", sw);
            var chosenFmt = ChooseFormat(formats).Format;
            var chosenMode = ChoosePresentMode(QueryModes(pd, surf, fnGetModes));

            var imageCount = Math.Clamp(caps.MinImageCount + 1, caps.MinImageCount,
                caps.MaxImageCount > 0 ? caps.MaxImageCount : uint.MaxValue);
            var extent = ChooseExtent(caps, reqW, reqH);

            // 6. Create Swapchain
            var scCI = new SwapchainCreateInfoKHR
            {
                SType = StructureType.SwapchainCreateInfoKhr, Surface = surf,
                MinImageCount = imageCount,
                ImageFormat = chosenFmt,
                ImageColorSpace = ChooseFormat(formats).ColorSpace,
                ImageExtent = extent, ImageArrayLayers = 1,
                ImageUsage = ImageUsageFlags.ColorAttachmentBit,
                ImageSharingMode = SharingMode.Exclusive,
                PreTransform = caps.CurrentTransform,
                CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr,
                PresentMode = chosenMode, Clipped = Vk.True
            };

            var createScFn = Marshal.GetDelegateForFunctionPointer<CreateSwapchainPtr>(fnCreateSwapchain);
            SwapchainKHR sc;
            if (createScFn(dev, &scCI, null, &sc) != Result.Success) return Fail("Swapchain 创建失败。", sw);
            swapchain = sc; scOk = true;

            var getImgsFn = Marshal.GetDelegateForFunctionPointer<GetSwapchainImagesPtr>(fnGetImages);
            uint imgCount = 0;
            getImgsFn(dev, swapchain, &imgCount, null);
            if (imgCount == 0) return Fail("Swapchain 图像数为 0。", sw);
            var images = new Image[imgCount];
            fixed (Image* ip = images) getImgsFn(dev, swapchain, &imgCount, ip);

            // 7. ImageViews
            imageViews = new ImageView[imgCount];
            for (var i = 0; i < imgCount; i++)
            {
                var ivCI = new ImageViewCreateInfo
                {
                    SType = StructureType.ImageViewCreateInfo, Image = images[i],
                    ViewType = ImageViewType.Type2D, Format = chosenFmt,
                    Components = new ComponentMapping { R = ComponentSwizzle.Identity, G = ComponentSwizzle.Identity, B = ComponentSwizzle.Identity, A = ComponentSwizzle.Identity },
                    SubresourceRange = new ImageSubresourceRange { AspectMask = ImageAspectFlags.ColorBit, BaseMipLevel = 0, LevelCount = 1, BaseArrayLayer = 0, LayerCount = 1 }
                };
                if (vk.CreateImageView(dev, &ivCI, null, out imageViews[i]) != Result.Success)
                    return Fail($"ImageView {i} 创建失败。", sw);
            }

            // 8. RenderPass
            var colorAtt = new AttachmentDescription
            {
                Format = chosenFmt, Samples = SampleCountFlags.Count1Bit,
                LoadOp = AttachmentLoadOp.Clear, StoreOp = AttachmentStoreOp.Store,
                StencilLoadOp = AttachmentLoadOp.DontCare, StencilStoreOp = AttachmentStoreOp.DontCare,
                InitialLayout = ImageLayout.Undefined, FinalLayout = ImageLayout.PresentSrcKhr
            };
            var colorRef = new AttachmentReference { Attachment = 0, Layout = ImageLayout.ColorAttachmentOptimal };
            var subpass = new SubpassDescription { PipelineBindPoint = PipelineBindPoint.Graphics, ColorAttachmentCount = 1, PColorAttachments = &colorRef };
            var rpCI = new RenderPassCreateInfo { SType = StructureType.RenderPassCreateInfo, AttachmentCount = 1, PAttachments = &colorAtt, SubpassCount = 1, PSubpasses = &subpass };
            if (vk.CreateRenderPass(dev, &rpCI, null, out renderPass) != Result.Success)
                return Fail("RenderPass 创建失败。", sw);
            rpOk = true;

            // 9. Shader Modules
            vertModule = CreateShaderModule(vk, dev, CompiledShaders.Basic3dVert);
            if (vertModule.Handle == 0) return Fail("Vertex Shader Module 创建失败。", sw);
            vertModOk = true;

            fragModule = CreateShaderModule(vk, dev, CompiledShaders.Basic3dFrag);
            if (fragModule.Handle == 0) return Fail("Fragment Shader Module 创建失败。", sw);
            fragModOk = true;

            // 10. Pipeline Layout (with push constant)
            var pcRange = new PushConstantRange
            {
                StageFlags = ShaderStageFlags.VertexBit,
                Offset = 0,
                Size = 64 // mat4 = 16 floats
            };
            var pCI = new PipelineLayoutCreateInfo
            {
                SType = StructureType.PipelineLayoutCreateInfo,
                PushConstantRangeCount = 1,
                PPushConstantRanges = &pcRange,
                SetLayoutCount = 0,
                PSetLayouts = null
            };
            if (vk.CreatePipelineLayout(dev, &pCI, null, out pipelineLayout) != Result.Success)
                return Fail("PipelineLayout 创建失败。", sw);
            layoutOk = true;

            // 11. Graphics Pipelines
            var vertexBinding = new VertexInputBindingDescription
            {
                Binding = 0,
                Stride = 28, // 7 floats × 4 bytes
                InputRate = VertexInputRate.Vertex
            };
            var vertexAttribs = new[]
            {
                new VertexInputAttributeDescription { Location = 0, Binding = 0, Format = Format.R32G32B32Sfloat, Offset = 0 },
                new VertexInputAttributeDescription { Location = 1, Binding = 0, Format = Format.R32G32B32A32Sfloat, Offset = 12 }
            };

            // Grid pipeline (LineList)
            fixed (VertexInputAttributeDescription* pAttr = vertexAttribs)
            {
                var viCI = new PipelineVertexInputStateCreateInfo
                {
                    SType = StructureType.PipelineVertexInputStateCreateInfo,
                    VertexBindingDescriptionCount = 1,
                    PVertexBindingDescriptions = &vertexBinding,
                    VertexAttributeDescriptionCount = 2,
                    PVertexAttributeDescriptions = pAttr
                };

                var iaCI = new PipelineInputAssemblyStateCreateInfo
                {
                    SType = StructureType.PipelineInputAssemblyStateCreateInfo,
                    Topology = PrimitiveTopology.LineList,
                    PrimitiveRestartEnable = Vk.False
                };

                // Viewport
                var viewport = new Viewport { X = 0, Y = 0, Width = extent.Width, Height = extent.Height, MinDepth = 0, MaxDepth = 1 };
                var scissor = new Rect2D(new Offset2D(0, 0), extent);
                var vsCI = new PipelineViewportStateCreateInfo
                {
                    SType = StructureType.PipelineViewportStateCreateInfo,
                    ViewportCount = 1, PViewports = &viewport,
                    ScissorCount = 1, PScissors = &scissor
                };

                var rsCI = new PipelineRasterizationStateCreateInfo
                {
                    SType = StructureType.PipelineRasterizationStateCreateInfo,
                    DepthClampEnable = Vk.False, RasterizerDiscardEnable = Vk.False,
                    PolygonMode = PolygonMode.Fill, CullMode = CullModeFlags.None,
                    FrontFace = FrontFace.Clockwise,
                    DepthBiasEnable = Vk.False, LineWidth = 1.0f
                };

                var msCI = new PipelineMultisampleStateCreateInfo
                {
                    SType = StructureType.PipelineMultisampleStateCreateInfo,
                    SampleShadingEnable = Vk.False, RasterizationSamples = SampleCountFlags.Count1Bit
                };

                var blendAtt = new PipelineColorBlendAttachmentState
                {
                    BlendEnable = Vk.False,
                    ColorWriteMask = ColorComponentFlags.RBit | ColorComponentFlags.GBit | ColorComponentFlags.BBit | ColorComponentFlags.ABit
                };
                var cbCI = new PipelineColorBlendStateCreateInfo
                {
                    SType = StructureType.PipelineColorBlendStateCreateInfo,
                    AttachmentCount = 1, PAttachments = &blendAtt
                };

                // Create grid pipeline
                var stages = stackalloc PipelineShaderStageCreateInfo[2];
                stages[0] = new PipelineShaderStageCreateInfo { SType = StructureType.PipelineShaderStageCreateInfo, Stage = ShaderStageFlags.VertexBit, Module = vertModule, PName = (byte*)Marshal.StringToHGlobalAnsi("main") };
                stages[1] = new PipelineShaderStageCreateInfo { SType = StructureType.PipelineShaderStageCreateInfo, Stage = ShaderStageFlags.FragmentBit, Module = fragModule, PName = (byte*)Marshal.StringToHGlobalAnsi("main") };

                var gpCI = new GraphicsPipelineCreateInfo
                {
                    SType = StructureType.GraphicsPipelineCreateInfo,
                    StageCount = 2, PStages = stages,
                    PVertexInputState = &viCI, PInputAssemblyState = &iaCI,
                    PViewportState = &vsCI, PRasterizationState = &rsCI,
                    PMultisampleState = &msCI, PColorBlendState = &cbCI,
                    Layout = pipelineLayout, RenderPass = renderPass, Subpass = 0
                };

                if (vk.CreateGraphicsPipelines(dev, default, 1, &gpCI, null, out gridPipeline) != Result.Success)
                    return Fail("Grid Pipeline 创建失败。", sw);
                gridPipeOk = true;

                // Unit pipeline (TriangleList)
                var iaCI2 = new PipelineInputAssemblyStateCreateInfo
                {
                    SType = StructureType.PipelineInputAssemblyStateCreateInfo,
                    Topology = PrimitiveTopology.TriangleList,
                    PrimitiveRestartEnable = Vk.False
                };

                var gpCI2 = new GraphicsPipelineCreateInfo
                {
                    SType = StructureType.GraphicsPipelineCreateInfo,
                    StageCount = 2, PStages = stages,
                    PVertexInputState = &viCI, PInputAssemblyState = &iaCI2,
                    PViewportState = &vsCI, PRasterizationState = &rsCI,
                    PMultisampleState = &msCI, PColorBlendState = &cbCI,
                    Layout = pipelineLayout, RenderPass = renderPass, Subpass = 0
                };

                if (vk.CreateGraphicsPipelines(dev, default, 1, &gpCI2, null, out unitPipeline) != Result.Success)
                    return Fail("Unit Pipeline 创建失败。", sw);
                unitPipeOk = true;

                Marshal.FreeHGlobal((nint)stages[0].PName);
                Marshal.FreeHGlobal((nint)stages[1].PName);
            }

            // 12. Vertex Buffers
            var gridData = VulkanScene3dVertices.ToInterleaved(gridVertices);
            var unitData = VulkanScene3dVertices.ToInterleaved(unitVertices);

            if (!CreateBuffer(vk, pd, dev, gridData, out gridBuffer, out gridMemory))
                return Fail("Grid Vertex Buffer 创建失败。", sw);
            gridBufOk = true;

            if (!CreateBuffer(vk, pd, dev, unitData, out unitBuffer, out unitMemory))
                return Fail("Unit Vertex Buffer 创建失败。", sw);
            unitBufOk = true;

            // 13. Framebuffers
            framebuffers = new Framebuffer[imgCount];
            for (var i = 0; i < imgCount; i++)
            {
                var att = stackalloc[] { imageViews[i] };
                var fbCI = new FramebufferCreateInfo { SType = StructureType.FramebufferCreateInfo, RenderPass = renderPass, AttachmentCount = 1, PAttachments = att, Width = extent.Width, Height = extent.Height, Layers = 1 };
                if (vk.CreateFramebuffer(dev, &fbCI, null, out framebuffers[i]) != Result.Success)
                    return Fail($"Framebuffer {i} 创建失败。", sw);
            }

            // 14. CommandPool
            var poolCI = new CommandPoolCreateInfo { SType = StructureType.CommandPoolCreateInfo, QueueFamilyIndex = qi };
            if (vk.CreateCommandPool(dev, &poolCI, null, out cmdPool) != Result.Success)
                return Fail("CommandPool 创建失败。", sw);
            poolOk = true;

            // 15. CommandBuffer
            var allocCI = new CommandBufferAllocateInfo { SType = StructureType.CommandBufferAllocateInfo, CommandPool = cmdPool, Level = CommandBufferLevel.Primary, CommandBufferCount = 1 };
            if (vk.AllocateCommandBuffers(dev, &allocCI, out cmdBuf) != Result.Success)
                return Fail("CommandBuffer 创建失败。", sw);

            // 16. Sync Objects
            var semCI = new SemaphoreCreateInfo { SType = StructureType.SemaphoreCreateInfo };
            var fenceCI = new FenceCreateInfo { SType = StructureType.FenceCreateInfo, Flags = FenceCreateFlags.SignaledBit };
            if (vk.CreateSemaphore(dev, &semCI, null, out semAvail) != Result.Success ||
                vk.CreateSemaphore(dev, &semCI, null, out semFin) != Result.Success ||
                vk.CreateFence(dev, &fenceCI, null, out fence) != Result.Success)
                return Fail("同步对象创建失败。", sw);
            syncOk = true;

            // 17. Acquire Image
            vk.WaitForFences(dev, 1, ref fence, Vk.True, ulong.MaxValue);
            vk.ResetFences(dev, 1, ref fence);

            uint imgIndex = 0;
            var acquireFn = Marshal.GetDelegateForFunctionPointer<AcquireNextImagePtr>(fnAcquire);
            var acqRes = acquireFn(dev, swapchain, ulong.MaxValue, semAvail, default, &imgIndex);
            if (acqRes == Result.ErrorOutOfDateKhr) return Fail("Acquire 返回 OutOfDate。", sw);
            if (acqRes != Result.Success && acqRes != Result.SuboptimalKhr)
                return Fail($"AcquireNextImage 失败：{acqRes}。", sw);

            // 18. MVP Matrix
            var aspect = extent.Width / (float)extent.Height;
            var mvp = VulkanCameraMatrices.ComputeVulkanMVP(camera, aspect);

            // 19. Record Command Buffer
            vk.ResetCommandBuffer(cmdBuf, CommandBufferResetFlags.None);
            var beginInfo = new CommandBufferBeginInfo { SType = StructureType.CommandBufferBeginInfo, Flags = CommandBufferUsageFlags.OneTimeSubmitBit };
            vk.BeginCommandBuffer(cmdBuf, &beginInfo);

            var clearVal = new ClearValue { Color = new ClearColorValue { Float32_0 = ClearR, Float32_1 = ClearG, Float32_2 = ClearB, Float32_3 = ClearA } };
            var rpBegin = new RenderPassBeginInfo
            {
                SType = StructureType.RenderPassBeginInfo, RenderPass = renderPass,
                Framebuffer = framebuffers[imgIndex],
                RenderArea = new Rect2D(new Offset2D(0, 0), extent),
                ClearValueCount = 1, PClearValues = &clearVal
            };
            vk.CmdBeginRenderPass(cmdBuf, &rpBegin, SubpassContents.Inline);

            // 19a. Draw grid (LineList)
            fixed (float* mvpPtr = mvp)
            {
                vk.CmdPushConstants(cmdBuf, pipelineLayout, ShaderStageFlags.VertexBit, 0, 64, mvpPtr);
            }
            // Bind vertex buffer and draw
            var bufferPtr = stackalloc[] { gridBuffer };
            var offsets = stackalloc[] { 0ul };
            vk.CmdBindVertexBuffers(cmdBuf, 0, 1, bufferPtr, offsets);
            vk.CmdDraw(cmdBuf, (uint)gridVertices.Length, 1, 0, 0);
            drawCalls++;

            // 19b. Draw unit (TriangleList)
            var bufferPtr2 = stackalloc[] { unitBuffer };
            vk.CmdBindVertexBuffers(cmdBuf, 0, 1, bufferPtr2, offsets);
            // Re-push constants (same MVP)
            fixed (float* mvpPtr2 = mvp)
            {
                vk.CmdPushConstants(cmdBuf, pipelineLayout, ShaderStageFlags.VertexBit, 0, 64, mvpPtr2);
            }
            vk.CmdDraw(cmdBuf, (uint)unitVertices.Length, 1, 0, 0);
            drawCalls++;

            vk.CmdEndRenderPass(cmdBuf);
            vk.EndCommandBuffer(cmdBuf);

            var queue = default(Queue);
            vk.GetDeviceQueue(dev, qi, 0, out queue);

            // 20. QueueSubmit
            var waitSem = stackalloc[] { semAvail };
            var waitStage = stackalloc[] { PipelineStageFlags.ColorAttachmentOutputBit };
            var sigSem = stackalloc[] { semFin };
            var cBufs = stackalloc[] { cmdBuf };
            var submitInfo = new SubmitInfo
            {
                SType = StructureType.SubmitInfo,
                WaitSemaphoreCount = 1, PWaitSemaphores = waitSem,
                PWaitDstStageMask = waitStage,
                CommandBufferCount = 1, PCommandBuffers = cBufs,
                SignalSemaphoreCount = 1, PSignalSemaphores = sigSem
            };
            if (vk.QueueSubmit(queue, 1, &submitInfo, fence) != Result.Success)
                return Fail("QueueSubmit 失败。", sw);

            // 21. QueuePresent
            var presentFn = Marshal.GetDelegateForFunctionPointer<QueuePresentPtr>(fnQueuePresent);
            var scArr = stackalloc[] { swapchain };
            var idxArr = stackalloc[] { imgIndex };
            var presentInfo = new PresentInfoKHR
            {
                SType = StructureType.PresentInfoKhr,
                WaitSemaphoreCount = 1, PWaitSemaphores = sigSem,
                SwapchainCount = 1, PSwapchains = scArr,
                PImageIndices = idxArr
            };
            var presentRes = presentFn(queue, &presentInfo);
            if (presentRes != Result.Success && presentRes != Result.SuboptimalKhr)
                return Fail($"QueuePresent 失败：{presentRes}。", sw);

            vk.DeviceWaitIdle(dev);
            sw.Stop();

            return new VulkanScene3dInfo(
                VulkanScene3dStatus.Succeeded,
                $"Vulkan 3D 场景绘制成功，Grid：{gridVertices.Length} 顶点/{(gridVertices.Length / 2)} 线段，" +
                $"Unit：{unitVertices.Length} 顶点/{unitVertices.Length / 3} 三角形，" +
                $"DrawCall：{drawCalls}，用时：{sw.Elapsed.TotalMilliseconds:F2} ms。",
                gridVertices.Length, gridVertices.Length / 2,
                unitVertices.Length, unitVertices.Length / 3,
                drawCalls, (int)extent.Width, (int)extent.Height,
                camera.ToSummary(),
                sw.Elapsed.TotalMilliseconds);
        }
        finally
        {
            if (vk is not null) { try { if (dev.Handle != 0) vk.DeviceWaitIdle(dev); } catch { } }
            // Cleanup in reverse order
            if (syncOk && dev.Handle != 0 && vk is not null)
            {
                if (semAvail.Handle != 0) vk.DestroySemaphore(dev, semAvail, null);
                if (semFin.Handle != 0) vk.DestroySemaphore(dev, semFin, null);
                if (fence.Handle != 0) vk.DestroyFence(dev, fence, null);
            }
            if (poolOk && dev.Handle != 0 && vk is not null) vk.DestroyCommandPool(dev, cmdPool, null);
            if (dev.Handle != 0 && vk is not null)
            {
                foreach (var fb in framebuffers) if (fb.Handle != 0) vk.DestroyFramebuffer(dev, fb, null);
            }
            if (unitBufOk && dev.Handle != 0 && vk is not null)
            {
                if (unitBuffer.Handle != 0) vk.DestroyBuffer(dev, unitBuffer, null);
                if (unitMemory.Handle != 0) vk.FreeMemory(dev, unitMemory, null);
            }
            if (gridBufOk && dev.Handle != 0 && vk is not null)
            {
                if (gridBuffer.Handle != 0) vk.DestroyBuffer(dev, gridBuffer, null);
                if (gridMemory.Handle != 0) vk.FreeMemory(dev, gridMemory, null);
            }
            if (unitPipeOk && dev.Handle != 0 && vk is not null) vk.DestroyPipeline(dev, unitPipeline, null);
            if (gridPipeOk && dev.Handle != 0 && vk is not null) vk.DestroyPipeline(dev, gridPipeline, null);
            if (layoutOk && dev.Handle != 0 && vk is not null) vk.DestroyPipelineLayout(dev, pipelineLayout, null);
            if (fragModOk && dev.Handle != 0 && vk is not null) vk.DestroyShaderModule(dev, fragModule, null);
            if (vertModOk && dev.Handle != 0 && vk is not null) vk.DestroyShaderModule(dev, vertModule, null);
            if (rpOk && dev.Handle != 0 && vk is not null && renderPass.Handle != 0) vk.DestroyRenderPass(dev, renderPass, null);
            if (dev.Handle != 0 && vk is not null)
                foreach (var iv in imageViews) if (iv.Handle != 0) vk.DestroyImageView(dev, iv, null);
            if (scOk && fnDestroySwapchain != 0 && vk is not null)
                Marshal.GetDelegateForFunctionPointer<DestroySwapchainPtr>(fnDestroySwapchain)(dev, swapchain, null);
            if (devOk && vk is not null && dev.Handle != 0) vk.DestroyDevice(dev, null);
            if (surfOk && fnDestroySurface != 0 && vk is not null)
                Marshal.GetDelegateForFunctionPointer<DestroySurfacePtr>(fnDestroySurface)(inst, surf, null);
            if (instOk && vk is not null) vk.DestroyInstance(inst, null);
        }
    }

    // ─── Shader Module ──────────────────────────────────────────────

    private static ShaderModule CreateShaderModule(Vk vk, Silk.NET.Vulkan.Device dev, uint[] spirv)
    {
        fixed (uint* code = spirv)
        {
            var ci = new ShaderModuleCreateInfo
            {
                SType = StructureType.ShaderModuleCreateInfo,
                CodeSize = (nuint)(spirv.Length * sizeof(uint)),
                PCode = code
            };
            if (vk.CreateShaderModule(dev, &ci, null, out var module) == Result.Success)
                return module;
        }
        return default;
    }

    // ─── Vertex Buffer ──────────────────────────────────────────────

    private static bool CreateBuffer(Vk vk, Silk.NET.Vulkan.PhysicalDevice pd,
        Silk.NET.Vulkan.Device dev, float[] data,
        out Silk.NET.Vulkan.Buffer buffer, out DeviceMemory memory)
    {
        buffer = default;
        memory = default;
        var size = (nuint)(data.Length * sizeof(float));

        var bCI = new BufferCreateInfo
        {
            SType = StructureType.BufferCreateInfo,
            Size = size,
            Usage = BufferUsageFlags.VertexBufferBit,
            SharingMode = SharingMode.Exclusive
        };
        if (vk.CreateBuffer(dev, &bCI, null, out buffer) != Result.Success)
            return false;

        vk.GetBufferMemoryRequirements(dev, buffer, out var memReqs);
        var allocCI = new MemoryAllocateInfo
        {
            SType = StructureType.MemoryAllocateInfo,
            AllocationSize = memReqs.Size,
            MemoryTypeIndex = FindMemoryType(vk, pd, memReqs.MemoryTypeBits,
                MemoryPropertyFlags.HostVisibleBit | MemoryPropertyFlags.HostCoherentBit)
        };
        if (allocCI.MemoryTypeIndex == uint.MaxValue) return false;

        if (vk.AllocateMemory(dev, &allocCI, null, out memory) != Result.Success)
            return false;

        if (vk.BindBufferMemory(dev, buffer, memory, 0) != Result.Success)
            return false;

        // Upload data
        void* mapped;
        if (vk.MapMemory(dev, memory, 0, size, 0, &mapped) != Result.Success)
            return false;

        fixed (float* src = data)
        {
            System.Buffer.MemoryCopy(src, mapped, (long)size, (long)size);
        }

        vk.UnmapMemory(dev, memory);
        return true;
    }

    private static uint FindMemoryType(Vk vk, Silk.NET.Vulkan.PhysicalDevice pd, uint typeBits,
        MemoryPropertyFlags props)
    {
        vk.GetPhysicalDeviceMemoryProperties(pd, out var memProps);
        for (var i = 0u; i < memProps.MemoryTypeCount; i++)
        {
            if ((typeBits & (1u << (int)i)) != 0 &&
                (memProps.MemoryTypes[(int)i].PropertyFlags & props) == props)
                return i;
        }
        return uint.MaxValue;
    }

    // ─── 辅助方法 ──────────────────────────────────────────────────

    private static nint LoadProc(Vk vk, Silk.NET.Vulkan.Instance inst, string name)
    { var p = Marshal.StringToHGlobalAnsi(name); try { return (nint)vk.GetInstanceProcAddr(inst, (byte*)p); } finally { Marshal.FreeHGlobal(p); } }

    private static nint LoadDeviceProc(Vk vk, Silk.NET.Vulkan.Device dev, string name)
    { var p = Marshal.StringToHGlobalAnsi(name); try { return (nint)vk.GetDeviceProcAddr(dev, (byte*)p); } finally { Marshal.FreeHGlobal(p); } }

    private static bool CreateInstance(Vk vk, out Silk.NET.Vulkan.Instance inst)
    {
        inst = default;
        var a = Marshal.StringToHGlobalAnsi("FluidWarfare"); var e = Marshal.StringToHGlobalAnsi("FluidWarfare");
        var s = Marshal.StringToHGlobalAnsi("VK_KHR_surface"); var w = Marshal.StringToHGlobalAnsi("VK_KHR_win32_surface");
        try
        {
            var exts = stackalloc byte*[] { (byte*)s, (byte*)w };
            var ai = new ApplicationInfo { SType = StructureType.ApplicationInfo, PApplicationName = (byte*)a, ApplicationVersion = 1, PEngineName = (byte*)e, EngineVersion = 1, ApiVersion = PackVer(1, 0, 0) };
            var ci = new InstanceCreateInfo { SType = StructureType.InstanceCreateInfo, PApplicationInfo = &ai, EnabledExtensionCount = 2, PpEnabledExtensionNames = exts };
            return vk.CreateInstance(&ci, null, out inst) == Result.Success;
        }
        finally { Marshal.FreeHGlobal(a); Marshal.FreeHGlobal(e); Marshal.FreeHGlobal(s); Marshal.FreeHGlobal(w); }
    }

    private static bool CreateSurface(Vk vk, Silk.NET.Vulkan.Instance inst, nint hi, nint hw, out SurfaceKHR s)
    {
        s = default;
        var p = Marshal.StringToHGlobalAnsi("vkCreateWin32SurfaceKHR");
        try
        {
            var addr = (nint)vk.GetInstanceProcAddr(inst, (byte*)p);
            if (addr == 0) return false;
            var fn = Marshal.GetDelegateForFunctionPointer<CreateWin32SurfacePtr>(addr);
            var ci = new Win32SurfaceCreateInfoKHR { SType = StructureType.Win32SurfaceCreateInfoKhr, Hinstance = hi, Hwnd = hw };
            fixed (SurfaceKHR* sp = &s) return fn(inst, &ci, null, sp) == Result.Success;
        }
        finally { Marshal.FreeHGlobal(p); }
    }

    private static bool SelectDevice(Vk vk, Silk.NET.Vulkan.Instance inst, SurfaceKHR surf,
        out Silk.NET.Vulkan.PhysicalDevice pd, out uint qi, out string name)
    {
        pd = default; qi = 0; name = "未知";
        uint count = 0;
        if (vk.EnumeratePhysicalDevices(inst, ref count, null) != Result.Success || count == 0) return false;
        var devices = new Silk.NET.Vulkan.PhysicalDevice[count];
        fixed (Silk.NET.Vulkan.PhysicalDevice* p = devices) vk.EnumeratePhysicalDevices(inst, ref count, p);
        var fnSupport = LoadProc(vk, inst, "vkGetPhysicalDeviceSurfaceSupportKHR");
        if (fnSupport == 0) return false;
        var supportFn = Marshal.GetDelegateForFunctionPointer<SurfaceSupportPtr>(fnSupport);
        foreach (var d in devices)
        {
            vk.GetPhysicalDeviceProperties(d, out var props);
            name = Marshal.PtrToStringAnsi((nint)props.DeviceName) ?? "未知";
            uint qc = 0;
            vk.GetPhysicalDeviceQueueFamilyProperties(d, ref qc, null);
            var qProps = new QueueFamilyProperties[qc];
            fixed (QueueFamilyProperties* qp = qProps) vk.GetPhysicalDeviceQueueFamilyProperties(d, ref qc, qp);
            for (uint i = 0; i < qc; i++)
                if (qProps[i].QueueCount > 0 && qProps[i].QueueFlags.HasFlag(QueueFlags.GraphicsBit))
                {
                    int supported = 0;
                    supportFn(d, i, surf, &supported);
                    if (supported != 0) { pd = d; qi = i; return true; }
                }
        }
        return false;
    }

    private static bool CreateDevice(Vk vk, Silk.NET.Vulkan.PhysicalDevice pd, uint qi, out Silk.NET.Vulkan.Device dev)
    {
        dev = default;
        var qp = 1.0f;
        var qci = new DeviceQueueCreateInfo { SType = StructureType.DeviceQueueCreateInfo, QueueFamilyIndex = qi, QueueCount = 1, PQueuePriorities = &qp };
        var se = Marshal.StringToHGlobalAnsi("VK_KHR_swapchain");
        try
        {
            var exts = stackalloc byte*[] { (byte*)se };
            var dci = new DeviceCreateInfo { SType = StructureType.DeviceCreateInfo, QueueCreateInfoCount = 1, PQueueCreateInfos = &qci, EnabledExtensionCount = 1, PpEnabledExtensionNames = exts };
            return vk.CreateDevice(pd, &dci, null, out dev) == Result.Success;
        }
        finally { Marshal.FreeHGlobal(se); }
    }

    private static SurfaceCapabilitiesKHR QueryCaps(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR surf, nint fn)
    { var f = Marshal.GetDelegateForFunctionPointer<GetCapsPtr>(fn); SurfaceCapabilitiesKHR c; f(pd, surf, &c); return c; }

    private static SurfaceFormatKHR[] QueryFormats(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR surf, nint fn)
    {
        var f = Marshal.GetDelegateForFunctionPointer<GetFormatsPtr>(fn);
        uint c = 0; if (f(pd, surf, &c, null) != Result.Success || c == 0) return [];
        var r = new SurfaceFormatKHR[c]; fixed (SurfaceFormatKHR* p = r) f(pd, surf, &c, p); return r;
    }

    private static PresentModeKHR[] QueryModes(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR surf, nint fn)
    {
        var f = Marshal.GetDelegateForFunctionPointer<GetModesPtr>(fn);
        uint c = 0; if (f(pd, surf, &c, null) != Result.Success || c == 0) return [];
        var r = new PresentModeKHR[c]; fixed (PresentModeKHR* p = r) f(pd, surf, &c, p); return r;
    }

    private static SurfaceFormatKHR ChooseFormat(SurfaceFormatKHR[] f)
    { foreach (var x in f) if (x.Format == Format.B8G8R8A8Srgb || x.Format == Format.R8G8B8A8Srgb) return x; return f[0]; }

    private static PresentModeKHR ChoosePresentMode(PresentModeKHR[] m)
    { foreach (var x in m) if (x == PresentModeKHR.MailboxKhr || x == PresentModeKHR.ImmediateKhr) return x; return PresentModeKHR.FifoKhr; }

    private static Extent2D ChooseExtent(SurfaceCapabilitiesKHR c, uint fw, uint fh)
    { if (c.CurrentExtent.Width != uint.MaxValue) return c.CurrentExtent; return new Extent2D(Math.Clamp(fw, c.MinImageExtent.Width, c.MaxImageExtent.Width), Math.Clamp(fh, c.MinImageExtent.Height, c.MaxImageExtent.Height)); }

    private static VulkanScene3dInfo Fail(string msg, Stopwatch sw) =>
        new(VulkanScene3dStatus.Failed, msg, 0, 0, 0, 0, 0, 0, 0, "无", sw.Elapsed.TotalMilliseconds);

    private static uint PackVer(uint a, uint b, uint c) => (a << 22) | (b << 12) | c;

    // ─── 委托定义 ──────────────────────────────────────────────────

    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate Result CreateWin32SurfacePtr(Silk.NET.Vulkan.Instance i, Win32SurfaceCreateInfoKHR* ci, AllocationCallbacks* a, SurfaceKHR* s);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate void DestroySurfacePtr(Silk.NET.Vulkan.Instance i, SurfaceKHR s, AllocationCallbacks* a);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate Result SurfaceSupportPtr(Silk.NET.Vulkan.PhysicalDevice pd, uint qi, SurfaceKHR s, int* supported);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate Result GetCapsPtr(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR s, SurfaceCapabilitiesKHR* c);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate Result GetFormatsPtr(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR s, uint* c, SurfaceFormatKHR* f);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate Result GetModesPtr(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR s, uint* c, PresentModeKHR* m);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate Result CreateSwapchainPtr(Silk.NET.Vulkan.Device d, SwapchainCreateInfoKHR* ci, AllocationCallbacks* a, SwapchainKHR* sc);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate void DestroySwapchainPtr(Silk.NET.Vulkan.Device d, SwapchainKHR sc, AllocationCallbacks* a);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate Result GetSwapchainImagesPtr(Silk.NET.Vulkan.Device d, SwapchainKHR sc, uint* c, Image* imgs);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate Result AcquireNextImagePtr(Silk.NET.Vulkan.Device d, SwapchainKHR sc, ulong t, Silk.NET.Vulkan.Semaphore s, Fence f, uint* i);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate Result QueuePresentPtr(Queue q, PresentInfoKHR* p);
}
