#version 450

// 玄域编辑器：Blender 风格导航 Gizmo
// A 版冻结：96 DIP 清透底板、14 DIP 右上边距、三轴端点和独立交互态。

layout(push_constant) uniform GizmoPush
{
    vec4 cameraRight;      // xyz
    vec4 cameraUp;         // xyz
    vec4 cameraForward;    // xyz
    vec4 viewportAndDpi;   // xy = viewport px, z = DPI
    vec4 gizmoParams;      // x = size DIP, y = margin DIP, z = hover index, w = RenderScaling
    vec4 interactionParams; // x = active; y = pressed endpoint index (ACTIVE_INDEX)
} pc;

layout(location = 0) in vec2 vNdc;
layout(location = 0) out vec4 outColor;

const float AXIS_RADIUS_DIP = 24.0;
const float HUB_RADIUS_DIP = 14.0;
const float ENDPOINT_RADIUS_DIP = 9.0;
const float HOVER_RADIUS_DIP = 10.5;
const float PRESSED_RADIUS_DIP = 11.0;
const float AXIS_WIDTH_DIP = 3.0;
const float PANEL_RADIUS_DIP = 16.0;

const vec3 AXIS_COLOR[3] = vec3[3](
    vec3(0.929, 0.420, 0.420), // X #D65252
    vec3(0.310, 0.682, 0.447), // Y #4FAE72
    vec3(0.435, 0.643, 0.941)  // Z #6FA4F0
);

const vec3 HUB_LIGHT = vec3(0.925, 0.945, 0.965);
const vec3 HUB_DARK = vec3(0.720, 0.765, 0.810);
const vec3 HUB_RIM = vec3(0.310, 0.380, 0.455);
const vec3 LABEL_COLOR = vec3(0.985);
const vec3 LABEL_SHADOW = vec3(0.120, 0.160, 0.200);

struct Endpoint
{
    vec2 position;
    float depth;
    float projectedLength;
    float radius;
    float alpha;
    int axis;
    int index;
    bool positive;
    bool visible;
    bool facing;
};

float aaWidth(float distanceValue, float dpi)
{
    return max(fwidth(distanceValue), 0.65 / dpi);
}

float circleMask(vec2 p, vec2 center, float radius, float dpi)
{
    float d = length(p - center) - radius;
    float aa = aaWidth(d, dpi);
    return 1.0 - smoothstep(-aa, aa, d);
}

float ringMask(vec2 p, vec2 center, float radius, float width, float dpi)
{
    float d = abs(length(p - center) - radius) - width * 0.5;
    float aa = aaWidth(d, dpi);
    return 1.0 - smoothstep(-aa, aa, d);
}

float roundedPanel(vec2 p, vec2 center, vec2 halfSize, float radius, float dpi)
{
    vec2 q = abs(p - center) - halfSize + vec2(radius);
    float d = length(max(q, 0.0)) + min(max(q.x, q.y), 0.0) - radius;
    float aa = aaWidth(d, dpi);
    return 1.0 - smoothstep(-aa, aa, d);
}

float segmentMask(vec2 p, vec2 a, vec2 b, float width, float dpi)
{
    vec2 ab = b - a;
    float denom = max(dot(ab, ab), 1e-6);
    float t = clamp(dot(p - a, ab) / denom, 0.0, 1.0);
    float d = length(p - (a + ab * t)) - width * 0.5;
    float aa = aaWidth(d, dpi);
    return 1.0 - smoothstep(-aa, aa, d);
}

// acc.rgb 使用预乘色；最终输出前恢复为非预乘，适配现有 SrcAlpha 混合管线。
void compositeOver(inout vec4 acc, vec3 color, float alpha)
{
    alpha = clamp(alpha, 0.0, 1.0);
    acc.rgb = color * alpha + acc.rgb * (1.0 - alpha);
    acc.a = alpha + acc.a * (1.0 - alpha);
}

vec3 axisDirection(int index)
{
    if (index == 0) return vec3(1.0, 0.0, 0.0);
    if (index == 1) return vec3(0.0, 1.0, 0.0);
    return vec3(0.0, 0.0, 1.0);
}

Endpoint buildEndpoint(
    int index,
    vec2 center,
    vec3 right,
    vec3 up,
    vec3 forward)
{
    vec3 direction = axisDirection(index);
    vec2 projection = vec2(dot(direction, right), -dot(direction, up));
    if (length(projection) < 0.2)
    {
        projection = index == 0 ? vec2(1.0, 0.0) :
            (index == 1 ? vec2(-0.75, 0.66) : vec2(0.0, -1.0));
    }
    float projectedLength = length(projection) * AXIS_RADIUS_DIP;
    float depth = dot(direction, forward);
    bool facing = false;
    bool front = depth > 0.0;
    bool positive = true;

    Endpoint e;
    e.position = facing && front
        ? center
        : center + projection * AXIS_RADIUS_DIP;
    e.depth = depth;
    e.projectedLength = projectedLength;
    e.axis = index;
    e.index = index;
    e.positive = positive;
    e.visible = true;
    e.facing = facing;

    e.radius = ENDPOINT_RADIUS_DIP;
    e.alpha = 1.0;

    return e;
}

float glyphX(vec2 p, vec2 center, float dpi)
{
    float s = 3.0;
    float a = segmentMask(p, center + vec2(-s, -s), center + vec2(s, s), 1.15, dpi);
    float b = segmentMask(p, center + vec2(-s, s), center + vec2(s, -s), 1.15, dpi);
    return max(a, b);
}

float glyphY(vec2 p, vec2 center, float dpi)
{
    float s = 3.2;
    float a = segmentMask(p, center + vec2(-s, -s), center, 1.15, dpi);
    float b = segmentMask(p, center + vec2(s, -s), center, 1.15, dpi);
    float c = segmentMask(p, center, center + vec2(0.0, s), 1.15, dpi);
    return max(max(a, b), c);
}

float glyphZ(vec2 p, vec2 center, float dpi)
{
    float sx = 3.0;
    float sy = 3.0;
    float a = segmentMask(p, center + vec2(-sx, -sy), center + vec2(sx, -sy), 1.15, dpi);
    float b = segmentMask(p, center + vec2(sx, -sy), center + vec2(-sx, sy), 1.15, dpi);
    float c = segmentMask(p, center + vec2(-sx, sy), center + vec2(sx, sy), 1.15, dpi);
    return max(max(a, b), c);
}

float glyphMask(vec2 p, vec2 center, int axis, float dpi)
{
    if (axis == 0) return glyphX(p, center, dpi);
    if (axis == 1) return glyphY(p, center, dpi);
    return glyphZ(p, center, dpi);
}

void drawAxis(
    inout vec4 acc,
    vec2 p,
    vec2 center,
    Endpoint e,
    float dpi,
    bool frontPass)
{
    if (!e.visible || e.facing) return;
    bool isFront = e.depth > 0.0;
    if (isFront != frontPass) return;

    vec2 delta = e.position - center;
    float len = length(delta);
    if (len < 1e-4) return;

    vec2 dir = delta / len;
    float startRadius = HUB_RADIUS_DIP + 1.6;
    float endRadius = e.radius + 1.4;
    vec2 a = center + dir * startRadius;
    vec2 b = e.position - dir * endRadius;

    float line = segmentMask(p, a, b, AXIS_WIDTH_DIP, dpi);
    float alpha = isFront ? 0.78 : 0.20;
    compositeOver(acc, AXIS_COLOR[e.axis], line * alpha);
}

void drawEndpoint(
    inout vec4 acc,
    vec2 p,
    Endpoint e,
    float dpi,
    bool frontPass)
{
    if (!e.visible) return;
    bool isFront = e.depth > 0.0;
    if (isFront != frontPass) return;

    vec3 color = AXIS_COLOR[e.axis];
    if (!e.positive && !e.facing)
    {
        color = mix(vec3(0.58, 0.62, 0.66), color, 0.45);
    }

    float disc = circleMask(p, e.position, e.radius, dpi);
    compositeOver(acc, color, disc * e.alpha);

    float innerRim = ringMask(p, e.position, e.radius - 0.45, 0.8, dpi);
    compositeOver(acc, mix(color, vec3(0.14), 0.36), innerRim * e.alpha * 0.55);
}

void drawEndpointLabel(
    inout vec4 acc,
    vec2 p,
    Endpoint e,
    float dpi)
{
    if (!e.visible || e.depth <= 0.0) return;

    // Blender 式规则：
    // 1. 正方向端点显示 X/Y/Z；
    // 2. 正对相机时，即使是负方向，也显示轴字母，避免中心只剩无意义灰点。
    bool showLabel = e.positive || e.facing;
    if (!showLabel) return;

    float shadow = glyphMask(p, e.position + vec2(0.65, 0.75), e.axis, dpi);
    float glyph = glyphMask(p, e.position, e.axis, dpi);
    compositeOver(acc, LABEL_SHADOW, shadow * 0.52);
    compositeOver(acc, LABEL_COLOR, glyph * 0.96);
}

void main()
{
    float dpi = max(pc.gizmoParams.w, 0.5);
    vec2 viewportPx = pc.viewportAndDpi.xy;
    float sizeDip = pc.gizmoParams.x;
    float marginDip = pc.gizmoParams.y;
    int hoverIndex = int(round(pc.gizmoParams.z));

    float sizePx = sizeDip * dpi;
    float marginPx = marginDip * dpi;
    vec2 topLeftPx = vec2(
        viewportPx.x - marginPx - sizePx,
        marginPx);

    // Vulkan Fragment 坐标按左上原点使用；转换为 Gizmo 内部 DIP。
    vec2 p = (gl_FragCoord.xy - topLeftPx) / dpi;
    if (p.x < 0.0 || p.y < 0.0 || p.x > sizeDip || p.y > sizeDip)
    {
        discard;
    }

    vec2 center = vec2(sizeDip * 0.5);
    vec3 right = normalize(pc.cameraRight.xyz);
    vec3 up = normalize(pc.cameraUp.xyz);
    vec3 forward = normalize(pc.cameraForward.xyz);

    Endpoint endpoints[3];
    for (int i = 0; i < 3; ++i)
    {
        endpoints[i] = buildEndpoint(i, center, right, up, forward);
    }

    vec4 acc = vec4(0.0);
    float panel = roundedPanel(p, vec2(sizeDip * 0.5), vec2(sizeDip * 0.5), PANEL_RADIUS_DIP, dpi);
    float shadow = roundedPanel(p + vec2(0.0, -2.0), vec2(sizeDip * 0.5),
        vec2(sizeDip * 0.5), PANEL_RADIUS_DIP, dpi);
    compositeOver(acc, vec3(0.14, 0.19, 0.23), (shadow - panel) * 0.18);
    compositeOver(acc, vec3(0.96, 0.98, 0.99), panel * 0.90);

    // 第一层：背向轴和小端点。
    for (int i = 0; i < 3; ++i) drawAxis(acc, p, center, endpoints[i], dpi, false);
    for (int i = 0; i < 3; ++i) drawEndpoint(acc, p, endpoints[i], dpi, false);

    // 第二层：小型中心球。使用轻微径向和左上高光，不再画成大白圆盘。
    float hub = circleMask(p, center, HUB_RADIUS_DIP, dpi);
    float hubDistance = clamp(length(p - center) / HUB_RADIUS_DIP, 0.0, 1.0);
    vec3 hubColor = mix(HUB_LIGHT, HUB_DARK, smoothstep(0.18, 1.0, hubDistance));
    compositeOver(acc, hubColor, hub * 0.96);

    float hubRim = ringMask(p, center, HUB_RADIUS_DIP, 1.1, dpi);
    compositeOver(acc, HUB_RIM, hubRim * 0.88);

    float highlight = circleMask(
        p,
        center + vec2(-2.8, -3.1),
        2.15,
        dpi);
    compositeOver(acc, vec3(1.0), highlight * 0.20);
    float centerDot = circleMask(p, center, 4.0, dpi);
    compositeOver(acc, vec3(0.54, 0.63, 0.68), centerDot * 0.92);

    // 第三层：朝向轴、端点和端点内部标签。
    for (int i = 0; i < 3; ++i) drawAxis(acc, p, center, endpoints[i], dpi, true);
    for (int i = 0; i < 3; ++i) drawEndpoint(acc, p, endpoints[i], dpi, true);
    for (int i = 0; i < 3; ++i) drawEndpointLabel(acc, p, endpoints[i], dpi);

    // 最后一层：单一 Hover 环。
    if (hoverIndex >= 0 && hoverIndex < 3 && endpoints[hoverIndex].visible)
    {
        Endpoint hovered = endpoints[hoverIndex];
        float ring = ringMask(p, hovered.position, HOVER_RADIUS_DIP, 1.35, dpi);
        compositeOver(acc, vec3(0.98), ring * 0.92);
    }

    int activeIndex = int(round(pc.interactionParams.x));
    if (activeIndex >= 0 && activeIndex < 3 && endpoints[activeIndex].visible)
    {
        float ring = ringMask(p, endpoints[activeIndex].position, ENDPOINT_RADIUS_DIP + 2.4, 1.15, dpi);
        compositeOver(acc, vec3(1.0, 0.92, 0.62), ring * 0.78);
    }

    int pressedIndex = int(round(pc.interactionParams.y));
    if (pressedIndex >= 0 && pressedIndex < 3 && endpoints[pressedIndex].visible)
    {
        float ring = ringMask(p, endpoints[pressedIndex].position, PRESSED_RADIUS_DIP, 1.8, dpi);
        compositeOver(acc, vec3(0.72, 0.22, 0.22), ring * 0.95);
    }

    if (acc.a <= 0.001) discard;

    vec3 nonPremultiplied = acc.rgb / max(acc.a, 1e-5);
    outColor = vec4(nonPremultiplied, acc.a);
}
