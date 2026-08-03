#version 450

// MAP-A-R1-D5-R1-F3-F3：导航 Gizmo Overlay Pass —— 片元着色器（屏幕空间，Blender 结构）。
// 分层绘制：后向轴/端点 → 中心球 → 前向轴/端点（正对端点位于中心，覆盖球）→ 标签 → Hover 环。
// 轴正对相机（投影长度 < 6 DIP）时隐藏背向端点与轴线，只显示朝向端点（置于中心球中央）。
// 轴线从中心球边缘开始（不穿过球）；标签仅正方向且朝向相机时显示（11 DIP 半粗）。
// 玄域低饱和配色：X #C4874F / Y #5684A8 / Z #8EA8C2；球 #D7DEE6、描边 #66788B；背向 30% Alpha。

layout(push_constant) uniform GizmoPush {
    vec4 cameraRight;      // 0   相机 Right（投影 X）
    vec4 cameraUp;         // 16  相机 Up（投影 Y，屏幕向下取负）
    vec4 cameraForward;    // 32  相机 Forward（深度）
    vec4 viewportAndDpi;   // 48  xy=视口尺寸(px); z=DPI scale
    vec4 gizmoParams;      // 64  x=区域尺寸 DIP(96); y=边距 DIP(14); z=悬停索引(-1 无)
} pc;

layout(location = 0) in vec2 vNdc;
layout(location = 0) out vec4 outColor;

const float AXIS_RADIUS = 27.0;    // 轴线投影半径（DIP）
const float CENTER_RADIUS = 13.0;  // 中心球半径
const float POS_RADIUS = 9.0;      // 正方向端点半径
const float NEG_RADIUS = 5.0;      // 负方向端点半径
const float AXIS_WIDTH = 1.5;      // 轴线宽度
const float FACING_LIMIT = 6.0;    // 轴正对相机判定（屏幕投影长度，DIP）

const vec3 AXIS_COLORS[3] = vec3[3](vec3(0.769, 0.529, 0.310), // X #C4874F
                                    vec3(0.337, 0.518, 0.659), // Y #5684A8
                                    vec3(0.557, 0.659, 0.761)); // Z #8EA8C2
const vec3 BALL_FILL = vec3(0.843, 0.871, 0.902);  // 中心球 #D7DEE6
const vec3 BALL_RIM = vec3(0.400, 0.471, 0.545);   // 中心球描边 #66788B

const vec3 DIRECTIONS[6] = vec3[6](vec3(1,0,0), vec3(-1,0,0), vec3(0,1,0), vec3(0,-1,0), vec3(0,0,1), vec3(0,0,-1));

struct Endpoint {
    vec2 screen; float depth; int axis; bool positive;
    float alpha; float radius; int index;
    float projLen; bool visible;
};

float distToSegment(vec2 p, vec2 a, vec2 b) {
    vec2 ab = b - a;
    float t = clamp(dot(p - a, ab) / max(dot(ab, ab), 1e-6), 0.0, 1.0);
    return length(p - (a + ab * t));
}

// 端点字母 X/Y/Z（11 DIP 半粗，白色）——点在端点右下方。
float letterMask(vec2 px, vec2 at, int axis) {
    vec2 c = at + vec2(8.0, 8.0);
    float s = 5.5;
    if (axis == 0) {
        float d1 = distToSegment(px, c + vec2(-s,-s), c + vec2(s,s));
        float d2 = distToSegment(px, c + vec2(-s,s), c + vec2(s,-s));
        return min(d1, d2) < 1.4 ? 1.0 : 0.0;
    }
    if (axis == 1) {
        float d1 = distToSegment(px, c + vec2(0,-s), c + vec2(0,0));
        float d2 = distToSegment(px, c + vec2(-s,-s), c + vec2(0,0));
        float d3 = distToSegment(px, c + vec2(s,-s), c + vec2(0,0));
        return min(min(d1, d2), d3) < 1.4 ? 1.0 : 0.0;
    }
    float d1 = distToSegment(px, c + vec2(-s,-s), c + vec2(s,-s));
    float d2 = distToSegment(px, c + vec2(-s,s), c + vec2(s,s));
    float d3 = distToSegment(px, c + vec2(s,-s), c + vec2(-s,s));
    return min(min(d1, d2), d3) < 1.4 ? 1.0 : 0.0;
}

Endpoint makeEndpoint(int i, vec2 gizmoCenter, float dpi) {
    vec3 d = DIRECTIONS[i];
    float sx = dot(d, pc.cameraRight.xyz);
    float sy = -dot(d, pc.cameraUp.xyz);
    float depth = dot(d, pc.cameraForward.xyz);
    float projLen = length(vec2(sx, sy)) * AXIS_RADIUS;
    bool facingCamera = projLen < FACING_LIMIT;
    bool positive = i % 2 == 0;
    Endpoint e;
    e.screen = facingCamera ? gizmoCenter : gizmoCenter + (vec2(sx, sy) * AXIS_RADIUS * dpi);
    e.depth = depth;
    e.axis = i / 2;
    e.positive = positive;
    float alpha;
    if (depth < -0.35) alpha = 0.30;
    else if (depth < 0.35) alpha = 0.78;
    else alpha = 1.0;
    e.alpha = alpha;
    e.radius = (positive ? POS_RADIUS : NEG_RADIUS) * dpi;
    e.index = i;
    e.projLen = projLen;
    e.visible = !facingCamera || depth > 0.0;
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

// 轴线遮罩：从中心球边缘到端点（不穿过球）；仅可见且非正对的轴。
float axisMask(vec2 px, Endpoint e, vec2 gizmoCenter, float dpi) {
    if (!e.visible || e.projLen < FACING_LIMIT) return 0.0;
    vec2 dir = e.screen - gizmoCenter;
    float len = length(dir);
    if (len < 1e-6) return 0.0;
    vec2 start = gizmoCenter + (dir / len) * (CENTER_RADIUS * dpi);
    float d = distToSegment(px, start, e.screen);
    return d < AXIS_WIDTH * 0.5 * dpi ? 1.0 : 0.0;
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

    // 层1：后向轴线与后向端点（depth<0；按深度正序：更背向先画）。
    for (int i = 0; i < 6; i++) {
        Endpoint e = eps[i];
        if (e.depth >= 0.0) break;
        if (axisMask(px, e, gizmoCenter, dpi) > 0.5) { outColor = vec4(AXIS_COLORS[e.axis], e.alpha * 0.9); return; }
    }
    for (int i = 0; i < 6; i++) {
        Endpoint e = eps[i];
        if (e.depth >= 0.0) break;
        if (!e.visible || length(px - e.screen) >= e.radius) continue;
        outColor = vec4(AXIS_COLORS[e.axis], e.alpha);
        return;
    }

    // 层2：中心球（浅灰填充 + 蓝灰描边），遮挡背向轴与端点。
    float cd = length(px - gizmoCenter);
    if (cd < CENTER_RADIUS * dpi) {
        float rim = abs(cd - CENTER_RADIUS * dpi) < 1.2 * dpi ? 1.0 : 0.0;
        outColor = vec4(mix(BALL_FILL, BALL_RIM, rim), 0.97);
        return;
    }

    // 层3：前向轴线与前向端点（depth>=0；按深度倒序：最朝前后画，正对端点覆盖球）。
    for (int i = 5; i >= 0; i--) {
        Endpoint e = eps[i];
        if (e.depth < 0.0) continue;
        if (axisMask(px, e, gizmoCenter, dpi) > 0.5) { outColor = vec4(AXIS_COLORS[e.axis], e.alpha * 0.9); return; }
    }
    for (int i = 5; i >= 0; i--) {
        Endpoint e = eps[i];
        if (e.depth < 0.0) continue;
        if (!e.visible || length(px - e.screen) >= e.radius) continue;
        vec3 color = AXIS_COLORS[e.axis];
        float alpha = e.alpha;
        if (e.index == hoverIndex) { color = color * 1.15; alpha = min(1.0, alpha + 0.15); }
        outColor = vec4(color, alpha);
        return;
    }

    // 层4：标签——仅正方向且朝向相机（depth>0）的端点。
    for (int i = 5; i >= 0; i--) {
        Endpoint e = eps[i];
        if (!e.positive || e.depth <= 0.0 || !e.visible) continue;
        if (letterMask(px, e.screen, e.axis) > 0.5) { outColor = vec4(1.0, 1.0, 1.0, e.alpha); return; }
    }

    // 层5：Hover 环（端点外圈亮环）。
    if (hoverIndex >= 0) {
        for (int i = 0; i < 6; i++) {
            Endpoint e = eps[i];
            if (e.index != hoverIndex || !e.visible) continue;
            float hd = length(px - e.screen);
            if (abs(hd - (e.radius + 2.0 * dpi)) < 1.2 * dpi) { outColor = vec4(1.0, 1.0, 1.0, 0.85); return; }
        }
    }

    discard;
}
