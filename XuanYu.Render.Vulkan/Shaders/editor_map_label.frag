#version 450

layout(set = 0, binding = 0) uniform sampler2D labelTexture;
layout(push_constant) uniform ScenePush {
    mat4 viewProjection;
    vec4 worldPosition;
    float gizmoMode;
    float gizmoRingRadius;
    float selectionMode;
    float staticAlpha;
    vec4 entityRotation;
    vec4 entityScale;
} pc;
layout(location = 0) in vec2 vUv;
layout(location = 0) out vec4 outColor;

void main() {
    float alpha = texture(labelTexture, vUv).a;
    outColor = vec4(pc.entityRotation.xyz, pc.staticAlpha * alpha);
}
