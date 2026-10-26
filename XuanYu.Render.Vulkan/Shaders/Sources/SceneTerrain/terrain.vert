#version 450
layout(location = 0) in vec3 inPosition;
layout(location = 1) in vec3 inNormal;
layout(location = 0) out vec3 outNormal;
layout(push_constant) uniform Scene { mat4 viewProjection; vec4 worldPosition; } scene;
void main() {
    gl_Position = scene.viewProjection * vec4(inPosition + scene.worldPosition.xyz, 1.0);
    outNormal = inNormal;
}
