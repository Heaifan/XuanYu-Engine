#version 450

layout(location = 0) in vec3 inPosition;
layout(location = 1) in vec4 inColor;

layout(location = 0) out vec4 outColor;

layout(push_constant) uniform PushConstants
{
    mat4 mvp;
    vec4 tint; // .rgb = 覆盖色, .a = 混合系数 (0=顶点色, 1=覆盖色)
} pc;

void main()
{
    gl_Position = pc.mvp * vec4(inPosition, 1.0);
    outColor = vec4(
        mix(inColor.rgb, pc.tint.rgb, pc.tint.a),
        inColor.a);
}
