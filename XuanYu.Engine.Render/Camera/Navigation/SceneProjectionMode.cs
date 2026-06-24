namespace FluidWarfare.Render.Camera.Navigation;

/// <summary>
/// 相机投影模式。
/// </summary>
public enum SceneProjectionMode
{
    /// <summary>透视投影 — 使用 FOV。</summary>
    Perspective,

    /// <summary>正交投影 — 使用 OrthographicHeight。</summary>
    Orthographic
}
