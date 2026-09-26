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

layout(location = 0) in vec2 vLocal;
layout(location = 1) flat in float vSegmentLength;
layout(location = 0) out vec4 outColor;

float capsuleDistance(vec2 p, float lengthPx, float radiusPx) {
    float nearestX = clamp(p.x, 0.0, lengthPx);
    return length(vec2(p.x - nearestX, p.y)) - radiusPx;
}

void main() {
    float distance = capsuleDistance(vLocal, vSegmentLength, max(pc.gizmoRingRadius, 0.5));
    float coverage = 1.0 - smoothstep(0.0, max(fwidth(distance), 0.0005), distance);
    outColor = vec4(pc.entityRotation.xyz, pc.staticAlpha * coverage);
}
