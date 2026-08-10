#version 450

// GRID-RW-1：颜色与 Alpha 固定；不执行 local LOD、fwidth 或 band-pass。
layout(location = 0) out vec4 outColor;

void main() {
    outColor = vec4(0.322, 0.361, 0.404, 0.22);
}
