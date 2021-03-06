$folders = Get-ChildItem -Directory -Exclude @( "Packages", "Samples", ".vs", ".git" )

foreach ($folder in $folders) {
    $p = $folder.Name;

    nuget pack "$p/$p.csproj" -Build -OutputDirectory "packages" -Properties "Configuration=Release"
}

$files = Get-ChildItem -File -Filter "ToolKitty*" -Path "packages"

foreach ($file in $files) {
    $p = $file.Name;

    nuget push "packages/$p" -Source "https://api.nuget.org/v3/index.json"
}