cd ScorpioExec
set version=1.0.2
dotnet publish -c release -o ../bin/sco-%version%-win-x64 -r win-x64
dotnet publish -c release -o ../bin/sco-%version%-osx-x64 -r osx-x64
dotnet publish -c release -o ../bin/sco-%version%-linux-x64 -r linux-x64
cd ../Scorpio
dotnet pack -p:PackageVersion=%version% -o ../bin
rd /S /Q ..\ScorpioTest\Assets\Scorpio\
mkdir ..\ScorpioTest\Assets\Scorpio\
xcopy /s /e ..\Scorpio\src\*.* ..\ScorpioTest\Assets\Scorpio\