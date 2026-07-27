#version 450

layout(push_constant) uniform ScenePush {
    mat4 viewProjection;
    vec4 worldPosition;
    float gizmoMode;
    float gizmoRingRadius;
    float outlineMode;
    vec4 entityRotation;
    vec4 entityScale;
} pc;

layout(location = 0) out vec4 outColor;

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

// 由欧拉角（度）构造旋转矩阵，约定 Rz * Ry * Rx。
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

// 旋转环：每个环在垂直于该轴的平面内生成 48 段细环带（每环 288 顶点，3 环共 864）。
// 环半径取自 push 常量 gizmoRingRadius（CPU 按屏幕空间恒定尺寸换算的世界半径）。
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

void main() {
    vec3 world;
    if (gl_VertexIndex < 3) {
        // 实体三角形：应用缩放 → 旋转 → 平移，使世界 Rotation/Scale 真正影响画面。
        // R4-R3：outlineMode=1 时先放大作底（浅蓝白），实体三角形随后压顶形成轮廓高亮。
        vec3 local = triangleVertex(gl_VertexIndex);
        if (pc.outlineMode > 0.5) local = local * 1.16;
        local = local * pc.entityScale.xyz;
        local = eulerRot(pc.entityRotation.xyz) * local;
        world = local + pc.worldPosition.xyz;
        outColor = pc.outlineMode > 0.5 ? vec4(0.80, 0.90, 1.0, 1.0) : vec4(1.0, 0.85, 0.2, 1.0);
    } else if (pc.gizmoMode < 0.5) {
        int gi = gl_VertexIndex - 3;
        if (gi < 18) {
            int plane = gi / 6;
            world = planeVertex(plane, gi % 6) + pc.worldPosition.xyz;
            outColor = plane == 0 ? vec4(0.82, 0.66, 0.16, 1.0) :
                (plane == 1 ? vec4(0.64, 0.26, 0.82, 1.0) : vec4(0.16, 0.68, 0.76, 1.0));
        } else {
            int axis = (gi - 18) / 6;
            world = gizmoVertex(axis, (gi - 18) % 6) + pc.worldPosition.xyz;
            outColor = axis == 0 ? vec4(0.9, 0.18, 0.16, 1.0) :
                (axis == 1 ? vec4(0.16, 0.72, 0.28, 1.0) : vec4(0.18, 0.42, 0.95, 1.0));
        }
    } else {
        int ri = gl_VertexIndex - 3;
        int ring = ri / (48 * 6);
        int seg = (ri % (48 * 6)) / 6;
        int vert = ri % 6;
        world = ringVertex(ring, seg, vert) + pc.worldPosition.xyz;
        outColor = ring == 0 ? vec4(0.9, 0.18, 0.16, 1.0) :
            (ring == 1 ? vec4(0.16, 0.72, 0.28, 1.0) : vec4(0.18, 0.42, 0.95, 1.0));
    }
    gl_Position = pc.viewProjection * vec4(world, 1.0);
}
