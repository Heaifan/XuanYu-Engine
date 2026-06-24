#version 450

// Input: pixel position and color
layout(location = 0) in vec2 inPixelPosition;
layout(location = 1) in vec4 inColor;

layout(location = 0) out vec4 outColor;

// Push constants: viewport width/height
layout(push_constant) uniform PushConstants
{
    float viewportWidth;
    float viewportHeight;
} pc;

void main()
{
    // Convert pixel coordinates to Vulkan NDC
    // Pixel (0,0) = NDC (-1,+1) — top-left corner
    // Pixel (w, h) = NDC (+1,-1) — bottom-right corner
    float ndcX = (inPixelPosition.x / pc.viewportWidth) * 2.0 - 1.0;
    float ndcY = (inPixelPosition.y / pc.viewportHeight) * 2.0 - 1.0;

    gl_Position = vec4(ndcX, ndcY, 0.0, 1.0);
    outColor = inColor;
}
