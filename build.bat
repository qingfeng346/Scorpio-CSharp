cd ScorpioExec
set version=1.0.1
dotnet publish -c release -o ../bin/sco-%version%-win-x64 -r win-x64
dotnet publish -c release -o ../bin/sco-%version%-osx-x64 -r osx-x64
dotnet publish -c release -o ../bin/sco-%version%-linux-x64 -r linux-x64
rd /S /Q ..\ScorpioTest\Assets\Scorpio\
mkdir ..\ScorpioTest\Assets\Scorpio\
xcopy /s /e ..\Scorpio\src\*.* ..\ScorpioTest\Assets\Scorpio\