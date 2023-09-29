dotnet build -c release

rem removing the console window
.\Tools\editbin.exe /subsystem:windows .\Spacetris\bin\Release\net7.0\Spacetris.exe