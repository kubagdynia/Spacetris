dotnet publish -c release

rem removing the console window
.\Tools\editbin.exe /subsystem:windows .\Spacetris\bin\Release\net7.0\Spacetris.exe
.\Tools\editbin.exe /subsystem:windows .\Spacetris\bin\Release\net7.0\publish\Spacetris.exe