cd Scorpio
dotnet build -c Release -o ../bin/win
copy /y ..\bin\win\Scorpio.dll ..\ScorpioTest\Assets\Plugins
cd ../ScorpioExec
dotnet build -c Release -o ../bin/win -r win7-x64