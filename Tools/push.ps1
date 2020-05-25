$files = [System.IO.Directory]::GetFiles("../bin", "*.nupkg")
foreach ($file in $files) {
    dotnet nuget push $file -k oy2ftfeca4445cphia7aumdzgm63ndoupeeehtmxlibioe -s https://api.nuget.org/v3/index.json
}