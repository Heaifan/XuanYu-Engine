#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""生成 XuanYu.Editor.UI/Design/ 的 8 个 Token XAML 文件。
唯一人工编辑源：Design/UiTokenManifest.json。
确定性：Category 固定顺序 + Key 排序，连续两次生成结果完全一致。
无第三方依赖。用法：python scripts/generate-ui-tokens.py"""

import json
import os
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
MANIFEST = os.path.join(ROOT, "XuanYu.Editor.UI", "Design", "UiTokenManifest.json")
DESIGN = os.path.join(ROOT, "XuanYu.Editor.UI", "Design")

CATEGORY_FILE = [
    ("Fonts", "UiTokens.Fonts.axaml"),
    ("Colors.Core", "UiTokens.Colors.Core.axaml"),
    ("Colors.Components", "UiTokens.Colors.Components.axaml"),
    ("Spacing", "UiTokens.Spacing.axaml"),
    ("Controls", "UiTokens.Controls.axaml"),
    ("Icons", "UiTokens.Icons.axaml"),
    ("Motion", "UiTokens.Motion.axaml"),
]

HEADER = ("<!-- 生成文件（ARCH-UI-SPEC-R1-D2-F1）：由 UiTokenManifest.json 确定性生成，禁止手工修改。 -->")


def render(token):
    t, k, v = token["Type"], token["Key"], token["Value"]
    if t == "Color":
        return f'  <SolidColorBrush x:Key="{k}" Color="{v}"/>'
    tag = {"Double": "x:Double", "String": "x:String"}.get(t, t)
    return f'  <{tag} x:Key="{k}">{v}</{tag}>'


def main():
    with open(MANIFEST, encoding="utf-8") as f:
        manifest = json.load(f)
    tokens = manifest["Tokens"]
    by_cat = {name: [t for t in tokens if t["Category"] == name] for name, _ in CATEGORY_FILE}
    for name, fname in CATEGORY_FILE:
        lines = [HEADER, "<ResourceDictionary xmlns=\"https://github.com/avaloniaui\"",
                 "                    xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">"]
        for t in sorted(by_cat[name], key=lambda x: x["Key"]):
            lines.append(render(t))
        lines.append("</ResourceDictionary>")
        with open(os.path.join(DESIGN, fname), "w", encoding="utf-8", newline="\n") as f:
            f.write("\n".join(lines) + "\n")
        print(f"生成 {fname}（{len(by_cat[name])} 条）")
    aggregate = [HEADER,
                 "<ResourceDictionary xmlns=\"https://github.com/avaloniaui\"",
                 "                    xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">",
                 "  <ResourceDictionary.MergedDictionaries>"]
    for _, fname in CATEGORY_FILE:
        aggregate.append(f'    <ResourceInclude Source="avares://XuanYu.Editor.UI/Design/{fname}"/>')
    aggregate += ["  </ResourceDictionary.MergedDictionaries>", "</ResourceDictionary>"]
    with open(os.path.join(DESIGN, "UiTokens.axaml"), "w", encoding="utf-8", newline="\n") as f:
        f.write("\n".join(aggregate) + "\n")
    print("生成 UiTokens.axaml（聚合 7 子文件）")


if __name__ == "__main__":
    sys.exit(main())
