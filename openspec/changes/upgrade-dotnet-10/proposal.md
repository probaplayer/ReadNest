## Why

ReadNest currently mixes .NET 8 backend/shared projects with a .NET 9 Blazor WebAssembly frontend, while the GitHub Pages workflow still installs only the .NET 8 SDK. This makes local and CI builds depend on whichever SDK happens to be installed.

.NET 10 is the next LTS target for the application stack. Upgrading all .NET projects and deployment infrastructure together keeps the backend, frontend, shared model assembly, Docker image, and CI pipeline aligned.

## What Changes

- Retarget backend API, shared model, and Blazor WebAssembly projects to `net10.0`.
- Upgrade Microsoft ASP.NET Core, EF Core, SQLite, and Blazor package references to .NET 10 package versions.
- Update backend Docker build/runtime images from .NET 8 to .NET 10.
- Update the GitHub Pages workflow to install the .NET 10 SDK.

## Impact

- Requires .NET 10 SDK for local development and CI.
- Requires .NET 10 ASP.NET runtime in backend containers.
- No intentional database schema or EF migration changes.
- Existing pending OpenSpec changes for image cleanup and Docker database path remain separate.
