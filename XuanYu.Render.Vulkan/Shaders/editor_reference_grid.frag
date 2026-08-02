#version 450

// MAP-A-R1-D5-R1-F2A：Blender 风格自适应参考网格 —— 片元着色器。
// 世界射线与 Z=0 平面求交；按 worldMetersPerPixel 选两个相邻十进制层级；
// fract/fwidth 屏幕恒定像素线宽；距离淡出 + 掠射角淡出；X/Y 主轴贯穿；
// 地图矩形内逐片元 discard。独立 Pass（不依赖 scene shader 魔法分支）。
//
// 配色（玄域浅色编辑器，克制蓝灰体系，禁高饱和/荧光/红绿工程轴）：
//   细网格 #7E8FA1  α 0.18   主网格 #607487  α 0.32
//   X 主轴 #5A7FA3  α 0.78   Y 主轴 #B68B54  α 0.78
//   原点   #D1AE69  α 0.85

layout(push_constant) uniform GridPush {
    mat4 viewProjection;
    mat4 inverseViewProjection;
    vec4 cameraPosition;
    vec4 viewportAndFar;   // x,y=视口; z=Far; w=gridMaxDistance
    vec4 mapParams;        // x=HasMap; y=MapCenterX; z=MapCenterY; w=MapHalfWidth
    vec4 mapParams2;       // x=MapHalfDepth
} pc;

layout(location = 0) in vec4 vFarWorld;
layout(location = 1) in vec4 vNearWorld;
layout(location = 0) out vec4 outColor;

// 合法层级（十进制）。
const float MIN_STEP = 0.1;
const float MAX_STEP = 10000.0;

// 屏幕恒定抗锯齿线：worldXY/step 的 fract+fwidth，返回线强度（0..1）。
// linePixels 控制线宽（1≈细格 1px，2≈主格 1.5~2px）。
float gridLine(vec2 worldXY, float step, float linePixels) {
    vec2 g = worldXY / step;
    vec2 f = abs(fract(g) - 0.5);
    vec2 d = max(fwidth(g), vec2(0.000001));
    vec2 edge = vec2(0.5) - d * (linePixels * 0.5);
    vec2 line = smoothstep(vec2(0.5), edge, f);
    return max(line.x, line.y);
}

void main() {
    vec3 nearWorld = vNearWorld.xyz / vNearWorld.w;
    vec3 farWorld = vFarWorld.xyz / vFarWorld.w;
    vec3 rayDirection = farWorld - nearWorld;

    // 保护 1：近似平行于 Z=0 平面。
    if (abs(rayDirection.z) < 0.001) discard;
    // 保护 2：交点在相机后方。
    float t = -nearWorld.z / rayDirection.z;
    if (t <= 0.0) discard;
    // 保护 3：超出最大距离。
    vec3 worldPosition = nearWorld + rayDirection * t;
    if (t > pc.viewportAndFar.w) discard;

    // 真实深度（Vulkan 0~1）。
    vec4 clipPosition = pc.viewProjection * vec4(worldPosition, 1.0);
    float depth = clipPosition.z / clipPosition.w;
    if (!(depth >= 0.0 && depth <= 1.0)) discard;
    gl_FragDepth = depth;

    // 地图矩形内精确裁切（含边缘缓冲，不写死几十米空白）。
    float hasMap = pc.mapParams.x;
    if (hasMap > 0.5) {
        float worldPixel = max(length(dFdx(worldPosition.xy)), length(dFdy(worldPosition.xy)));
        float feather = max(worldPixel * 1.5, 0.05);
        float dx = abs(worldPosition.x - pc.mapParams.y);
        float dy = abs(worldPosition.y - pc.mapParams.z);
        if (dx <= pc.mapParams.w + feather && dy <= pc.mapParams2.x + feather) {
            discard;
        }
    }

    // 距离淡出：0~45% far 完整，45~75% 平滑淡出，>75% 隐藏。
    float distToCamera = length(worldPosition - pc.cameraPosition.xyz);
    float distanceFade = 1.0 - smoothstep(pc.viewportAndFar.z * 0.45,
        pc.viewportAndFar.z * 0.75, distToCamera);

    // 掠射角淡出：N=(0,0,1)，V=相机方向；<0.03 隐藏，0.03~0.12 淡入。
    vec3 viewDirection = normalize(pc.cameraPosition.xyz - worldPosition);
    float grazingFactor = abs(dot(vec3(0.0, 0.0, 1.0), viewDirection));
    float grazingFade = smoothstep(0.03, 0.12, grazingFactor);

    // X/Y 主轴（世界 Y=0 为 X 轴、世界 X=0 为 Y 轴），屏幕恒定 ~2.5px。
    float xAxis = 1.0 - smoothstep(0.0, 2.5, abs(worldPosition.y) / max(fwidth(worldPosition.y), 0.000001));
    float yAxis = 1.0 - smoothstep(0.0, 2.5, abs(worldPosition.x) / max(fwidth(worldPosition.x), 0.000001));

    // 自适应层级：worldMetersPerPixel × 20 → 相邻两级平滑交叉淡入。
    float worldMetersPerPixel = max(length(dFdx(worldPosition.xy)), length(dFdy(worldPosition.xy)));
    float desiredStep = clamp(worldMetersPerPixel * 20.0, MIN_STEP, MAX_STEP);
    float logStep = log(desiredStep) / log(10.0);
    float lowerExp = floor(logStep);
    float lowerStep = clamp(pow(10.0, lowerExp), MIN_STEP, MAX_STEP);
    float upperStep = clamp(lowerStep * 10.0, MIN_STEP, MAX_STEP);
    float transition = fract(logStep);
    float upperWeight = smoothstep(0.25, 0.75, transition);
    float lowerWeight = 1.0 - upperWeight;

    // 细格（lowerStep）1px 低透明；主格（upperStep）2px 较高透明。
    float minorLine = gridLine(worldPosition.xy, lowerStep, 1.0);
    float majorLine = gridLine(worldPosition.xy, upperStep, 2.0);
    float grid = minorLine * lowerWeight * 0.18 + majorLine * upperWeight * 0.32;

    // 主轴与原点优先于网格线。
    vec3 minorColor = vec3(0.494, 0.561, 0.631); // #7E8FA1
    vec3 majorColor = vec3(0.376, 0.455, 0.529); // #607487
    vec3 axisXColor = vec3(0.353, 0.498, 0.639); // #5A7FA3
    vec3 axisYColor = vec3(0.714, 0.545, 0.329); // #B68B54
    vec3 originColor = vec3(0.820, 0.682, 0.412); // #D1AE69

    vec3 color = mix(minorColor, majorColor, upperWeight);
    float alpha = grid * distanceFade * grazingFade;

    // 原点标记（屏幕恒定约 6px 范围）。
    float originRange = max(fwidth(worldPosition.x), fwidth(worldPosition.y)) * 3.0;
    float originMark = 1.0 - smoothstep(0.0, originRange,
        max(abs(worldPosition.x), abs(worldPosition.y)));

    if (xAxis > 0.5 || yAxis > 0.5 || originMark > 0.5) {
        float axisAlpha = max(max(xAxis, yAxis), originMark) * distanceFade * grazingFade
            * (originMark > 0.5 ? 0.85 : 0.78);
        color = originMark > 0.5 ? originColor : (xAxis > yAxis ? axisXColor : axisYColor);
        alpha = max(alpha, axisAlpha);
    }

    if (alpha < 0.005) discard;
    outColor = vec4(color, alpha);
}
