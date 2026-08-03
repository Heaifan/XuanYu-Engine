#version 450

// MAP-A-R1-D5-R1-F3-F1：导航 Gizmo Overlay Pass —— 顶点着色器。
// 纯屏幕空间：全屏三角形，直接输出 NDC（不需要世界射线）。

layout(location = 0) out vec2 vNdc;

void main() {
    vec2 p[3] = vec2[3](vec2(-1.0, -1.0), vec2(3.0, -1.0), vec2(-1.0, 3.0));
    vec2 ndc = p[gl_VertexIndex];
    gl_Position = vec4(ndc, 0.0, 1.0);
    vNdc = ndc;
}
