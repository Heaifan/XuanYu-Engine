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

int digitSegments(int digit)
{
    if (digit == 0) return 119;
    if (digit == 1) return 36;
    if (digit == 2) return 93;
    if (digit == 3) return 109;
    if (digit == 4) return 46;
    if (digit == 5) return 107;
    if (digit == 6) return 123;
    if (digit == 7) return 37;
    if (digit == 8) return 127;
    return 111;
}

float sevenSegment(vec2 p, int bits, float dpi)
{
    float m = 0.0;
    if ((bits & 1) != 0) m = max(m, segment(p, vec2(-2.2,-4.0), vec2(2.2,-4.0), 1.0, dpi));
    if ((bits & 2) != 0) m = max(m, segment(p, vec2(-2.2,-4.0), vec2(-2.2,0.0), 1.0, dpi));
    if ((bits & 4) != 0) m = max(m, segment(p, vec2(2.2,-4.0), vec2(2.2,0.0), 1.0, dpi));
    if ((bits & 8) != 0) m = max(m, segment(p, vec2(-2.2,0.0), vec2(2.2,0.0), 1.0, dpi));
    if ((bits & 16) != 0) m = max(m, segment(p, vec2(-2.2,0.0), vec2(-2.2,4.0), 1.0, dpi));
    if ((bits & 32) != 0) m = max(m, segment(p, vec2(2.2,0.0), vec2(2.2,4.0), 1.0, dpi));
    if ((bits & 64) != 0) m = max(m, segment(p, vec2(-2.2,4.0), vec2(2.2,4.0), 1.0, dpi));
    return m;
}

float glyph(vec2 p, int code, float dpi)
{
    if (code >= 0 && code <= 9) return sevenSegment(p, digitSegments(code), dpi);
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

    float background = box(p, pc.rect.zw * 0.5, pc.rect.zw * 0.5, 5.0, dpi);
    vec4 color = vec4(0.055, 0.075, 0.095, background * 0.86);
    float barStart = 6.0;
    float barEnd = min(barStart + pc.scale.x, pc.rect.z - 6.0);
    float barY = pc.rect.w - 8.0;
    float bar = segment(p, vec2(barStart,barY), vec2(barEnd,barY), 1.6, dpi);
    bar = max(bar, segment(p, vec2(barStart,barY-5.0), vec2(barStart,barY+1.0), 1.6, dpi));
    bar = max(bar, segment(p, vec2(barEnd,barY-5.0), vec2(barEnd,barY+1.0), 1.6, dpi));
    color = mix(color, vec4(0.93,0.95,0.98,1.0), bar);

    float text = 0.0;
    for (int i = 0; i < 8; ++i)
    {
        if (i >= int(pc.scale.y)) break;
        vec2 local = p - vec2(8.5 + float(i) * 7.0, 11.5);
        text = max(text, glyph(local, glyphCode(i), dpi));
    }
    color = mix(color, vec4(0.96,0.97,0.99,1.0), text);
    if (color.a <= 0.001) discard;
    outColor = color;
}
