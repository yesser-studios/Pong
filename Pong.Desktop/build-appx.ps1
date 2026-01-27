dotnet build -c Release -r win-x64
winapp pack .\bin\Release\net8.0\win-x64 --manifest .\appxmanifest.xml --cert .\devcert.pfx --output .\bin\msix\x64.msix
dotnet build -c Release -r win-x86
winapp pack .\bin\Release\net8.0\win-x86 --manifest .\appxmanifest.xml --cert .\devcert.pfx --output .\bin\msix\x86.msix
dotnet build -c Release -r win-arm64
winapp pack .\bin\Release\net8.0\win-arm64 --manifest .\appxmanifest.xml --cert .\devcert.pfx --output .\bin\msix\arm64.msix
