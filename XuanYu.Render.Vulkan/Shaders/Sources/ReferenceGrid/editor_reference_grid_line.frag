#version 450

// GRID-RW-1-CORR2：Major/Minor Alpha 分级（Minor 0.10 / Major 0.18）+ 连续 Fade；
// 不执行 local LOD / band-pass / 突然 discard。
layout(location = 0) in float vMajor;
layout(location = 1) in float vFade;
layout(location = 0) out vec4 outColor;

void main() {
    float alpha = mix(0.10, 0.18, vMajor) * vFade;
    outColor = vec4(0.322, 0.361, 0.404, alpha);
}
