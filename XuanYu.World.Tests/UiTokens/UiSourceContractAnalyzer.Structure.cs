using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
namespace XuanYu.World.Tests.UiTokens;
// ARCH-UI-SPEC-R1-D2-F2：AXAML 结构索引——稳定定位 v3。
// 匿名元素 → "Path:<最近命名祖先|ROOT>/<父类型链>/<类型>:<同父序号>"；命名元素 → "Name:<x:Name>"。
public static partial class UiSourceContractAnalyzer
{
    private static readonly Regex TagRx = new(
        @"<(?<close>/)?(?<tag>[A-Za-z]\w*)\b(?<attrs>[^>]*?)(?<self>/)?>", RegexOptions.Compiled);
    private static readonly Regex NameAttrRx = new(@"x:Name=""([^""]+)""", RegexOptions.Compiled);
    private static readonly Regex AttrBeforeHexRx = new(@"(?<attr>[A-Za-z][\w.]*)\s*=\s*""[^""]*$", RegexOptions.Compiled);

    internal sealed record El(int Start, int End, string Tag, string? Name, string ParentKey, int Ordinal);

    // 结构扫描：元素开标签起始 → 元素信息（父链 key + 同父序号）。
    internal static Dictionary<int, El> BuildIndex(string text)
    {
        var index = new Dictionary<int, El>();
        var stack = new Stack<El>();
        var ordinal = new Dictionary<string, int>();
        var seen = new List<(int Start, int End)>();
        foreach (Match m in TagRx.Matches(text))
        {
            if (seen.Any(s => m.Index > s.Start && m.Index < s.End))
                continue;
            var tag = m.Groups["tag"].Value;
            var attrs = m.Groups["attrs"].Value;
            var name = NameAttrRx.Match(attrs).Success ? NameAttrRx.Match(attrs).Groups[1].Value : null;
            if (m.Groups["close"].Success)
            {
                if (stack.Count > 0 && stack.Peek().Tag == tag)
                {
                    var top = stack.Pop();
                    index[top.Start] = top with { End = m.Index };
                }
                continue;
            }
            var chain = new List<string>();
            var prefix = "ROOT";
            foreach (var a in stack.Reverse()) // 根→父 顺序（与生成脚本一致）
            {
                if (a.Name != null)
                {
                    prefix = "Name:" + a.Name;
                    chain.Clear();
                }
                else
                    chain.Add(a.Tag);
            }
            var parentKey = chain.Count == 0 ? prefix : prefix + "/" + string.Join("/", chain);
            var ordKey = parentKey + "/" + tag;
            ordinal[ordKey] = ordinal.GetValueOrDefault(ordKey) + 1;
            var el = new El(m.Index, -1, tag, name, parentKey, ordinal[ordKey]);
            if (m.Groups["self"].Success)
            {
                index[m.Index] = el with { End = m.Index + m.Length };
                continue;
            }
            stack.Push(el);
            seen.Add((m.Index, m.Index + m.Length));
        }
        foreach (var el in stack)
            index[el.Start] = el with { End = text.Length };
        return index;
    }

    internal static string LocatorOf(El e) =>
        e.Name != null ? $"Name:{e.Name}" : $"Path:{e.ParentKey}/{e.Tag}:{e.Ordinal}";

    internal static El? FindEl(Dictionary<int, El> index, int pos)
    {
        El? best = null;
        foreach (var kv in index)
            if (pos > kv.Key && pos < kv.Value.End)
                if (best == null || kv.Value.End - kv.Key < best.End - best.Start)
                    best = kv.Value;
        return best;
    }

    private static string AttributeName(string text, int hexPos)
    {
        var m = AttrBeforeHexRx.Match(text, 0, hexPos + 1);
        return m.Success ? m.Groups["attr"].Value : "Color";
    }
}
