param(
    [string]$OutputPath = (Join-Path (Get-Location) ("PhoAmThucVinhKhanh-clean-" + (Get-Date -Format "yyyyMMdd-HHmmss") + ".zip"))
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$outputFile = [System.IO.Path]::GetFullPath($OutputPath)
$outputDir = Split-Path -Parent $outputFile

if (-not (Test-Path -LiteralPath $outputDir)) {
    New-Item -ItemType Directory -Path $outputDir | Out-Null
}

$excludeDirectoryNames = @(
    '.git',
    '.codegraph',
    '.codex',
    '.agents',
    'node_modules',
    'dist',
    'bin',
    'obj',
    'tmp'
)

function Should-ExcludeFile {
    param(
        [string]$RelativePath
    )

    $normalized = $RelativePath -replace '/', '\'

    foreach ($segment in $normalized.Split('\')) {
        if ($excludeDirectoryNames -contains $segment) {
            return $true
        }
    }

    if ($normalized -match '^backend\\.*\\_build_verify[^\\]*(\\|$)') {
        return $true
    }

    if ($normalized -like 'backend\Quan4CulinaryTourism.Api\wwwroot\uploads\*') {
        return $true
    }

    $fileName = [System.IO.Path]::GetFileName($normalized)
    if ($fileName -eq '.env') {
        return $true
    }

    if ($fileName.StartsWith('.env.') -and $fileName -ne '.env.example') {
        return $true
    }

    return $false
}

function Get-RepoRelativePath {
    param(
        [string]$FullPath
    )

    $resolvedFullPath = [System.IO.Path]::GetFullPath($FullPath)
    if ($resolvedFullPath.StartsWith($repoRoot, [System.StringComparison]::OrdinalIgnoreCase)) {
        return $resolvedFullPath.Substring($repoRoot.Length).TrimStart('\', '/')
    }

    return $resolvedFullPath
}

$files = Get-ChildItem -LiteralPath $repoRoot -File -Recurse | Where-Object {
    $relativePath = Get-RepoRelativePath -FullPath $_.FullName
    -not (Should-ExcludeFile -RelativePath $relativePath)
}

if (Test-Path -LiteralPath $outputFile) {
    Remove-Item -LiteralPath $outputFile -Force
}

Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem

$zipArchive = [System.IO.Compression.ZipFile]::Open($outputFile, [System.IO.Compression.ZipArchiveMode]::Create)
try {
    foreach ($file in $files) {
        $relativePath = Get-RepoRelativePath -FullPath $file.FullName
        [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zipArchive, $file.FullName, $relativePath, [System.IO.Compression.CompressionLevel]::Optimal) | Out-Null
    }
}
finally {
    $zipArchive.Dispose()
}

Write-Output "Created clean zip: $outputFile"
