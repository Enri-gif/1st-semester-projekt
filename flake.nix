{
    description = "C# dev flake with dotnet environment";

    inputs = {
        nixpkgs.url = "github:nixos/nixpkgs/nixos-unstable";
    };

    outputs = { self, nixpkgs, ... }:
    let
        system = "x86_64-linux";
        pkgs = nixpkgs.legacyPackages.${system};

        dotnet = pkgs.dotnet-sdk_10;
    in
    {
        devShells.${system}.default = pkgs.mkShell {
            packages = [
                dotnet
                pkgs.sqlcmd
            ];

            shellHook = ''
                export DOTNET_ROOT=${dotnet}/share/dotnet
                export DOTNET_MSBUILD_SDK_RESOLVER_SDKS_DIR=${dotnet}/share/dotnet/sdk
                export MSBuildSDKsPath=${dotnet}/share/dotnet/sdk/${dotnet.version}/Sdks

                export NUGET_PACKAGES=$PWD/.nupkg

                if [ ! -d "$NUGET_PACKAGES" ]; then
                    echo "Restoring NuGet packages for project..."
                    dotnet restore
                fi

                eval "$(dotnet completions script zsh)"

                if [ -f .config/dotnet-tools.json ]; then
                    dotnet tool restore
                fi

                exec zsh
            '';
        };
    };
}
