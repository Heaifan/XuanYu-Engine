#version 450

// MAP-A-R1-D5-R1-F3-F1：世界原点标记独立全屏 Pass —— 片元着色器（屏幕空间版）。
// 不再投影到 Z=0 地面（旧版低角度透视被压扁成黄色面片）；
// 改为：世界原点 (0,0,0) 投影到屏幕中心，画恒定尺寸的细十字线 + 小空心圆 + 中心点。
// 蓝灰描边 #718096，中心淡金褐小点 #C18A55；开关独立于 ShowGrid/ShowWorldAxes。

layout(push_constant) uniform GridPush {
    mat4 viewProjection;        // 0   世界→裁剪
    mat4 inverseViewProjection; // 64  未使用（保留布局）
    vec4 cameraPosition;        // 128 未使用
    vec4 viewportAndFar;        // 144 x,y=视口尺寸(px); z,w 未使用
} pc;

layout(location = 0) out vec4 outColor;

const float CROSS_HALF_LEN = 8.0;   // 十字线半长（像素）
const float CROSS_HALF_WID = 1.1;   // 十字线半宽
const float RING_RADIUS = 5.0;      // 空心圆半径
const float RING_HALF_WID = 1.2;    // 圆环半宽
const float DOT_RADIUS = 1.8;       // 中心点半径

vec3 BLUE_GRAY = vec3(0.443, 0.502, 0.588);   // #718096
vec3 CENTER_GOLD = vec3(0.757, 0.541, 0.333); // #C18A55

void main() {
    vec4 clip = pc.viewProjection * vec4(0.0, 0.0, 0.0, 1.0);
    if (clip.w <= 0.0) discard; // 原点在相机后方
    vec2 ndc = clip.xy / clip.w;
    if (abs(ndc.x) > 1.0 || abs(ndc.y) > 1.0) discard; // 屏幕外
    vec2 screenCenter = (ndc * 0.5 + 0.5) * pc.viewportAndFar.xy;

    vec2 px = gl_FragCoord.xy;
    vec2 d = px - screenCenter;
    float dist = length(d);
    float ax = abs(d.x);
    float ay = abs(d.y);

    // 深度保持原点平面深度（实体更近则自然遮挡标记）。
    float depth = clip.z / clip.w;
    float bias = clamp(fwidth(depth) * 0.5, 0.0000001, 0.00002);
    gl_FragDepth = depth - bias;

    // 十字线：竖线 |dx|<w 且 |dy|<len；横线 |dy|<w 且 |dx|<len。
    float cross = (ax < CROSS_HALF_WID && ay < CROSS_HALF_LEN)
        || (ay < CROSS_HALF_WID && ax < CROSS_HALF_LEN) ? 1.0 : 0.0;
    // 空心圆环。
    float ring = abs(dist - RING_RADIUS) < RING_HALF_WID ? 1.0 : 0.0;
    // 中心淡金褐点。
    float dotMask = dist < DOT_RADIUS ? 1.0 : 0.0;

    vec3 color = mix(BLUE_GRAY, CENTER_GOLD, dotMask);
    float alpha = max(max(cross, ring), dotMask) * 0.85;
    if (alpha < 0.01) discard;
    outColor = vec4(color, alpha);
}
