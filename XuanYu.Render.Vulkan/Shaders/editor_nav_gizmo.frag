#version 450

// 玄域编辑器：Blender 风格导航 Gizmo
// 保持现有 80B Push Constant、96 DIP 区域、14 DIP 边距和 CPU 命中布局。
// 视觉原则：小中心球、细轴线、前后分层、正对轴只显示一个端点、文字写在端点内部。

layout(push_constant) uniform GizmoPush
{
    vec4 cameraRight;      // xyz
    vec4 cameraUp;         // xyz
    vec4 cameraForward;    // xyz
    vec4 viewportAndDpi;   // xy = viewport px, z = DPI
    vec4 gizmoParams;      // x = size DIP, y = margin DIP, z = hover index
} pc;

layout(location = 0) in vec2 vNdc;
layout(location = 0) out vec4 outColor;

const float AXIS_RADIUS_DIP = 27.0;
const float HUB_RADIUS_DIP = 9.5;
const float FRONT_RADIUS_DIP = 7.5;
const float BACK_RADIUS_DIP = 3.8;
const float FACING_RADIUS_DIP = 8.5;
const float AXIS_WIDTH_DIP = 1.25;
const float FACING_LIMIT_DIP = 6.0;

const vec3 AXIS_COLOR[3] = vec3[3](
    vec3(0.776, 0.416, 0.369), // X #C66A5E：低饱和珊瑚红
    vec3(0.420, 0.624, 0.518), // Y #6B9F84：低饱和豆青
    vec3(0.384, 0.557, 0.761)  // Z #628EC2：钢蓝
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
    if (index == 0) return vec3( 1.0,  0.0,  0.0);
    if (index == 1) return vec3(-1.0,  0.0,  0.0);
    if (index == 2) return vec3( 0.0,  1.0,  0.0);
    if (index == 3) return vec3( 0.0, -1.0,  0.0);
    if (index == 4) return vec3( 0.0,  0.0,  1.0);
    return vec3(0.0, 0.0, -1.0);
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
    float projectedLength = length(projection) * AXIS_RADIUS_DIP;
    float depth = dot(direction, forward);
    bool facing = projectedLength < FACING_LIMIT_DIP;
    bool front = depth > 0.0;
    bool positive = (index % 2) == 0;

    Endpoint e;
    e.position = facing && front
        ? center
        : center + projection * AXIS_RADIUS_DIP;
    e.depth = depth;
    e.projectedLength = projectedLength;
    e.axis = index / 2;
    e.index = index;
    e.positive = positive;
    e.visible = !facing || front;
    e.facing = facing;

    if (facing && front)
    {
        e.radius = FACING_RADIUS_DIP;
        e.alpha = positive ? 1.0 : 0.82;
    }
    else if (front)
    {
        e.radius = positive ? FRONT_RADIUS_DIP : BACK_RADIUS_DIP + 0.8;
        e.alpha = positive ? 1.0 : 0.62;
    }
    else
    {
        e.radius = BACK_RADIUS_DIP;
        e.alpha = positive ? 0.30 : 0.22;
    }

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
    float dpi = max(pc.viewportAndDpi.z, 0.5);
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

    Endpoint endpoints[6];
    for (int i = 0; i < 6; ++i)
    {
        endpoints[i] = buildEndpoint(i, center, right, up, forward);
    }

    vec4 acc = vec4(0.0);

    // 第一层：背向轴和小端点。
    for (int i = 0; i < 6; ++i) drawAxis(acc, p, center, endpoints[i], dpi, false);
    for (int i = 0; i < 6; ++i) drawEndpoint(acc, p, endpoints[i], dpi, false);

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

    // 第三层：朝向轴、端点和端点内部标签。
    for (int i = 0; i < 6; ++i) drawAxis(acc, p, center, endpoints[i], dpi, true);
    for (int i = 0; i < 6; ++i) drawEndpoint(acc, p, endpoints[i], dpi, true);
    for (int i = 0; i < 6; ++i) drawEndpointLabel(acc, p, endpoints[i], dpi);

    // 最后一层：单一 Hover 环。
    if (hoverIndex >= 0 && hoverIndex < 6 && endpoints[hoverIndex].visible)
    {
        Endpoint hovered = endpoints[hoverIndex];
        float ring = ringMask(p, hovered.position, hovered.radius + 2.0, 1.35, dpi);
        compositeOver(acc, vec3(0.98), ring * 0.92);
    }

    if (acc.a <= 0.001) discard;

    vec3 nonPremultiplied = acc.rgb / max(acc.a, 1e-5);
    outColor = vec4(nonPremultiplied, acc.a);
}
