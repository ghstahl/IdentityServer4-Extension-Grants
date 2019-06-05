dotnet clean ./src/IdentityServer4-Extension-Grants.sln
dotnet restore ./src/IdentityServer4-Extension-Grants.sln
dotnet build ./src/IdentityServer4-Extension-Grants.sln
dotnet test ./src/IdentityServer4-Extension-Grants.sln --no-build
dotnet pack ./src/IdentityServer4-Extension-Grants.sln --include-source --include-symbols /property:Version=1.0.0-beta-10