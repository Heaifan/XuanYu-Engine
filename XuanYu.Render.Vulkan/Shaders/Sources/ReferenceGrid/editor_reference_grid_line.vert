#version 450

// GRID-RW-1-CORR2：由 gl_VertexIndex 生成相机吸附固定世界网格线。
// Major/Minor：世界坐标是 10×Step 整数倍的线为 Major；连续远距/掠射 Fade。
layout(push_constant) uniform GridPush {
    mat4 viewProjection;
    mat4 inverseViewProjection;
    vec4 cameraPosition;
    vec4 viewportAndFar; // xy=视口, z=Far, w=GridMaxDist
    vec4 gridState;      // x=Step, y=AnchorX, z=AnchorY, w=BaseHeight
} pc;

layout(location = 0) out float vMajor;
layout(location = 1) out float vFade;

const int LINES_PER_AXIS = 513;
const int HALF_LINE_COUNT = 256;

void main() {
    int lineIndex = gl_VertexIndex / 2;
    int offset = (lineIndex % LINES_PER_AXIS) - HALF_LINE_COUNT;
    bool xAxis = lineIndex < LINES_PER_AXIS;
    float coordinate = float(offset) * pc.gridState.x;
    float endpoint = (gl_VertexIndex % 2 == 0 ? -1.0 : 1.0) *
        float(HALF_LINE_COUNT) * pc.gridState.x;
    vec3 world = xAxis
        ? vec3(pc.gridState.y + coordinate, pc.gridState.z + endpoint, pc.gridState.w)
        : vec3(pc.gridState.y + endpoint, pc.gridState.z + coordinate, pc.gridState.w);

    // Major：世界坐标是 10×Step 整数倍（anchor 吸附于 Step，格号 anchorIdx+offset）。
    float anchorIdx = round((xAxis ? pc.gridState.y : pc.gridState.z) / pc.gridState.x);
    float n10 = mod(anchorIdx + float(offset), 10.0);
    vMajor = (n10 < 0.5 || n10 > 9.5) ? 1.0 : 0.0;

    // 连续远距/掠射淡出：Minor 提前淡、Major 保持更远，地平线附近连续归零。
    // 禁止 band-pass / local LOD / 突然 discard。
    float dist = length(world - pc.cameraPosition.xyz);
    float dMax = max(pc.viewportAndFar.w, 1.0);
    float minorFade = 1.0 - smoothstep(0.30 * dMax, 0.55 * dMax, dist);
    float majorFade = 1.0 - smoothstep(0.55 * dMax, 0.85 * dMax, dist);
    float grazing = smoothstep(0.03, 0.12,
        abs(dot(vec3(0.0, 0.0, 1.0), normalize(pc.cameraPosition.xyz - world))));
    vFade = mix(minorFade, majorFade, vMajor) * grazing;
    gl_Position = pc.viewProjection * vec4(world, 1.0);
}
