using XYUI.Avalonia.Gallery;

namespace XYUI.Avalonia.Tests;

public sealed class XYUI2Phase2CIdentityTests
{
    [Fact]
    public void Phase2C_pages_use_canonical_identity_names()
    {
        var expected = new Dictionary<string, string>
        {
            ["XYUI-2-13"] = "XY.Select",
            ["XYUI-2-14"] = "XY.TextArea",
            ["XYUI-2-15"] = "XY.SearchField",
            ["XYUI-2-16"] = "XY.PasswordField",
            ["XYUI-2-17"] = "XY.DatePicker",
            ["XYUI-2-18"] = "XY.TimePicker"
        };

        var pages = XYUI2DocumentationCatalog.Build().ToDictionary(x => x.Id);
        foreach (var pair in expected)
            Assert.Equal(pair.Value, pages[pair.Key].CanonicalIdentity);
    }
}
