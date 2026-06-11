#version 450
// FluidWarfare basic 3D fragment shader
// Input: color from vertex shader
// Output: fragment color

layout(location = 0) in vec4 inColor;
layout(location = 0) out vec4 fragColor;

void main() {
    fragColor = inColor;
}
