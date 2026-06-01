## ADDED Requirements

### Requirement: Unified .NET 10 Target

All .NET projects in the ReadNest backend, frontend, and shared model solutions SHALL target `net10.0`.

#### Scenario: Local build uses .NET 10

- **WHEN** a developer runs `dotnet build` for the backend or frontend solution
- **THEN** the projects build against `net10.0`

### Requirement: .NET 10 Deployment Infrastructure

Deployment infrastructure SHALL use .NET 10 SDK/runtime images and tools.

#### Scenario: CI publishes the Blazor frontend

- **WHEN** the GitHub Pages workflow runs
- **THEN** it installs a .NET 10 SDK before restore and publish

#### Scenario: Docker builds the backend API

- **WHEN** the backend Docker image is built
- **THEN** the build stage uses the .NET 10 SDK image
- **AND** the final stage uses the .NET 10 ASP.NET runtime image

### Requirement: No Database Migration for Runtime Upgrade

The runtime upgrade SHALL NOT introduce schema changes by itself.

#### Scenario: Upgrading target frameworks and package versions

- **WHEN** project files and infrastructure are updated for .NET 10
- **THEN** no EF Core migration is added unless a compile or runtime issue requires a model change
