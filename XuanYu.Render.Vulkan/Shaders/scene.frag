#version 450

layout(location = 0) in vec4 vBaseColor;
layout(location = 1) in vec3 vBary;
layout(location = 2) in float vSelected;
layout(location = 3) in float vEntity;
layout(location = 0) out vec4 outColor;

void main() {
    vec3 col = vBaseColor.rgb;
    // R4-R3-R1：选中实体（vEntity=1 且 vSelected=1）仅边缘高亮：
    // 用重心坐标 + 屏幕空间导数（fwidth）判定像素距三角形边的距离，
    // 边缘区域输出浅蓝白，内部保持实体黄色；不输出第二张完整浅蓝白面。
    if (vEntity > 0.5 && vSelected > 0.5) {
        vec3 d = fwidth(vBary);
        vec3 lines = smoothstep(vec3(0.0), d * 1.5, vBary);
        float edge = min(min(lines.x, lines.y), lines.z);
        vec3 outline = vec3(0.80, 0.90, 1.0);
        col = mix(outline, col, edge);
    }
    outColor = vec4(col, 1.0);
}
