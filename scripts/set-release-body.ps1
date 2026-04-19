function Find-ModPath($name) {
    $searchPaths = @("src/$name", "src/Mods/$name", "src/ExternalMods/$name")
    foreach ($p in $searchPaths) {
        if (Test-Path $p) {
            return $p
        }
    }
    return $null
}

$tags = gh release list --json tagName --jq .[].tagName

foreach ($tag in $tags) {
    if ($tag -match "^([^/]+)/(.+)$") {
        $modName = $matches[1]

        $modPath = Find-ModPath $modName
        if (-not $modPath) { continue }

        $readmePath = "$modPath/README.md"
        if (-not (Test-Path $readmePath)) { continue }

        $desc = "[Mod Description](./$readmePath)"

        gh release edit $tag --notes "$desc"
    }
}
