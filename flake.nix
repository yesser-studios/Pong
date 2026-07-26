{
    description = "Pong by Yesser Studios";

    inputs = {
        nixpkgs.url = "github:nixos/nixpkgs?ref=nixos-unstable";
        flake-utils.url = "github:numtide/flake-utils";
    };

    outputs = { self, nixpkgs, flake-utils }: 
        flake-utils.lib.eachDefaultSystem (system:
        let
            pkgs = import nixpkgs { inherit system; };
            lib = pkgs.lib;
        in {
            packages.default = pkgs.buildDotnetModule {
                pname = "pong-by-yesser-studios";
                version = "1.2.4";
                src = ./.;
                projectFile = "Pong.Desktop/Pong.Desktop.csproj";

                dotnet-sdk = pkgs.dotnetCorePackages.sdk_8_0;
                dotnet-runtime = pkgs.dotnetCorePackages.runtime_8_0;

                executables = [ "Pong.Desktop" ];

                packNupkg = false;
                meta = {
                    description = "A Pong clone";
                    license = lib.licenses.mit;
                    maintainers = with lib.maintainers; [ yesseruser ];
                    homepage = "https://github.com/yesser-studios/Pong";
                };
            };
            devShells.default = pkgs.mkShell {
                packages = [ pkgs.dotnetCorePackages.sdk_8_0 pkgs.nuget-to-json ];
            };
        });
}