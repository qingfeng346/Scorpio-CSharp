$files = [System.IO.Directory]::GetFiles("../bin", "*.nupkg")
$apiKey = [System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String("b3kyZDVxNmEyenZ1enlsZm5rcHFhaGU0eGlxMnJuZHViY2ZuMmQ2MnE3YmoyYQ=="))
foreach ($file in $files) {
    dotnet nuget push $file -k "$apiKey" -s https://api.nuget.org/v3/index.json
}