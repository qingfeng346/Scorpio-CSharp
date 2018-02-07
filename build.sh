cd Scorpio
dotnet build -o ..\bin\mac
cp -rf ..\bin\mac\Scorpio.dll ..\ScorpioTest\Assets\Plugins
cd ..\ScorpioExec
dotnet build -o ..\bin\mac -r osx.10.11-x64