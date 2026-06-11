#version 450
// FluidWarfare basic 3D vertex shader
// Input: position (vec3) + color (vec4)
// Push constant: MVP matrix (mat4)
// Output: color to fragment shader

layout(location = 0) in vec3 inPosition;
layout(location = 1) in vec4 inColor;

layout(location = 0) out vec4 outColor;

layout(push_constant) uniform PushConstants {
    mat4 mvp;
} pc;

void main() {
    gl_Position = pc.mvp * vec4(inPosition, 1.0);
    outColor = inColor;
}
