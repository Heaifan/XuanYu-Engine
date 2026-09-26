#version 450

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

layout(location = 0) out vec2 vUv;

void main() {
    const vec2 positions[6] = vec2[](
        vec2(0.0, 0.0), vec2(1.0, 0.0), vec2(1.0, 1.0),
        vec2(0.0, 0.0), vec2(1.0, 1.0), vec2(0.0, 1.0));
    vec2 quad = positions[gl_VertexIndex];
    vec4 anchor = pc.viewProjection * pc.worldPosition;
    vec2 viewport = max(vec2(pc.entityRotation.w, pc.entityScale.w), vec2(1.0));
    vec2 pixelOffset = (quad - vec2(0.5)) * pc.entityScale.xy;
    vec2 clipOffset = pixelOffset * 2.0 / viewport * anchor.w;
    gl_Position = vec4(anchor.xy + clipOffset, anchor.z, anchor.w);
    vUv = quad;
}
