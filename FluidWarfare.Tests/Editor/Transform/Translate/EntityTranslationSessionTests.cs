using FluidWarfare.Core.Math;
using FluidWarfare.Editor.Transform.Translate;

namespace FluidWarfare.Tests.Editor.Transform.Translate;

public sealed class EntityTranslationSessionTests
{
    private static readonly TranslationCameraSnapshot TestCamera = new()
    {
        Position = new Vector3d(10, 10, 10),
        ViewProjection = IdentityMatrix(),
        InverseViewProjection = IdentityMatrix(),
        ViewportWidth = 800,
        ViewportHeight = 600
    };

    [Fact]
    public void GroundPlane_UsesFrozenPlaneHitDelta()
    {
        var session = new EntityTranslationSession();
        var initial = new Vector3d(10, 20, 5);
        var startRay = Ray(new Vector3d(10, 20, 20), new Vector3d(0, 0, -1));
        var currentRay = Ray(new Vector3d(14, 17, 20), new Vector3d(0, 0, -1));

        Assert.True(session.Begin("1", initial, TranslationConstraint.GroundPlane,
            startRay, 400, 300, TestCamera, initialSceneDirty: false));
        Assert.True(session.Preview(currentRay, 420, 310, out var pos));

        AssertClose(new Vector3d(14, 17, 5), pos);
    }

    [Fact]
    public void ZConstraint_MovesOnlyAlongWorldZ()
    {
        var camera = TestCamera with { Position = new Vector3d(10, 0, 10) };
        var session = new EntityTranslationSession();
        var startRay = Ray(new Vector3d(5, 0, 5), new Vector3d(-1, 0, -1));
        var currentRay = Ray(new Vector3d(5, 0, 8), new Vector3d(-1, 0, -1));

        Assert.True(session.Begin("1", Vector3d.Zero, TranslationConstraint.Z,
            startRay, 100, 100, camera, initialSceneDirty: false));
        Assert.True(session.Preview(currentRay, 100, 80, out var pos));

        AssertClose(new Vector3d(0, 0, 3), pos);
    }

    [Fact]
    public void SwitchConstraint_ReanchorsFromCurrentPreview()
    {
        var camera = TestCamera with { Position = new Vector3d(10, 0, 10) };
        var session = new EntityTranslationSession();
        var startRay = Ray(new Vector3d(0, 0, 10), new Vector3d(0, 0, -1));
        var groundRay = Ray(new Vector3d(2, 0, 10), new Vector3d(0, 0, -1));
        var zStartRay = Ray(new Vector3d(5, 0, 2), new Vector3d(-1, 0, -1));
        var zCurrentRay = Ray(new Vector3d(5, 0, 4), new Vector3d(-1, 0, -1));

        Assert.True(session.Begin("1", Vector3d.Zero, TranslationConstraint.GroundPlane,
            startRay, 0, 0, camera, initialSceneDirty: false));
        Assert.True(session.Preview(groundRay, 10, 0, out var groundPos));
        AssertClose(new Vector3d(2, 0, 0), groundPos);

        Assert.True(session.SwitchConstraint(TranslationConstraint.Z, zStartRay, 10, 0));
        Assert.True(session.Preview(zCurrentRay, 10, -10, out var zPos));

        AssertClose(new Vector3d(2, 0, 2), zPos);
    }

    [Fact]
    public void Cancel_RestoresInitialPosition()
    {
        var session = new EntityTranslationSession();
        TranslationResult? completed = null;
        session.Completed += result => completed = result;

        Assert.True(session.Begin("1", Vector3d.Zero, TranslationConstraint.GroundPlane,
            Ray(new Vector3d(0, 0, 10), new Vector3d(0, 0, -1)),
            0, 0, TestCamera, initialSceneDirty: true));
        Assert.True(session.Preview(Ray(new Vector3d(4, 0, 10), new Vector3d(0, 0, -1)), 10, 0, out _));
        session.Cancel();

        Assert.False(session.IsActive);
        Assert.NotNull(completed);
        Assert.True(completed.Value.IsCancelled);
        Assert.True(completed.Value.InitialWasDirty);
        AssertClose(Vector3d.Zero, completed.Value.FinalPosition!.Value);
    }

    private static TranslationRay Ray(Vector3d origin, Vector3d direction) =>
        new(origin, direction.Normalize());

    private static double[] IdentityMatrix() =>
    [
        1, 0, 0, 0,
        0, 1, 0, 0,
        0, 0, 1, 0,
        0, 0, 0, 1
    ];

    private static void AssertClose(Vector3d expected, Vector3d actual)
    {
        Assert.Equal(expected.X, actual.X, 6);
        Assert.Equal(expected.Y, actual.Y, 6);
        Assert.Equal(expected.Z, actual.Z, 6);
    }
}
