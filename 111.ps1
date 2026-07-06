$RepoUrl = "https://github.com/Heaifan/XuanYu-Engine.git"
$Branch = "fix/RZ-Fix1-editor-access-violation"

if (Test-Path ".git") {
    Remove-Item ".git" -Recurse -Force
}

git init
git checkout -b $Branch
git remote add origin $RepoUrl
git remote -v

@"

# Local build output
bin/
obj/
artifacts/

# Logs and temporary files
logs/
*.log
*.zip
chat*.txt
conversation*.txt

# IDE/user files
*.user
*.suo
.vs/
"@ | Add-Content .gitignore

git status --ignored

Write-Host "请确认 status 里没有聊天 zip、日志、bin、obj、artifacts 被加入提交。确认后按 Enter 继续。"
Read-Host

git add -A
git status

git commit -m "feat(editor): RZ-Fix2 UI layout and logging baseline"
git push -u origin $Branch