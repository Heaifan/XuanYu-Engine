#version 450

// GRID-RW-1：由 gl_VertexIndex 直接生成相机吸附的固定世界网格线。
layout(push_constant) uniform GridPush {
    mat4 viewProjection;
    mat4 inverseViewProjection;
    vec4 cameraPosition;
    vec4 viewportAndFar;
    vec4 gridState; // x=Step, y=AnchorX, z=AnchorY, w=BaseHeight
} pc;

const int LINES_PER_AXIS = 513;
const int HALF_LINE_COUNT = 256;

void main() {
    int lineIndex = gl_VertexIndex / 2;
    int offset = (lineIndex % LINES_PER_AXIS) - HALF_LINE_COUNT;
    float coordinate = float(offset) * pc.gridState.x;
    float endpoint = (gl_VertexIndex % 2 == 0 ? -1.0 : 1.0) *
        float(HALF_LINE_COUNT) * pc.gridState.x;
    vec3 world = lineIndex < LINES_PER_AXIS
        ? vec3(pc.gridState.y + coordinate, pc.gridState.z + endpoint, pc.gridState.w)
        : vec3(pc.gridState.y + endpoint, pc.gridState.z + coordinate, pc.gridState.w);
    gl_Position = pc.viewProjection * vec4(world, 1.0);
}
