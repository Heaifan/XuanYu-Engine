#version 450

layout(push_constant) uniform ScenePush {
    mat4 viewProjection;
    vec4 worldPosition;
    float gizmoMode;
    float gizmoRingRadius;
    float selectionMode;
    float staticAlpha;
    vec4 entityRotation;  // xyz=欧拉角(度), w=viewportWidth
    vec4 entityScale;     // xyz=实体缩放, w=viewportHeight
} pc;

layout(location = 0) in vec3 inPosition;
layout(location = 1) in vec3 inNormal;
layout(location = 2) in vec2 inUv0;
layout(location = 0) out vec4 vBaseColor;

vec3 triangleVertex(int index) {
    vec3 vertices[3] = vec3[3](
        vec3(-0.5, -0.5, 0.0),
        vec3(0.5, -0.5, 0.0),
        vec3(0.0, 0.5, 0.0));
    return vertices[index];
}

vec3 axisDir(int axis) {
    return axis == 0 ? vec3(1.0, 0.0, 0.0) :
        (axis == 1 ? vec3(0.0, 1.0, 0.0) : vec3(0.0, 0.0, 1.0));
}

vec3 axisColor(int axis) {
    return axis == 0 ? vec3(0.91, 0.35, 0.35) :
        (axis == 1 ? vec3(0.27, 0.66, 0.40) : vec3(0.30, 0.50, 0.93));
}

vec3 planeVertex(int plane, int index) {
    vec3 a = plane == 2 ? vec3(0, 1, 0) : vec3(1, 0, 0);
    vec3 b = plane == 0 ? vec3(0, 1, 0) : vec3(0, 0, 1);
    // The visible square shares the arrow's screen-constant world radius.
    float L = pc.gizmoRingRadius;
    float i = L * (12.0 / 90.0);
    float o = i + (L * (16.0 / 90.0));
    vec3 p0 = (a * i) + (b * i);
    vec3 p1 = (a * o) + (b * i);
    vec3 p2 = (a * o) + (b * o);
    vec3 p3 = (a * i) + (b * i);
    vec3 p4 = (a * o) + (b * o);
    vec3 p5 = (a * i) + (b * o);
    vec3 vertices[6] = vec3[6](
        p0, p1, p2,
        p0, p2, p5);
    return vertices[index];
}

mat3 eulerRot(vec3 deg) {
    vec3 r = radians(deg);
    float cx = cos(r.x), sx = sin(r.x);
    float cy = cos(r.y), sy = sin(r.y);
    float cz = cos(r.z), sz = sin(r.z);
    mat3 Rx = mat3(1.0, 0.0, 0.0, 0.0, cx, sx, 0.0, -sx, cx);
    mat3 Ry = mat3(cy, 0.0, sy, 0.0, 1.0, 0.0, -sy, 0.0, cy);
    mat3 Rz = mat3(cz, -sz, 0.0, sz, cz, 0.0, 0.0, 0.0, 1.0);
    return Rz * Ry * Rx;
}

vec3 ringVertex(int ring, int seg, int vert) {
    float R = pc.gizmoRingRadius;
    float w = R * 0.012;
    vec3 b1 = ring == 0 ? vec3(0.0, 1.0, 0.0) : (ring == 1 ? vec3(1.0, 0.0, 0.0) : vec3(1.0, 0.0, 0.0));
    vec3 b2 = ring == 0 ? vec3(0.0, 0.0, 1.0) : (ring == 1 ? vec3(0.0, 0.0, 1.0) : vec3(0.0, 1.0, 0.0));
    float t1 = float(seg) * 6.2831853 / 48.0;
    float t2 = float(seg + 1) * 6.2831853 / 48.0;
    vec3 d1 = (cos(t1) * b1) + (sin(t1) * b2);
    vec3 d2 = (cos(t2) * b1) + (sin(t2) * b2);
    vec3 ro1 = (R + w) * d1; vec3 ri1 = (R - w) * d1;
    vec3 ro2 = (R + w) * d2; vec3 ri2 = (R - w) * d2;
    vec3 c[6] = vec3[6](ro1, ro2, ri2, ro1, ri2, ri1);
    return c[vert];
}

// R4-R3-R2：外轮廓边带。每条边生成 6 顶点（2 三角形），3 条边共 18 顶点。
// 用屏幕空间法线偏移生成窄四边形，不依赖重心坐标或片元内部线。
void outlineRibbonVertex(int vi, out vec4 clipPos, out vec4 color) {
    int edgeIdx = vi / 6;
    int cornerIdx = vi % 6;
    // 边端点（局部空间）
    int i0 = edgeIdx;
    int i1 = (edgeIdx + 1) % 3;
    vec3 local0 = triangleVertex(i0);
    vec3 local1 = triangleVertex(i1);
    // 实体变换
    vec3 s0 = local0 * pc.entityScale.xyz;
    vec3 s1 = local1 * pc.entityScale.xyz;
    mat3 R = eulerRot(pc.entityRotation.xyz);
    vec3 w0 = R * s0 + pc.worldPosition.xyz;
    vec3 w1 = R * s1 + pc.worldPosition.xyz;
    // 裁剪空间
    vec4 c0 = pc.viewProjection * vec4(w0, 1.0);
    vec4 c1 = pc.viewProjection * vec4(w1, 1.0);
    // 屏幕空间边方向与法线（NDC 空间）
    vec2 ndc0 = c0.xy / c0.w;
    vec2 ndc1 = c1.xy / c1.w;
    vec2 edgeDir = normalize(ndc1 - ndc0);
    vec2 perp = vec2(-edgeDir.y, edgeDir.x);
    // 像素偏移 → NDC 偏移（目标线宽 3 DIP，半宽 1.5 px）
    float vpW = pc.entityRotation.w;
    float vpH = pc.entityScale.w;
    float halfWidth = 1.5;
    vec2 ndcOffset = perp * (halfWidth * 2.0 / vec2(vpW, vpH));
    // 四边形角点映射：0=A(start,-), 1=B(start,+), 2=C(end,-), 3=B, 4=D(end,+), 5=C
    bool useEnd = (cornerIdx == 2 || cornerIdx == 4 || cornerIdx == 5);
    bool usePos = (cornerIdx == 1 || cornerIdx == 3 || cornerIdx == 4);
    vec4 base = useEnd ? c1 : c0;
    float sign = usePos ? 1.0 : -1.0;
    clipPos = base;
    clipPos.xy += sign * ndcOffset * base.w;
    color = vec4(0.80, 0.90, 1.0, 1.0); // 浅蓝白轮廓
}

vec3 cube(vec3 center, vec3 halfExtent, int li) {
    vec3 corners[8] = vec3[8](
        center + vec3(-halfExtent.x, -halfExtent.y, -halfExtent.z),
        center + vec3( halfExtent.x, -halfExtent.y, -halfExtent.z),
        center + vec3( halfExtent.x,  halfExtent.y, -halfExtent.z),
        center + vec3(-halfExtent.x,  halfExtent.y, -halfExtent.z),
        center + vec3(-halfExtent.x, -halfExtent.y,  halfExtent.z),
        center + vec3( halfExtent.x, -halfExtent.y,  halfExtent.z),
        center + vec3( halfExtent.x,  halfExtent.y,  halfExtent.z),
        center + vec3(-halfExtent.x,  halfExtent.y,  halfExtent.z));
    int idx[36] = int[36](
        0,1,2, 0,2,3,   4,5,6, 4,6,7,
        0,1,5, 0,5,4,   3,2,6, 3,6,7,
        1,2,6, 1,6,5,   0,3,7, 0,7,4);
    return corners[idx[li]];
}

vec3 moveRodVertex(int axis, int li) {
    float L = pc.gizmoRingRadius;
    float bar = L * 0.018;
    vec3 a = axisDir(axis);
    vec3 halfExtent = axis == 0 ? vec3(L * 0.36, bar, bar)
        : (axis == 1 ? vec3(bar, L * 0.36, bar) : vec3(bar, bar, L * 0.36));
    return cube(a * (L * 0.42), halfExtent, li);
}

vec3 moveArrowVertex(int axis, int li) {
    float L = pc.gizmoRingRadius;
    vec3 a = axisDir(axis);
    vec3 u = axis == 0 ? vec3(0, 1, 0) : vec3(1, 0, 0);
    vec3 v = axis == 2 ? vec3(0, 1, 0) : vec3(0, 0, 1);
    vec3 tip = a * (L * 0.94);
    vec3 base = a * (L * 0.74);
    float s = L * 0.075;
    vec3 b0 = base + u * s;
    vec3 b1 = base + v * s;
    vec3 b2 = base - u * s;
    vec3 b3 = base - v * s;
    vec3 p[18] = vec3[18](
        tip,b0,b1, tip,b1,b2, tip,b2,b3, tip,b3,b0,
        b0,b2,b1, b0,b3,b2);
    return p[li];
}

vec3 neutralCenterVertex(int li, float size) {
    return cube(vec3(0.0), vec3(size), li);
}

vec3 cubeCorner(int index) {
    vec3 c[8] = vec3[8](
        vec3(-0.5,-0.5,-0.5), vec3(0.5,-0.5,-0.5),
        vec3(0.5,0.5,-0.5), vec3(-0.5,0.5,-0.5),
        vec3(-0.5,-0.5,0.5), vec3(0.5,-0.5,0.5),
        vec3(0.5,0.5,0.5), vec3(-0.5,0.5,0.5));
    return c[index];
}

void cubeOutlineVertex(int vi, out vec4 clipPos, out vec4 color) {
    int ends[24] = int[24](0,1, 1,2, 2,3, 3,0, 4,5, 5,6,
        6,7, 7,4, 0,4, 1,5, 2,6, 3,7);
    int edge = vi / 6;
    int corner = vi % 6;
    mat3 R = eulerRot(pc.entityRotation.xyz);
    vec3 w0 = R * (cubeCorner(ends[edge * 2]) * pc.entityScale.xyz) + pc.worldPosition.xyz;
    vec3 w1 = R * (cubeCorner(ends[edge * 2 + 1]) * pc.entityScale.xyz) + pc.worldPosition.xyz;
    vec4 c0 = pc.viewProjection * vec4(w0, 1.0);
    vec4 c1 = pc.viewProjection * vec4(w1, 1.0);
    vec2 d = normalize((c1.xy / c1.w) - (c0.xy / c0.w));
    vec2 offset = vec2(-d.y, d.x) * (3.0 / vec2(pc.entityRotation.w, pc.entityScale.w));
    bool useEnd = corner == 2 || corner == 4 || corner == 5;
    bool usePos = corner == 1 || corner == 3 || corner == 4;
    clipPos = useEnd ? c1 : c0;
    clipPos.xy += (usePos ? 1.0 : -1.0) * offset * clipPos.w;
    color = vec4(0.80, 0.90, 1.0, 1.0);
}

// Scale Gizmo 几何（相对 Gizmo 中心；无可见 Global/Local 切换前由上游传入零旋转，锁定世界轴）：
// [0..107] 三轴杆（沿旋转轴方向拉长）；[108..215] 三轴端立方体；[216..251] 中心等比立方体。
vec3 scaleVertex(int vi) {
    float L = pc.gizmoRingRadius;
    if (vi < 108) {
        int b = vi / 36;
        int li = vi % 36;
        vec3 a = (b == 0) ? vec3(1.0, 0.0, 0.0)
            : (b == 1 ? vec3(0.0, 1.0, 0.0) : vec3(0.0, 0.0, 1.0));
        float bar = L * 0.025;
        vec3 halfExtent = (b == 0) ? vec3(L * 0.8, bar, bar)
            : (b == 1 ? vec3(bar, L * 0.8, bar) : vec3(bar, bar, L * 0.8));
        vec3 centerBox = a * (L * 0.42);
        return eulerRot(pc.entityRotation.xyz) * cube(centerBox, halfExtent, li);
    } else if (vi < 216) {
        int c = (vi - 108) / 36;
        int li = (vi - 108) % 36;
        vec3 a = (c == 0) ? vec3(1.0, 0.0, 0.0)
            : (c == 1 ? vec3(0.0, 1.0, 0.0) : vec3(0.0, 0.0, 1.0));
        vec3 centerBox = a * L;
        vec3 halfExtent = vec3(L * 0.09);
        return eulerRot(pc.entityRotation.xyz) * cube(centerBox, halfExtent, li);
    } else {
        int li = vi - 216;
        return cube(vec3(0.0), vec3(L * 0.12), li);
    }
}

vec3 quadCorner(vec3 center, vec3 halfExtent, int li) {
    return cube(center, halfExtent, li);
}

void backgroundVertex(int vi, out vec4 clipPos, out vec4 color) {
    vec2 p[3] = vec2[3](vec2(-1.0, -1.0), vec2(3.0, -1.0), vec2(-1.0, 3.0));
    clipPos = vec4(p[vi], 1.0, 1.0);
    // WORLD-D-R1：程序化天空。逆视图投影恢复世界观察方向，按世界 Z 分量
    // （右手系 Z 轴向上）判断上下半球，地平线保持世界空间稳定。
    mat4 invVP = inverse(pc.viewProjection);
    vec4 farWorld = invVP * vec4(p[vi], 1.0, 1.0);
    vec4 camWorld = invVP * vec4(0.0, 0.0, 0.0, 1.0);
    vec3 dir = normalize(farWorld.xyz / farWorld.w - camWorld.xyz / camWorld.w);
    float up = clamp(dir.z, 0.0, 1.0);      // 上半球：0=地平线 .. 1=正上方
    float down = clamp(-dir.z, 0.0, 1.0);   // 下半球：0=地平线 .. 1=正下方
    vec3 skyTop = vec3(0.45, 0.56, 0.74);   // 天空顶部：较深但明亮的浅蓝
    vec3 horizon = vec3(0.88, 0.90, 0.94);  // 地平线：更浅、雾白蓝
    vec3 ground = vec3(0.55, 0.58, 0.62);   // 下半球：浅中性灰蓝/淡地面色
    vec3 rgb = dir.z >= 0.0
        ? mix(horizon, skyTop, smoothstep(0.0, 0.7, up))
        : mix(horizon, ground, smoothstep(0.0, 0.45, down));
    color = vec4(rgb, 1.0);
}

vec3 gridVertex(int vi, out vec4 color) {
    int line = vi / 6;
    int corner = vi % 6;
    bool xLine = line < 21;
    int offsetIndex = xLine ? line : line - 21;
    float coord = float(offsetIndex - 10);
    bool major = (offsetIndex % 5) == 0;
    float gridHalf = 10.0;
    float w = major ? 0.012 : 0.005;
    vec3 center = xLine ? vec3(0.0, coord, 0.0) : vec3(coord, 0.0, 0.0);
    vec3 extent = xLine ? vec3(gridHalf, w, 0.0) : vec3(w, gridHalf, 0.0);
    color = major ? vec4(0.43, 0.49, 0.54, 1.0) : vec4(0.54, 0.59, 0.63, 1.0);
    return quadCorner(center, extent, corner);
}

vec3 originVertex(int vi, out vec4 color) {
    color = vec4(0.82, 0.78, 0.64, 1.0);
    return cube(vec3(0.0, 0.0, 0.015), vec3(0.055, 0.055, 0.015), vi);
}

vec3 axisVertex(int vi, out vec4 color) {
    int axis = vi / 36;
    int li = vi % 36;
    vec3 center = axis == 0 ? vec3(1.25, 0.0, 0.02)
        : (axis == 1 ? vec3(0.0, 1.25, 0.02) : vec3(0.0, 0.0, 1.25));
    vec3 extent = axis == 0 ? vec3(1.25, 0.01, 0.01)
        : (axis == 1 ? vec3(0.01, 1.25, 0.01) : vec3(0.01, 0.01, 1.25));
    color = axis == 0 ? vec4(0.54, 0.36, 0.34, 1.0)
        : (axis == 1 ? vec4(0.35, 0.50, 0.39, 1.0) : vec4(0.36, 0.43, 0.56, 1.0));
    return cube(center, extent, li);
}

void main() {
    if (pc.gizmoMode < -14.5) {
        // MAP-A-R1-D4：地图边界线（CPU 顶点，世界坐标），亮琥珀色。
        gl_Position = pc.viewProjection * vec4(inPosition, 1.0);
        vBaseColor = vec4(0.96, 0.72, 0.25, 1.0);
    } else if (pc.gizmoMode < -13.5) {
        // MAP-A-R1-D4：有限地表（CPU 网格顶点：世界坐标 + 法线 + 预计算亮度 uv.x）。
        // 亮度由 MapTerrainMeshBuilder 用同一 MapSurfaceSampler 与 sunDirection 预计算。
        vec3 n = normalize(inNormal);
        float brightness = inUv0.x;
        vec3 base = vec3(0.42, 0.52, 0.36);   // 地表基色：土绿
        vec3 shaded = base * clamp(brightness, 0.0, 1.0);
        gl_Position = pc.viewProjection * vec4(inPosition, 1.0);
        vBaseColor = vec4(min(shaded, vec3(1.0)), 1.0);
    } else if (pc.gizmoMode > -3.5 && pc.gizmoMode < -2.5) {
        mat3 R = eulerRot(pc.entityRotation.xyz);
        vec3 local = inPosition * pc.entityScale.xyz;
        vec3 n = normalize(R * (inNormal / max(abs(pc.entityScale.xyz), vec3(0.0001))));
        vec3 base = vec3(pc.worldPosition.w, pc.gizmoRingRadius, pc.selectionMode);
        // WORLD-D-R1：固定世界方向光（指向光源方向）+ 半球环境光（世界 Z 轴向上）。
        vec3 lightDir = normalize(vec3(0.35, 0.55, 0.75));   // 斜上方固定方向，不随相机/模型变化
        vec3 lightColor = vec3(1.0, 0.98, 0.94);             // 近白微暖
        float nlen = length(n);
        float ndl = nlen > 0.0001 ? max(dot(n, lightDir), 0.0) : 0.0;   // 缺法线安全回退：无漫反射
        float hemi = nlen > 0.0001 ? clamp(n.z * 0.5 + 0.5, 0.0, 1.0) : 0.5; // 朝上=天空、朝下=地面
        vec3 skyAmbient = vec3(0.54, 0.60, 0.68);            // 略冷、稍亮
        vec3 groundAmbient = vec3(0.40, 0.41, 0.43);         // 中性、稍暗
        vec3 ambient = mix(groundAmbient, skyAmbient, hemi);
        vec3 shaded = base * (ambient + (lightColor * ndl * 0.50));
        gl_Position = pc.viewProjection * vec4((R * local) + pc.worldPosition.xyz, 1.0);
        vBaseColor = vec4(min(shaded, vec3(1.0)), pc.staticAlpha);
    } else if (pc.gizmoMode < -12.5) {
        vec4 color;
        vec3 local = axisVertex(gl_VertexIndex, color);
        gl_Position = pc.viewProjection * vec4(local, 1.0);
        vBaseColor = color;
    } else if (pc.gizmoMode < -11.5) {
        vec4 color;
        vec3 local = originVertex(gl_VertexIndex, color);
        gl_Position = pc.viewProjection * vec4(local, 1.0);
        vBaseColor = color;
    } else if (pc.gizmoMode < -10.5) {
        vec4 color;
        vec3 local = gridVertex(gl_VertexIndex, color);
        gl_Position = pc.viewProjection * vec4(local, 1.0);
        vBaseColor = color;
    } else if (pc.gizmoMode < -9.5) {
        vec4 clipPos; vec4 color;
        backgroundVertex(gl_VertexIndex, clipPos, color);
        gl_Position = clipPos;
        vBaseColor = color;
    } else if (pc.gizmoMode > -1.5 && pc.gizmoMode < -0.5 && pc.selectionMode > 1.5) {
        vec4 clipPos; vec4 color;
        cubeOutlineVertex(gl_VertexIndex, clipPos, color);
        gl_Position = clipPos;
        vBaseColor = color;
    } else if (pc.gizmoMode > -1.5 && pc.gizmoMode < -0.5) {
        vec3 local = cube(vec3(0.0), vec3(0.5), gl_VertexIndex);
        local = eulerRot(pc.entityRotation.xyz) * (local * pc.entityScale.xyz);
        gl_Position = pc.viewProjection * vec4(local + pc.worldPosition.xyz, 1.0);
        vBaseColor = vec4(0.72, 0.76, 0.82, 1.0);
    } else if (pc.selectionMode > 1.5) {
        // R4-R3-R2：外轮廓边带（18 顶点），非重心坐标内部线、非放大复制面
        vec4 clipPos; vec4 color;
        outlineRibbonVertex(gl_VertexIndex, clipPos, color);
        gl_Position = clipPos;
        vBaseColor = color;
    } else if (pc.gizmoMode < -1.5 && gl_VertexIndex < 3) {
        // 实体三角形填充
        vec3 local = triangleVertex(gl_VertexIndex);
        local = local * pc.entityScale.xyz;
        local = eulerRot(pc.entityRotation.xyz) * local;
        vec3 world = local + pc.worldPosition.xyz;
        gl_Position = pc.viewProjection * vec4(world, 1.0);
        vBaseColor = vec4(1.0, 0.85, 0.2, 1.0);
    } else if (pc.gizmoMode < 0.5) {
        int gi = gl_VertexIndex;
        if (gi < 18) {
            int plane = gi / 6;
            vec3 world = planeVertex(plane, gi % 6) + pc.worldPosition.xyz;
            vBaseColor = plane == 0 ? vec4(0.76, 0.72, 0.67, 0.18) :
                (plane == 1 ? vec4(0.71, 0.73, 0.80, 0.18) : vec4(0.69, 0.76, 0.73, 0.18));
            gl_Position = pc.viewProjection * vec4(world, 1.0);
        } else if (gi < 126) {
            int axis = (gi - 18) / 36;
            vec3 world = moveRodVertex(axis, (gi - 18) % 36) + pc.worldPosition.xyz;
            vBaseColor = vec4(axisColor(axis), 1.0);
            gl_Position = pc.viewProjection * vec4(world, 1.0);
        } else if (gi < 180) {
            int axis = (gi - 126) / 18;
            vec3 world = moveArrowVertex(axis, (gi - 126) % 18) + pc.worldPosition.xyz;
            vBaseColor = vec4(axisColor(axis), 1.0);
            gl_Position = pc.viewProjection * vec4(world, 1.0);
        } else {
            vec3 world = neutralCenterVertex(gi - 180, pc.gizmoRingRadius * 0.075) + pc.worldPosition.xyz;
            vBaseColor = vec4(0.96, 0.97, 0.98, 1.0);
            gl_Position = pc.viewProjection * vec4(world, 1.0);
        }
    } else if (pc.gizmoMode > 1.5) {
        // Scale Gizmo：三轴杆 + 三轴端立方体 + 中心等比立方体
        int vi = gl_VertexIndex;
        vec3 local = scaleVertex(vi);
        vec3 world = local + pc.worldPosition.xyz;
        gl_Position = pc.viewProjection * vec4(world, 1.0);
        vec3 col;
        if (vi < 108) {
            int b = vi / 36;
            col = axisColor(b);
        } else if (vi < 216) {
            int c = (vi - 108) / 36;
            col = axisColor(c);
        } else {
            col = vec3(0.96, 0.97, 0.98);
        }
        vBaseColor = vec4(col, 1.0);
    } else {
        int ri = gl_VertexIndex;
        if (ri < 864) {
            int ring = ri / (48 * 6);
            int seg = (ri % (48 * 6)) / 6;
            int vert = ri % 6;
            vec3 world = ringVertex(ring, seg, vert) + pc.worldPosition.xyz;
            vBaseColor = vec4(axisColor(ring), 1.0);
            gl_Position = pc.viewProjection * vec4(world, 1.0);
        } else {
            vec3 world = neutralCenterVertex(ri - 864, pc.gizmoRingRadius * 0.06) + pc.worldPosition.xyz;
            vBaseColor = vec4(0.96, 0.97, 0.98, 1.0);
            gl_Position = pc.viewProjection * vec4(world, 1.0);
        }
    }
}
