$files = [System.IO.Directory]::GetFiles("../bin", "*.nupkg")
$apiKey = [System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String("b3kyZWJucXlkc3R1MmF5MnFkNXFobHNna29odDNqbnVuZnBhd3A0cGc2cTZuZQ=="))
foreach ($file in $files) {
    dotnet nuget push $file -k "$apiKey" -s https://api.nuget.org/v3/index.json
}