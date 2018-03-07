dotnet publish -r win10-x64 -c release

rem removing the console window
.\Tools\editbin.exe /subsystem:windows .\Spacetris\bin\Release\netcoreapp2.0\win10-x64\Spacetris.exe
.\Tools\editbin.exe /subsystem:windows .\Spacetris\bin\Release\netcoreapp2.0\win10-x64\publish\Spacetris.exe