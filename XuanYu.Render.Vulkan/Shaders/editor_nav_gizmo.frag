#version 450

// MAP-A-R1-D5-R1-F3-F1：导航 Gizmo Overlay Pass —— 片元着色器（屏幕空间）。
// 固定视口右上角（88 DIP 区域，距右/上 12 DIP）；中心球 + 三根世界轴 + 六正负端点 + X/Y/Z 标签。
// 投影：screenX = dot(d, Right)；screenY = -dot(d, Up)；depth = dot(d, Forward)。
// 深度排序：背向（depth 小）先画、朝向（depth 大）后画；背向端点 40% Alpha 小点。
// 玄域低饱和配色：X #C18A55 淡金褐 / Y #5F87A7 蓝灰 / Z #A9B8C7 浅钢灰；中心球 #CDD6DF。

layout(push_constant) uniform GizmoPush {
    vec4 cameraRight;      // 0   相机 Right（投影 X）
    vec4 cameraUp;         // 16  相机 Up（投影 Y，屏幕向下取负）
    vec4 cameraForward;    // 32  相机 Forward（深度）
    vec4 viewportAndDpi;   // 48  xy=视口尺寸(px); z=DPI scale; w=未使用
    vec4 gizmoParams;      // 64  x=区域尺寸 DIP(88); y=边距 DIP(12); z=悬停索引(-1 无); w=未使用
} pc;

layout(location = 0) in vec2 vNdc;
layout(location = 0) out vec4 outColor;

const float AXIS_RADIUS = 25.0;    // 轴线投影半径（DIP）
const float CENTER_RADIUS = 13.0;  // 中心球半径
const float POS_RADIUS = 9.0;      // 正方向端点半径
const float NEG_RADIUS = 5.5;      // 负方向端点半径
const float AXIS_WIDTH = 1.5;      // 轴线宽度

struct Endpoint { vec2 screen; float depth; int axis; bool positive; float alpha; float radius; int index; };
const vec3 AXIS_COLORS[3] = vec3[3](vec3(0.757, 0.541, 0.333), // X #C18A55
                                    vec3(0.373, 0.529, 0.655), // Y #5F87A7
                                    vec3(0.663, 0.722, 0.780)); // Z #A9B8C7

const vec3 DIRECTIONS[6] = vec3[6](vec3(1,0,0), vec3(-1,0,0), vec3(0,1,0), vec3(0,-1,0), vec3(0,0,1), vec3(0,0,-1));

float distToSegment(vec2 p, vec2 a, vec2 b) {
    vec2 ab = b - a;
    float t = clamp(dot(p - a, ab) / max(dot(ab, ab), 1e-6), 0.0, 1.0);
    return length(p - (a + ab * t));
}

// 端点字母 X/Y/Z（8 DIP 高，白色）——点在端点右下方。
float letterMask(vec2 px, vec2 at, int axis) {
    vec2 c = at + vec2(7.0, 7.0); // 字母中心相对端点偏移
    float s = 4.0; // 半尺寸
    if (axis == 0) { // X：两条对角线
        float d1 = distToSegment(px, c + vec2(-s,-s), c + vec2(s,s));
        float d2 = distToSegment(px, c + vec2(-s,s), c + vec2(s,-s));
        return min(d1, d2) < 1.2 ? 1.0 : 0.0;
    }
    if (axis == 1) { // Y：竖线下半 + 两斜线
        float d1 = distToSegment(px, c + vec2(0,-s), c + vec2(0,0));
        float d2 = distToSegment(px, c + vec2(-s,-s), c + vec2(0,0));
        float d3 = distToSegment(px, c + vec2(s,-s), c + vec2(0,0));
        return min(min(d1, d2), d3) < 1.2 ? 1.0 : 0.0;
    }
    // Z：上横 + 下横 + 对角线
    float d1 = distToSegment(px, c + vec2(-s,-s), c + vec2(s,-s));
    float d2 = distToSegment(px, c + vec2(-s,s), c + vec2(s,s));
    float d3 = distToSegment(px, c + vec2(s,-s), c + vec2(-s,s));
    return min(min(d1, d2), d3) < 1.2 ? 1.0 : 0.0;
}

Endpoint makeEndpoint(int i, vec2 gizmoCenter, float dpi) {
    vec3 d = DIRECTIONS[i];
    float sx = dot(d, pc.cameraRight.xyz);
    float sy = -dot(d, pc.cameraUp.xyz);
    float depth = dot(d, pc.cameraForward.xyz);
    bool positive = i % 2 == 0;
    Endpoint e;
    e.screen = gizmoCenter + (vec2(sx, sy) * AXIS_RADIUS * dpi);
    e.depth = depth;
    e.axis = i / 2;
    e.positive = positive;
    float alpha;
    if (depth < -0.35) alpha = 0.40;
    else if (depth < 0.35) alpha = 0.78;
    else alpha = 1.0;
    e.alpha = alpha;
    e.radius = (positive ? POS_RADIUS : NEG_RADIUS) * dpi;
    e.index = i;
    return e;
}

// 简单插入排序（按 depth 升序：背向先绘制）。
void sortEndpoints(inout Endpoint e[6]) {
    for (int i = 1; i < 6; i++) {
        Endpoint key = e[i];
        int j = i - 1;
        while (j >= 0 && e[j].depth > key.depth) { e[j + 1] = e[j]; j--; }
        e[j + 1] = key;
    }
}

void main() {
    vec2 viewport = pc.viewportAndDpi.xy;
    float dpi = pc.viewportAndDpi.z;
    float sizeDips = pc.gizmoParams.x;
    float marginDips = pc.gizmoParams.y;
    int hoverIndex = int(pc.gizmoParams.z);

    vec2 gizmoCenter = vec2(viewport.x - (marginDips + sizeDips * 0.5) * dpi,
                            (marginDips + sizeDips * 0.5) * dpi);
    vec2 px = gl_FragCoord.xy;

    Endpoint eps[6];
    for (int i = 0; i < 6; i++) eps[i] = makeEndpoint(i, gizmoCenter, dpi);
    sortEndpoints(eps);

    // 1) 轴线（中心 → 端点，全部端点）。
    for (int i = 0; i < 6; i++) {
        float d = distToSegment(px, gizmoCenter, eps[i].screen);
        if (d < AXIS_WIDTH * 0.5 * dpi) {
            vec3 color = AXIS_COLORS[eps[i].axis];
            float alpha = eps[i].alpha * 0.9;
            outColor = vec4(color, alpha);
            return;
        }
    }
    // 2) 中心球（在轴线之上）。
    float cd = length(px - gizmoCenter);
    if (cd < CENTER_RADIUS * dpi) {
        vec3 fill = vec3(0.804, 0.839, 0.875); // #CDD6DF
        float rim = abs(cd - CENTER_RADIUS * dpi) < 1.0 * dpi ? 0.35 : 0.0;
        outColor = vec4(mix(fill, vec3(0.443, 0.502, 0.588), rim), 0.95);
        return;
    }
    // 3) 端点与标签（按深度升序：背向先画、朝向后画；重叠时朝向优先）。
    for (int i = 0; i < 6; i++) {
        Endpoint e = eps[i];
        float d = length(px - e.screen);
        if (d < e.radius) {
            vec3 color = AXIS_COLORS[e.axis];
            float alpha = e.alpha;
            // 悬停：端点提高亮度并加亮环。
            if (e.index == hoverIndex) {
                color = color * 1.15;
                alpha = min(1.0, alpha + 0.15);
            }
            outColor = vec4(color, alpha);
            return;
        }
        if (e.positive) {
            float lm = letterMask(px, e.screen, e.axis);
            if (lm > 0.5) { outColor = vec4(1.0, 1.0, 1.0, e.alpha); return; }
        }
    }
    discard;
}
