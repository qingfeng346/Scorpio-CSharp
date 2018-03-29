cd ScorpioExec
dotnet publish -c release -o ../bin/win -r win7-x64
dotnet publish -c release -o ../bin/mac -r osx.10.11-x64
dotnet publish -c release -o ../bin/centos -r centos.7-x64
dotnet publish -c release -o ../bin/ubuntu -r ubuntu.14.04-x64
dotnet publish -c release -o ../bin/debian -r debian.8-x64