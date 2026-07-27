#version 450

layout(location = 0) in vec4 vBaseColor;
layout(location = 0) out vec4 outColor;

// R4-R3-R2：轮廓由顶点着色器的外轮廓边带生成，片元着色器直接透传基础色。
// 不再使用重心坐标 fwidth 内部边线方案。
void main() {
    outColor = vec4(vBaseColor.rgb, 1.0);
}