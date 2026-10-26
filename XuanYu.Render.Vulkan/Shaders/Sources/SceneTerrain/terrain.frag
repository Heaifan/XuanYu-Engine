#version 450
layout(location = 0) in vec3 inNormal;
layout(location = 0) out vec4 outColor;
void main() {
    float light = 0.45 + 0.55 * max(dot(normalize(inNormal), normalize(vec3(0.35, 0.55, 0.75))), 0.0);
    outColor = vec4(vec3(0.24, 0.48, 0.32) * light, 1.0);
}
