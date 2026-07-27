#version 450

layout(push_constant) uniform ScenePush {
    mat4 viewProjection;
    vec4 worldPosition;
    float gizmoMode;
    float gizmoRingRadius;
    float selectionMode;
    vec4 entityRotation;  // xyz=欧拉角(度), w=viewportWidth
    vec4 entityScale;     // xyz=实体缩放, w=viewportHeight
} pc;

layout(location = 0) out vec4 vBaseColor;

vec3 triangleVertex(int index) {
    vec3 vertices[3] = vec3[3](
        vec3(-0.5, -0.5, 0.0),
        vec3(0.5, -0.5, 0.0),
        vec3(0.0, 0.5, 0.0));
    return vertices[index];
}

vec3 gizmoVertex(int axis, int index) {
    vec3 direction = axis == 0 ? vec3(1.2, 0, 0) :
        (axis == 1 ? vec3(0, 1.2, 0) : vec3(0, 0, 1.2));
    vec3 side = axis == 1 ? vec3(0.035, 0, 0) : vec3(0, 0.035, 0);
    vec3 vertices[6] = vec3[6](
        -side, side, direction + side,
        -side, direction + side, direction - side);
    return vertices[index];
}

vec3 planeVertex(int plane, int index) {
    vec3 a = plane == 2 ? vec3(0, 1, 0) : vec3(1, 0, 0);
    vec3 b = plane == 0 ? vec3(0, 1, 0) : vec3(0, 0, 1);
    float i = 0.22;
    float o = 0.60;
    vec3 vertices[6] = vec3[6](
        (a * i) + (b * i), (a * o) + (b * i), (a * o) + (b * o),
        (a * i) + (b * i), (a * o) + (b * o), (a * i) + (b * o));
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
    float w = R * 0.025;
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

void main() {
    if (pc.selectionMode > 1.5) {
        // R4-R3-R2：外轮廓边带（18 顶点），非重心坐标内部线、非放大复制面
        vec4 clipPos; vec4 color;
        outlineRibbonVertex(gl_VertexIndex, clipPos, color);
        gl_Position = clipPos;
        vBaseColor = color;
    } else if (gl_VertexIndex < 3) {
        // 实体三角形填充
        vec3 local = triangleVertex(gl_VertexIndex);
        local = local * pc.entityScale.xyz;
        local = eulerRot(pc.entityRotation.xyz) * local;
        vec3 world = local + pc.worldPosition.xyz;
        gl_Position = pc.viewProjection * vec4(world, 1.0);
        vBaseColor = vec4(1.0, 0.85, 0.2, 1.0);
    } else if (pc.gizmoMode < 0.5) {
        int gi = gl_VertexIndex - 3;
        if (gi < 18) {
            int plane = gi / 6;
            vec3 world = planeVertex(plane, gi % 6) + pc.worldPosition.xyz;
            vBaseColor = plane == 0 ? vec4(0.82, 0.66, 0.16, 1.0) :
                (plane == 1 ? vec4(0.64, 0.26, 0.82, 1.0) : vec4(0.16, 0.68, 0.76, 1.0));
            gl_Position = pc.viewProjection * vec4(world, 1.0);
        } else {
            int axis = (gi - 18) / 6;
            vec3 world = gizmoVertex(axis, (gi - 18) % 6) + pc.worldPosition.xyz;
            vBaseColor = axis == 0 ? vec4(0.9, 0.18, 0.16, 1.0) :
                (axis == 1 ? vec4(0.16, 0.72, 0.28, 1.0) : vec4(0.18, 0.42, 0.95, 1.0));
            gl_Position = pc.viewProjection * vec4(world, 1.0);
        }
    } else {
        int ri = gl_VertexIndex - 3;
        int ring = ri / (48 * 6);
        int seg = (ri % (48 * 6)) / 6;
        int vert = ri % 6;
        vec3 world = ringVertex(ring, seg, vert) + pc.worldPosition.xyz;
        vBaseColor = ring == 0 ? vec4(0.9, 0.18, 0.16, 1.0) :
            (ring == 1 ? vec4(0.16, 0.72, 0.28, 1.0) : vec4(0.18, 0.42, 0.95, 1.0));
        gl_Position = pc.viewProjection * vec4(world, 1.0);
    }
}