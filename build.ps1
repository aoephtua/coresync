dotnet publish -c Release -r win-x64 --self-contained false
dotnet publish -c Release -r win-x86 --self-contained false
dotnet publish -c Release -r linux-x64 --self-contained false
dotnet publish -c Release -r osx-x64 --self-contained false