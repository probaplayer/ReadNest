## 1. Project Target Frameworks

- [x] 1.1 Update backend API project to `net10.0`
- [x] 1.2 Update shared model project to `net10.0`
- [x] 1.3 Update Blazor WebAssembly project to `net10.0`

## 2. Package Versions

- [x] 2.1 Update backend Microsoft package references to `10.0.8`
- [x] 2.2 Update frontend Blazor package references to `10.0.8`
- [x] 2.3 Keep third-party package upgrades out of scope unless required by build failures

## 3. Infrastructure

- [x] 3.1 Update Docker SDK and ASP.NET runtime images to `10.0`
- [x] 3.2 Update GitHub Actions .NET SDK setup to `10.0.x`

## 4. Verification

- [x] 4.1 Restore and build backend solution
- [x] 4.2 Restore and build frontend solution
- [x] 4.3 Build Vite frontend assets
- [x] 4.4 Publish Blazor frontend for Release
- [ ] 4.5 Build backend Docker image (blocked locally: Docker CLI is not installed or not in PATH)
