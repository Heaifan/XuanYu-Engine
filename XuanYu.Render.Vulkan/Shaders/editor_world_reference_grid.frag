#version 450

// FAR-VIEW-B / RZ-E：World XY / Z=0 参考网格的连续尺度和真实 Reverse-Z 深度。
layout(push_constant) uniform GridPush {
    mat4 viewProjection;
    mat4 inverseViewProjection;
    vec4 cameraPosition;
    vec4 viewportAndFar;
    vec4 gridState;
} pc;

layout(location = 0) in vec4 vFarWorld;
layout(location = 1) in vec4 vNearWorld;
layout(location = 0) out vec4 outColor;

float lineMask(float coordinate) {
    float derivative = max(fwidth(coordinate), 0.000001);
    float distanceToLine = abs(fract(coordinate - 0.5) - 0.5) / derivative;
    return 1.0 - smoothstep(0.65, 1.35, distanceToLine);
}

float densityFade(float coordinate, float spacing) {
    float cellPixels = 1.0 / max(fwidth(coordinate / spacing), 0.000001);
    return smoothstep(6.0, 12.0, cellPixels);
}

float gridLine(vec2 normalizedCoordinate, float density) {
    float xLine = lineMask(normalizedCoordinate.x);
    float yLine = lineMask(normalizedCoordinate.y);
    return max(xLine, yLine) * density;
}

void main() {
    vec3 nearWorld = vNearWorld.xyz / vNearWorld.w;
    vec3 farWorld = vFarWorld.xyz / vFarWorld.w;
    vec3 rayDirection = farWorld - nearWorld;
    if (abs(rayDirection.z) < 0.000001) discard;
    float t = -nearWorld.z / rayDirection.z;
    if (t <= 0.0 || t > pc.viewportAndFar.z) discard;
    vec3 worldPosition = nearWorld + rayDirection * t;

    vec4 clipPosition = pc.viewProjection * vec4(worldPosition, 1.0);
    float depth = clipPosition.z / clipPosition.w;
    if (!(depth >= 0.0 && depth <= 1.0)) discard;
    float fineSpacing = max(pc.gridState.x, 100.0);
    float coarseSpacing = max(pc.gridState.y, fineSpacing);
    float fineWeight = clamp(pc.gridState.z, 0.0, 1.0);
    float coarseWeight = clamp(pc.gridState.w, 0.0, 1.0);
    float fineDensity = min(densityFade(worldPosition.x, fineSpacing),
        densityFade(worldPosition.y, fineSpacing));
    float coarseDensity = min(densityFade(worldPosition.x, coarseSpacing),
        densityFade(worldPosition.y, coarseSpacing));
    float fineLine = gridLine(vec2(worldPosition.x / fineSpacing,
        worldPosition.y / fineSpacing), fineDensity);
    float coarseLine = gridLine(worldPosition.xy / coarseSpacing, coarseDensity);
    float fineContribution = fineLine * 0.16 * fineWeight;
    float coarseContribution = coarseLine * 0.24 * coarseWeight;
    float gridAlpha = max(fineContribution, coarseContribution);
    if (gridAlpha <= 0.0) discard;

    vec3 fineColor = vec3(0.365, 0.400, 0.439);
    vec3 coarseColor = vec3(0.322, 0.361, 0.404);
    float totalContribution = fineContribution + coarseContribution;
    vec3 gridColor = totalContribution > 0.0
        ? (fineColor * fineContribution + coarseColor * coarseContribution) / totalContribution
        : fineColor;
    outColor = vec4(gridColor, gridAlpha);
}
