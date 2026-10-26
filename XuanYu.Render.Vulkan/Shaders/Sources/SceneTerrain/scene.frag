#version 450

layout(location = 0) in vec4 vBaseColor;
layout(location = 1) flat in mat4 vInvViewProjection;
layout(location = 5) in vec2 vBackgroundNdc;
layout(location = 0) out vec4 outColor;

// FAR-VIEW-A：每像素程序化编辑器环境（天空 + 连续中性环境背景）。
// 背景顶点只输出 NDC（哨兵 (2,2) 表示非背景），本片元每像素重建世界视线：
// 上半球 = 天空、接近水平 = 地平线混合区、下半球 = 连续中性环境色。
// 背景不写深度（管线深度写关），地图与实体在后续 Pass 自然覆盖。
bool isFinite(float value) {
    return !isnan(value) && !isinf(value);
}

bool isFiniteVec3(vec3 value) {
    return isFinite(value.x) && isFinite(value.y) && isFinite(value.z);
}

void main() {
    // 非背景（实体/地形/Gizmo 等）：透传顶点色。
    if (vBackgroundNdc.x > 1.5 || vBackgroundNdc.y > 1.5) {
        outColor = vBaseColor;
        return;
    }

    // 每像素重建视线方向：near/far 必须使用同一个像素的 NDC xy。
    // w 过小、非有限或极端远裁剪面时回退，避免 NaN / Inf 污染颜色。
    const float minHomogeneousW = 1e-7;
    const vec3 safeDirectionFallback = vec3(0.0, 0.0, 1.0);
    // Reverse-Z contract: NDC z=1 is Near and NDC z=0 is Far.
    vec4 nearWorld = vInvViewProjection * vec4(vBackgroundNdc, 1.0, 1.0);
    vec4 farWorld = vInvViewProjection * vec4(vBackgroundNdc, 0.0, 1.0);
    vec3 near = safeDirectionFallback;
    vec3 far = safeDirectionFallback;
    if (isFinite(nearWorld.w) && isFinite(farWorld.w) &&
        abs(nearWorld.w) > minHomogeneousW && abs(farWorld.w) > minHomogeneousW) {
        near = nearWorld.xyz / nearWorld.w;
        far = farWorld.xyz / farWorld.w;
    }
    vec3 dir = safeDirectionFallback;
    if (isFiniteVec3(near) && isFiniteVec3(far)) {
        vec3 delta = far - near;
        float lengthSquared = dot(delta, delta);
        if (isFiniteVec3(delta) && isFinite(lengthSquared) && lengthSquared > 1e-12) {
            dir = normalize(far - near);
            if (!isFiniteVec3(dir)) {
                dir = safeDirectionFallback;
            }
        }
    }

    // 连续配色：天空顶部 #A6C0DF → 近地平线 #B3C6DA；
    // 地平线以下只保留轻微中性环境渐变，不模拟 World Ground。
    vec3 skyTop = vec3(0.651, 0.753, 0.875);     // #A6C0DF
    vec3 skyHorizon = vec3(0.702, 0.776, 0.855); // #B3C6DA
    vec3 neutralEnvironment = vec3(0.665, 0.690, 0.715);

    vec3 rgb;
    if (dir.z >= 0.0) {
        // 天空：地平线蓝灰 → 天顶蓝（上半球渐变集中系数）。
        float up01 = pow(clamp(dir.z, 0.0, 1.0), 0.35);
        rgb = mix(skyHorizon, skyTop, up01);
    } else {
        // 地平线以下仍与天空在 dir.z=0 处连续，远离地平线只趋向中性色。
        float environment01 = smoothstep(-0.20, 0.0, dir.z);
        rgb = mix(neutralEnvironment, skyHorizon, environment01);
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
