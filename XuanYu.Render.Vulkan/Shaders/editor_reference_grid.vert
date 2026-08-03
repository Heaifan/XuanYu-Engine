#version 450

// MAP-A-R1-D5-R1-F2：独立编辑器参考网格 Pass —— 顶点着色器。
// 全屏三角形；为片元输出近点/远点的世界坐标（透视校正插值）。
// 不依赖 scene shader 的 gizmoMode 魔法分支。

layout(push_constant) uniform GridPush {
    mat4 viewProjection;        // 0   世界→裁剪（深度投影用）
    mat4 inverseViewProjection; // 64  裁剪→世界（射线重建）
    vec4 cameraPosition;        // 128 相机世界位置
    vec4 viewportAndFar;        // 144 x,y=视口尺寸; z=Far; w=GridMaxDistance
} pc;

layout(location = 0) out vec4 vFarWorld;
layout(location = 1) out vec4 vNearWorld;

void main() {
    vec2 p[3] = vec2[3](vec2(-1.0, -1.0), vec2(3.0, -1.0), vec2(-1.0, 3.0));
    vec2 ndc = p[gl_VertexIndex];
    gl_Position = vec4(ndc, 1.0, 1.0);
    vFarWorld = pc.inverseViewProjection * vec4(ndc, 1.0, 1.0);
    vNearWorld = pc.inverseViewProjection * vec4(ndc, 0.0, 1.0);
}
