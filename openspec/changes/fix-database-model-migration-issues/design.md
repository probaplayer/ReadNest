## Context

The ReadNest backend has several infrastructure issues that need correction:

1. **Model-Database Mismatch**: Novel model has properties (IsLocked, SharedUserIds, IsAdult) that don't exist in the database schema. SQL queries using raw SQL through Dapper fail at runtime.

2. **Inconsistent SQL Queries**: NovelRepository has two methods - GetNovelResponse and GetNovelsFilter - that return different fields for the same entity.

3. **Seed Data Bug**: Role seed generates ID "role-admin-000" but UserRole seed references "role-admin-132" (non-existent).

4. **Version Mismatch**: EF Core 9.x packages installed but project targets .NET 8.0.

## Goals / Non-Goals

**Goals:**
- Add missing columns to Novels table via migration
- Fix SQL query consistency
- Fix seed data ID mismatch
- Ensure EF Core version compatibility with .NET 8
- Fix rate limiting JSON response format

**Non-Goals:**
- Adding soft delete functionality (user request to exclude)
- Modifying business logic or API contracts
- Performance optimizations beyond infrastructure fixes

## Decisions

1. **Migration Strategy**: Create a new migration to add IsLocked, SharedUserIds, IsAdult columns to Novels table with appropriate data types.

   *Alternative*: Drop database and recreate - rejected because it would lose existing data.

2. **SQL Fix**: Both GetNovelResponse and GetNovelsFilter should include IsLocked, SharedUserIds, IsAdult fields for consistency. Update both queries.

3. **EF Core Version**: Downgrade to Microsoft.EntityFrameworkCore.Sqlite 8.x (latest 8.x version, currently 8.0.x).

4. **Seed Data Fix**: Change UserRole seed data to reference correct Role ID "role-admin-000" instead of "role-admin-132".

5. **Rate Limiting Response**: Return proper JSON response using Response class instead of raw string.

## Risks / Trade-offs

- **Migration Risk**: Adding nullable columns is safe. Risk: LOW → Mitigation: Migration only adds columns, no data modification.

- **Package Version Risk**: Downgrading EF Core could break other features. Risk: MEDIUM → Mitigation: Use stable 8.x LTS version with documented compatibility.

- **SQL Query Risk**: Changes to raw SQL queries could introduce runtime errors. Risk: MEDIUM → Mitigation: Test all query paths after changes.

## Migration Plan

1. Run `dotnet ef migrations add AddNovelExtensionColumns`
2. Update NovelRepository.cs SQL queries
3. Fix AppDbContext.cs seed data
4. Update .csproj package versions to 8.x
5. Add Fluent API relationships to AppDbContext.cs
6. Fix Program.cs rate limiting response
7. Rename Utils..cs to Utils.cs
8. Run `dotnet build` to verify

## Open Questions

- Should Redis cache be enabled (currently commented out)? Current: Using in-memory cache.
- Are there any other missing columns in other tables? Quick scan shows Novel.cs is the main issue.