$version = "2.3.5"
$name = "sco"
$cur = Get-Location

$today = Get-Date
$date = $today.ToString('yyyy-MM-dd')

Write-Host "开始打包$name  版本号:$version  日期:$date"

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
Set-Content -Path ./src/Version.cs -Value $fileData -Encoding utf8

$contextExcute = Get-Content -Path ./src/Runtime/ScriptContextExecute.cs
Set-Content -Path ./src/Runtime/ScriptContextExecuteBase.cs -Value "#define EXECUTE_BASE",$contextExcute
Set-Content -Path ./src/Runtime/ScriptContextExecuteCoroutine.cs -Value "#define EXECUTE_COROUTINE",$contextExcute
Set-Content -Path ./src/Runtime/ScriptContextExecuteCoroutineBase.cs -Value "#define EXECUTE_COROUTINE","#define EXECUTE_BASE",$contextExcute

Remove-Item ../bin/* -Force -Recurse
Write-Host "正在生成nupkg..."
dotnet pack -p:PackageVersion=$version -o ../bin/ /p:AssemblyVersion=$version | Out-Null

Set-Location ../ScorpioExec

$platforms = @("win-x86", "win-x64", "win-arm", "win-arm64", "linux-x64", "linux-musl-x64", "linux-arm", "linux-arm64", "osx-x64", "osx-arm64")
foreach ($platform in $platforms) {
    Write-Host "正在打包 $platform 版本..."
    $pathName = $name-$platform
    dotnet publish --self-contained -c release -o ../bin/$pathName -r $platform /p:DefineConstants="SCORPIO_STACK" /p:AssemblyVersion=$version | Out-Null
    Write-Host "正在压缩 $platform ..."
    $fileName = "$name-$version-$platform"
    Compress-Archive ../bin/$pathName/* ../bin/$fileName.zip -Force
    if ($IsWindows) {
        if ($platform -eq "win-x86") {
            git checkout ./Install.aip
            AdvancedInstaller.com /edit .\Install.aip /AddFolder APPDIR ..\bin\$pathName\
            AdvancedInstaller.com /edit .\Install.aip /SetPackageName ..\bin\$fileName.msi -buildname DefaultBuild
            AdvancedInstaller.com /edit MyProject.aip /SetPackageType x86 DefaultBuild
            AdvancedInstaller.com /edit .\Install.aip /SetVersion $version
            AdvancedInstaller.com /build .\Install.aip buildslist DefaultBuild
        } elseif ($platform -eq "win-x64") {
            git checkout ./Install.aip
            AdvancedInstaller.com /edit .\Install.aip /AddFolder APPDIR ..\bin\$pathName\
            AdvancedInstaller.com /edit .\Install.aip /SetPackageName ..\bin\$fileName.msi -buildname DefaultBuild
            AdvancedInstaller.com /edit MyProject.aip /SetPackageType x64 DefaultBuild
            AdvancedInstaller.com /edit .\Install.aip /SetVersion $version
            AdvancedInstaller.com /build .\Install.aip buildslist DefaultBuild
        }
    }
}
Set-Location $cur
Write-Host "生成完成"