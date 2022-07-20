$version = "2.4.1"
$name = "sco"

$today = Get-Date
$date = $today.ToString('yyyy-MM-dd')

Write-Host "开始打包$name  版本号:$version  日期:$date"

$fileData = @"
//github : https://github.com/qingfeng346/Scorpio-CSharp
namespace Scorpio {
    public static class Version {
        public const string version = "$version";
        public const string date = "$date";
    }
}
"@
Set-Content -Path ../Scorpio/src/Version.cs -Value $fileData -Encoding utf8

$contextExcute = Get-Content -Path ../Scorpio/src/Runtime/ScriptContextExecute.cs
Set-Content -Path ../Scorpio/src/Runtime/ScriptContextExecuteBase.cs -Value "#define EXECUTE_BASE",$contextExcute
Set-Content -Path ../Scorpio/src/Runtime/ScriptContextExecuteCoroutine.cs -Value "#define EXECUTE_COROUTINE",$contextExcute
Set-Content -Path ../Scorpio/src/Runtime/ScriptContextExecuteCoroutineBase.cs -Value "#define EXECUTE_COROUTINE","#define EXECUTE_BASE",$contextExcute

Remove-Item ../bin/* -Force -Recurse
Write-Host "正在生成nupkg..."
dotnet pack ../Scorpio/Scorpio.csproj -p:PackageVersion=$version -o ../bin/ /p:AssemblyVersion=$version | Out-Null

$platforms = @("win-x86", "win-x64", "win-arm", "win-arm64", "linux-x64", "linux-musl-x64", "linux-arm", "linux-arm64", "osx-x64", "osx-arm64")
$aipPath = ".\Install.aip"
foreach ($platform in $platforms) {
    Write-Host "正在打包 $platform 版本..."
    $pathName = "$name-$platform"
    dotnet publish ../ScorpioExec/ScorpioExec.csproj --self-contained -c release -o ../bin/$pathName -r $platform /p:DefineConstants="SCORPIO_STACK" /p:AssemblyVersion=$version | Out-Null
    Write-Host "正在压缩 $platform ..."
    $fileName = "$name-$version-$platform"
    Compress-Archive ../bin/$pathName/* ../bin/$fileName.zip -Force
    if ($IsWindows -and (($platform -eq "win-x86") -or ($platform -eq "win-x64"))) {
        Write-Host "正在生成安装包 $platform ..."
        git checkout $aipPath
        Get-ChildItem ..\bin\$pathName\ | ForEach-Object -Process{
            if($_ -is [System.IO.FileInfo]) {
                AdvancedInstaller.com /edit $aipPath /AddFile APPDIR $_.FullName
            }
        }
        AdvancedInstaller.com /edit $aipPath /SetVersion $version
        AdvancedInstaller.com /edit $aipPath /SetPackageName ..\bin\$fileName.msi -buildname DefaultBuild
        if ($platform -eq "win-x86") {
            AdvancedInstaller.com /edit $aipPath /SetPackageType x86 -buildname DefaultBuild
        } elseif ($platform -eq "win-x64") {
            AdvancedInstaller.com /edit $aipPath /SetPackageType x64 -buildname DefaultBuild
        }
        AdvancedInstaller.com /build $aipPath -buildslist DefaultBuild
        git checkout $aipPath
    }
}
Write-Host "生成完成"

Write-Host "更新winget命令 wingetcreate update --urls https://github.com/qingfeng346/Scorpio-CSharp/releases/download/v$version/sco-$version-win-x64.msi --version $version Scorpio.sco"