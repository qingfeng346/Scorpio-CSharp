$version = "2.0.8"
$name = "sco"
$cur = Get-Location

$today = Get-Date
$date = $today.ToString('yyyy-MM-dd')

Write-Host "开始打包sco  版本号:$version  日期:$date"

Set-Location ../Scorpio
$fileData = @"
//github : https://github.com/qingfeng346/Scorpio-CSharp
namespace Scorpio {
    public static class Version {
        public const string version = "$version";
        public const string date = "$date";
    }
}
"@
$fileData | Out-File -Encoding utf8 ./src/Version.cs

Remove-Item ../bin/* -Force -Recurse
Write-Host "正在生成nupkg..."
dotnet pack -p:PackageVersion=$version -o ../bin/ | Out-Null

Set-Location ../ScorpioExec
# dotnet publish -c release -o ../bin/$name-win-x64 -r win-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true
# dotnet publish -c release -o ../bin/$name-osx-x64 -r osx-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true
# dotnet publish -c release -o ../bin/$name-linux-x64 -r linux-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true
Write-Host "正在打包win版本..."
dotnet publish -c release -o ../bin/$name-win-x64 -r win-x64 /p:DefineConstants="SCORPIO_DEBUG" | Out-Null
Write-Host "正在打包osx版本..."
dotnet publish -c release -o ../bin/$name-osx-x64 -r osx-x64 /p:DefineConstants="SCORPIO_DEBUG" | Out-Null
Write-Host "正在打包linux版本..."
dotnet publish -c release -o ../bin/$name-linux-x64 -r linux-x64 /p:DefineConstants="SCORPIO_DEBUG" | Out-Null

Write-Host "正在压缩文件夹..."
Compress-Archive ../bin/sco-win-x64 ../bin/$name-$version-win-x64.zip -Force
Compress-Archive ../bin/sco-osx-x64 ../bin/$name-$version-osx-x64.zip -Force
Compress-Archive ../bin/sco-linux-x64 ../bin/$name-$version-linux-x64.zip -Force

Write-Host "复制库文件到Unity示例项目"
Remove-Item ../ScorpioUnityTest/Assets/Scorpio/ -Force -Recurse
Remove-Item ../ScorpioUnityTest/Assets/Editor/ScorpioReflect -Force -Recurse
Remove-Item ../ScorpioUnityTest/Assets/Resources/ -Force -Recurse

Copy-Item ../Scorpio/src/ ../ScorpioUnityTest/Assets/Scorpio/ -Recurse -Force 
Copy-Item ../ScorpioReflect/src/ ../ScorpioUnityTest/Assets/Editor/ScorpioReflect/ -Recurse -Force
New-Item -ItemType "directory" ../ScorpioUnityTest/Assets/Resources/ | Out-Null
Get-ChildItem ../ExampleScripts | ForEach-Object -Process {
    if ($_ -is [System.IO.FileInfo]) {
        $fileName = [System.IO.Path]::GetFileNameWithoutExtension($_.Name)
        Copy-Item $_.FullName -Destination "../ScorpioUnityTest/Assets/Resources/sco_$fileName.txt" -Recurse -Force
    }
}

Set-Location $cur
Write-Host "生成完成"


# dotnet push ./a.nupkg -k oy2ibgtbm2lzfxzi3b4akycdlwhiwgxuzd3mdopbdtdqre -s https://api.nuget.org/v3/index.json