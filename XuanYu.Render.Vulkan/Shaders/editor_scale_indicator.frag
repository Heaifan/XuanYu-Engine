#version 450

layout(push_constant) uniform ScalePush
{
    vec4 viewport;   // xy = physical viewport, z = DPI, w = visible
    vec4 rect;       // overlay rect in DIP
    vec4 scale;      // x = bar width DIP, y = label length
    vec4 glyphs0;    // first four ScaleIndicatorGlyphLite codes
    vec4 glyphs1;    // final four ScaleIndicatorGlyphLite codes
} pc;

layout(location = 0) in vec2 vNdc;
layout(location = 0) out vec4 outColor;

float aa(float d, float dpi) { return max(fwidth(d), 0.65 / dpi); }

float segment(vec2 p, vec2 a, vec2 b, float width, float dpi)
{
    vec2 ab = b - a;
    float t = clamp(dot(p - a, ab) / max(dot(ab, ab), 1e-5), 0.0, 1.0);
    float d = length(p - (a + ab * t)) - width * 0.5;
    return 1.0 - smoothstep(-aa(d, dpi), aa(d, dpi), d);
}

float disc(vec2 p, vec2 center, float radius, float dpi)
{
    float d = length(p - center) - radius;
    return 1.0 - smoothstep(-aa(d, dpi), aa(d, dpi), d);
}

float box(vec2 p, vec2 center, vec2 halfSize, float radius, float dpi)
{
    vec2 q = abs(p - center) - halfSize + vec2(radius);
    float d = min(max(q.x, q.y), 0.0) + length(max(q, vec2(0.0))) - radius;
    return 1.0 - smoothstep(-aa(d, dpi), aa(d, dpi), d);
}

float digitGlyph(vec2 p, int digit, float dpi)
{
    float m = 0.0;
    float w = 0.85;
    vec2 tl = vec2(-2.2,-4.0), tr = vec2(2.2,-4.0);
    vec2 ml = vec2(-2.2,0.0), mr = vec2(2.2,0.0);
    vec2 bl = vec2(-2.2,4.0), br = vec2(2.2,4.0);
    if (digit == 0 || digit == 2 || digit == 3 || digit == 5 || digit == 6 || digit == 7 || digit == 8 || digit == 9)
        m = max(m, segment(p, tl, tr, w, dpi));
    if (digit == 0 || digit == 4 || digit == 5 || digit == 6 || digit == 8 || digit == 9)
        m = max(m, segment(p, tl, ml, w, dpi));
    if (digit == 0 || digit == 1 || digit == 2 || digit == 3 || digit == 4 || digit == 7 || digit == 8 || digit == 9)
        m = max(m, segment(p, tr, mr, w, dpi));
    if (digit == 2 || digit == 3 || digit == 4 || digit == 5 || digit == 6 || digit == 8 || digit == 9)
        m = max(m, segment(p, ml, mr, w, dpi));
    if (digit == 0 || digit == 2 || digit == 6 || digit == 8)
        m = max(m, segment(p, ml, bl, w, dpi));
    if (digit == 0 || digit == 1 || digit == 3 || digit == 4 || digit == 5 || digit == 6 || digit == 7 || digit == 8 || digit == 9)
        m = max(m, segment(p, mr, br, w, dpi));
    if (digit == 0 || digit == 2 || digit == 3 || digit == 5 || digit == 6 || digit == 8)
        m = max(m, segment(p, bl, br, w, dpi));
    return m;
}

float glyph(vec2 p, int code, float dpi)
{
    if (code >= 0 && code <= 9) return digitGlyph(p, code, dpi);
    if (code == 10)
    {
        float left = segment(p, vec2(-2.5,4.0), vec2(-2.5,-1.5), 1.0, dpi);
        float right = segment(p, vec2(2.5,4.0), vec2(2.5,-1.5), 1.0, dpi);
        float crownA = segment(p, vec2(-2.5,-1.5), vec2(0.0,0.8), 1.0, dpi);
        float crownB = segment(p, vec2(0.0,0.8), vec2(2.5,-1.5), 1.0, dpi);
        return max(max(left, right), max(crownA, crownB));
    }
    if (code == 11)
    {
        float stem = segment(p, vec2(-2.2,-4.0), vec2(-2.2,4.0), 1.0, dpi);
        float upper = segment(p, vec2(-2.0,0.5), vec2(2.3,-3.0), 1.0, dpi);
        float lower = segment(p, vec2(-1.0,-0.3), vec2(2.3,4.0), 1.0, dpi);
        return max(stem, max(upper, lower));
    }
    if (code == 12) return disc(p, vec2(0.0,3.8), 0.8, dpi);
    return 0.0;
}

int glyphCode(int index)
{
    if (index == 0) return int(pc.glyphs0.x);
    if (index == 1) return int(pc.glyphs0.y);
    if (index == 2) return int(pc.glyphs0.z);
    if (index == 3) return int(pc.glyphs0.w);
    if (index == 4) return int(pc.glyphs1.x);
    if (index == 5) return int(pc.glyphs1.y);
    if (index == 6) return int(pc.glyphs1.z);
    return int(pc.glyphs1.w);
}

void main()
{
    if (pc.viewport.w < 0.5) discard;
    float dpi = max(pc.viewport.z, 0.5);
    vec2 p = gl_FragCoord.xy / dpi - pc.rect.xy;
    if (p.x < 0.0 || p.y < 0.0 || p.x > pc.rect.z || p.y > pc.rect.w) discard;

    float outer = box(p, pc.rect.zw * 0.5, pc.rect.zw * 0.5, 3.0, dpi);
    vec2 innerHalf = pc.rect.zw * 0.5 - vec2(1.0);
    float inner = box(p, pc.rect.zw * 0.5, innerHalf, 2.0, dpi);
    float border = max(outer - inner, 0.0);
    vec3 panelColor = vec3(0.973, 0.980, 0.984);
    vec3 borderColor = vec3(0.835, 0.871, 0.894);
    vec3 textColor = vec3(0.141, 0.216, 0.267);
    vec3 accentColor = vec3(0.196, 0.435, 0.541);
    vec3 cardColor = mix(panelColor, borderColor, border);
    vec4 color = vec4(cardColor, outer * 0.96);
    float barStart = 6.0;
    float barEnd = min(barStart + pc.scale.x, pc.rect.z - 6.0);
    float barY = pc.rect.w - 8.0;
    float bar = segment(p, vec2(barStart,barY), vec2(barEnd,barY), 1.6, dpi);
    bar = max(bar, segment(p, vec2(barStart,barY-5.0), vec2(barStart,barY+1.0), 1.6, dpi));
    bar = max(bar, segment(p, vec2(barEnd,barY-5.0), vec2(barEnd,barY+1.0), 1.6, dpi));
    color = mix(color, vec4(accentColor, 1.0), bar);

    float text = 0.0;
    for (int i = 0; i < 8; ++i)
    {
        if (i >= int(pc.scale.y)) break;
        vec2 local = p - vec2(8.5 + float(i) * 7.0, 11.5);
        text = max(text, glyph(local, glyphCode(i), dpi));
    }
    color = mix(color, vec4(textColor, 1.0), text);
    if (color.a <= 0.001) discard;
    outColor = color;
}
