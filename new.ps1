# 改进后的替换/重命名脚本
[CmdletBinding(SupportsShouldProcess=$true)]
param(
    [Parameter(Mandatory=$true, Position=0)]
    [string]$NewString,

    [Parameter(Mandatory=$false)]
    [string]$OldString = "Stargazer.Abp.Template",

    [string[]]$Exclude = @(".git","bin","obj","node_modules"),

    [ValidateSet("utf8","utf8bom","unicode","ascii","default")]
    [string]$Encoding = "utf8"
)

# 切换到脚本所在目录
Set-Location $PSScriptRoot

Write-Host "开始替换：'$OldString' -> '$NewString'"
Write-Host "排除目录：" ($Exclude -join ", ")
Write-Host "编码：" $Encoding

$escapedOld = [Regex]::Escape($OldString)

# Helper: 检查路径是否在排除目录中
function IsExcludedPath($path) {
    foreach ($e in $Exclude) {
        if ($path -like "*$e*") { return $true }
    }
    return $false
}

# 1) 先更新文件内容（只处理文本文件）
Get-ChildItem -Recurse -File | Where-Object { -not (IsExcludedPath $_.FullName) } | ForEach-Object {
    try {
        $file = $_.FullName
        # 尝试以文本方式读取 -Raw；若包含 NULL 字节，视为二进制并跳过
        $raw = Get-Content -Raw -ErrorAction Stop -LiteralPath $file
        if ($raw -match "`0") {
            Write-Verbose "跳过二进制文件: $file"
            return
        }

        if ($raw -match $escapedOld) {
            $newContent = $raw -replace $escapedOld, $NewString
            if ($PSCmdlet.ShouldProcess("文件: $file", "替换内容")) {
                switch ($Encoding) {
                    "utf8"     { Set-Content -LiteralPath $file -Value $newContent -Encoding utf8 -Force }
                    "utf8bom"  { Set-Content -LiteralPath $file -Value $newContent -Encoding utf8BOM -Force }
                    "unicode"  { Set-Content -LiteralPath $file -Value $newContent -Encoding Unicode -Force }
                    "ascii"    { Set-Content -LiteralPath $file -Value $newContent -Encoding ascii -Force }
                    "default"  { Set-Content -LiteralPath $file -Value $newContent -Encoding Default -Force }
                }
                Write-Host "已更新文件: $file"
            } else {
                Write-Host "（WhatIf）将更新文件: $file"
            }
        }
    } catch {
        Write-Warning "处理文件时出错：$($_.FullName) -> $($_.Exception.Message)"
    }
}

# 2) 再按从深到浅重命名目录，避免父目录先被改名影响遍历
Get-ChildItem -Recurse -Directory | Sort-Object FullName -Descending | ForEach-Object {
    if (IsExcludedPath $_.FullName) { return }
    $oldName = $_.Name
    if ($oldName -match $escapedOld) {
        $newName = $oldName -replace $escapedOld, $NewString
        $parent = Split-Path -Parent $_.FullName
        $target = Join-Path $parent $newName
        if ($PSCmdlet.ShouldProcess("目录: $($_.FullName)", "重命名为 $target")) {
            try {
                Rename-Item -LiteralPath $_.FullName -NewName $newName -Force
                Write-Host "已重命名：$($_.FullName) -> $target"
            } catch {
                Write-Warning "重命名失败：$($_.FullName) -> $newName : $($_.Exception.Message)"
            }
        } else {
            Write-Host "（WhatIf）将重命名：$($_.FullName) -> $target"
        }
    }
}

Write-Host "完成。请检查并根据需要运行 git status / restore。"