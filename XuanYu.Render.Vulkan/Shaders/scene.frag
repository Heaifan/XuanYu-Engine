#version 450

layout(location = 0) in vec4 vBaseColor;
layout(location = 1) flat in mat4 vInvViewProjection;
layout(location = 5) in vec2 vBackgroundNdc;
layout(location = 0) out vec4 outColor;

// MAP-A-R1-D5-R1-F2-R3-R2：每像素程序化编辑器环境（天空 + 中性灰参考地面）。
// 背景顶点只输出 NDC（哨兵 (2,2) 表示非背景），本片元每像素重建世界视线：
// 上半球 = 天空、接近水平 = 地平线混合区、下半球 = 中性灰参考地面。
// 背景不写深度（管线深度写关），地图与实体在后续 Pass 自然覆盖。
// 所有 smoothstep 均满足 edge0 < edge1（修正上一版反写）。
void main() {
    // 非背景（实体/地形/Gizmo 等）：透传顶点色。
    if (vBackgroundNdc.x > 1.5 || vBackgroundNdc.y > 1.5) {
        outColor = vBaseColor;
        return;
    }

    // 每像素重建视线方向（far-cam 差值，仅依赖相机旋转与投影）。
    vec4 farWorld = vInvViewProjection * vec4(vBackgroundNdc, 1.0, 1.0);
    vec4 camWorld = vInvViewProjection * vec4(0.0, 0.0, 0.0, 1.0);
    vec3 dir = normalize(farWorld.xyz / farWorld.w - camWorld.xyz / camWorld.w);

    // F2-R3-R2 配色（用户验收建议，浅色体系、对比拉开）：
    // 天空顶部 #A6C0DF → 天空近地平线 #B3C6DA → 地平线 #9CA6AF
    // → 远处参考地面 #858B91 → 近处参考地面 #747A80。
    vec3 skyTop = vec3(0.651, 0.753, 0.875);     // #A6C0DF
    vec3 skyHorizon = vec3(0.702, 0.776, 0.855); // #B3C6DA
    vec3 horizonColor = vec3(0.612, 0.651, 0.686); // #9CA6AF
    vec3 groundFar = vec3(0.522, 0.545, 0.569);  // #858B91
    vec3 groundNear = vec3(0.455, 0.478, 0.502); // #747A80

    vec3 rgb;
    if (dir.z >= 0.0) {
        // 天空：地平线蓝灰 → 天顶蓝（上半球渐变集中系数）。
        float up01 = pow(clamp(dir.z, 0.0, 1.0), 0.35);
        rgb = mix(skyHorizon, skyTop, up01);
    } else {
        // 地平线过渡（edge0<edge1）：dir.z ∈ [-0.06, 0] 地平线色 → 地面色。
        float belowHorizon = 1.0 - smoothstep(-0.06, 0.0, dir.z);
        // 地面远近（符号修正）：-dir.z ∈ [0.06, 0.50] 远处灰 → 近处深灰。
        float groundNearness = smoothstep(0.06, 0.50, -dir.z);
        vec3 groundColor = mix(groundFar, groundNear, groundNearness);
        rgb = mix(horizonColor, groundColor, belowHorizon);
    }

    // 最小太阳圆盘：方向与 D1 合同 sunDirection 一致（归一化 (-0.35,-0.55,0.75)）。
    // 只做简单圆盘 + 微弱辉光，不做耀斑与体积光。
    vec3 sunDir = normalize(vec3(-0.35, -0.55, 0.75));
    float facingSun = max(dot(dir, sunDir), 0.0);
    float disk = smoothstep(0.9992, 0.9998, facingSun);       // 圆盘
    float glow = smoothstep(0.9950, 1.0000, facingSun) * 0.35; // 微弱辉光
    rgb += vec3(1.0, 0.96, 0.88) * (disk + glow);

    outColor = vec4(clamp(rgb, 0.0, 1.0), 1.0);
}
