## Why

The codebase has critical inconsistencies between Entity Framework models and the actual database schema. The Novel model defines fields (IsLocked, SharedUserIds, IsAdult) that don't exist in the database migration, causing SQL queries to fail at runtime. Additionally, seed data has mismatched IDs between Role and UserRole. These issues prevent the application from functioning correctly and must be resolved immediately.

## What Changes

- Add missing database columns (IsLocked, SharedUserIds, IsAdult) to Novels table via EF Core migration
- Fix SQL queries in NovelRepository to match current schema (GetNovelsFilter missing fields that GetNovelResponse has)
- Fix seed data Role ID mismatch between Roles and UserRoles table
- Downgrade EF Core packages from 9.x to 8.x for .NET 8 compatibility
- Add Fluent API relationship configurations in AppDbContext for proper entity relationships
- Fix rate limiting response to return JSON format instead of plain text
- Rename Utils..cs to Utils.cs (fix double-dot filename)
- Configure Redis cache properly (uncomment or remove conflicting memory cache)

## Capabilities

This is a bug fix / infrastructure correction - no new capabilities being introduced.

### New Capabilities
None - all changes are bug fixes and infrastructure corrections.

### Modified Capabilities
None - no existing spec requirements are being changed.

## Impact

- **Backend**: NovelRepository.cs, AppDbContext.cs, Program.cs, seed data in AppDbContext.cs
- **Database**: New migration required for Novels table
- **Dependencies**: EF Core package versions need downgrading
- **Build**: Will require `dotnet restore` and `dotnet build` after package updates