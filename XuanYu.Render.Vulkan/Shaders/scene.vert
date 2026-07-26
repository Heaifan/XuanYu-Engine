#version 450

layout(push_constant) uniform ScenePush {
    mat4 viewProjection;
    vec4 worldPosition;
} pc;

layout(location = 0) out vec4 outColor;

vec3 triangleVertex(int index) {
    vec3 vertices[3] = vec3[3](
        vec3(-0.5, -0.5, 0.0),
        vec3(0.5, -0.5, 0.0),
        vec3(0.0, 0.5, 0.0));
    return vertices[index];
}

vec3 gizmoVertex(int axis, int index) {
    vec3 direction = axis == 0 ? vec3(1.2, 0, 0) :
        (axis == 1 ? vec3(0, 1.2, 0) : vec3(0, 0, 1.2));
    vec3 side = axis == 1 ? vec3(0.035, 0, 0) : vec3(0, 0.035, 0);
    vec3 vertices[6] = vec3[6](
        -side, side, direction + side,
        -side, direction + side, direction - side);
    return vertices[index];
}

vec3 planeVertex(int plane, int index) {
    vec3 a = plane == 2 ? vec3(0, 1, 0) : vec3(1, 0, 0);
    vec3 b = plane == 0 ? vec3(0, 1, 0) : vec3(0, 0, 1);
    float i = 0.22;
    float o = 0.60;
    vec3 vertices[6] = vec3[6](
        (a * i) + (b * i), (a * o) + (b * i), (a * o) + (b * o),
        (a * i) + (b * i), (a * o) + (b * o), (a * i) + (b * o));
    return vertices[index];
}

void main() {
    vec3 world;
    if (gl_VertexIndex < 3) {
        world = triangleVertex(gl_VertexIndex) + pc.worldPosition.xyz;
        outColor = vec4(1.0, 0.85, 0.2, 1.0);
    } else if (gl_VertexIndex < 21) {
        int planeIndex = gl_VertexIndex - 3;
        int plane = planeIndex / 6;
        world = planeVertex(plane, planeIndex % 6) + pc.worldPosition.xyz;
        outColor = plane == 0 ? vec4(0.82, 0.66, 0.16, 1.0) :
            (plane == 1 ? vec4(0.64, 0.26, 0.82, 1.0) : vec4(0.16, 0.68, 0.76, 1.0));
    } else {
        int gizmoIndex = gl_VertexIndex - 21;
        int axis = gizmoIndex / 6;
        world = gizmoVertex(axis, gizmoIndex % 6) + pc.worldPosition.xyz;
        outColor = axis == 0 ? vec4(0.9, 0.18, 0.16, 1.0) :
            (axis == 1 ? vec4(0.16, 0.72, 0.28, 1.0) : vec4(0.18, 0.42, 0.95, 1.0));
    }
    gl_Position = pc.viewProjection * vec4(world, 1.0);
}
