## 1. Database Migration

- [x] 1.1 Run `dotnet ef migrations add AddNovelExtensionColumns` to create new migration
- [x] 1.2 Verify migration creates IsLocked, SharedUserIds, IsAdult columns in Novels table
- [x] 1.3 Run `dotnet ef database update` to apply migration

## 2. Fix SQL Queries in NovelRepository

- [x] 2.1 Update GetNovelsFilter method to include IsLocked, SharedUserIds, IsAdult fields in SELECT
- [x] 2.2 Verify both GetNovelResponse and GetNovelsFilter return consistent field set

## 3. Fix Seed Data in AppDbContext

- [x] 3.1 Fix UserRole seed data to reference correct Role ID "role-admin-000" instead of "role-admin-132"
- [x] 3.2 Verify Role seed generates IDs that match UserRole references

## 4. Fix EF Core Version Compatibility

- [x] 4.1 Update .NET version from 8.0 to 10.0 in ReadNest-BE.csproj
- [x] 4.2 Build passes with .NET 10
- [x] 4.3 Run `dotnet restore` to update packages
- [x] 4.4 Run `dotnet build` to verify no breaking changes

## 5. Database Migration to Docker

- [x] 5.1 Migrate data from ReadNestDb to readnest.db
- [x] 5.2 Add IsAdult, IsLocked, SharedUserIds columns to existing data

## 9. Verification

- [x] 9.1 Run `dotnet build` to ensure all code compiles
- [x] 9.2 Database schema is correct with new columns