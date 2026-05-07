## ADDED Requirements

### Requirement: Database schema must match model definitions

The system MUST ensure all Entity Framework model properties have corresponding database columns. When models define properties, the database schema MUST include those columns to prevent runtime SQL errors.

#### Scenario: Novel model properties exist in database
- **WHEN** Novel model defines IsLocked, SharedUserIds, IsAdult properties
- **THEN** Novels database table MUST have corresponding columns with appropriate data types

### Requirement: SQL queries must return consistent fields

The system MUST ensure all methods in NovelRepository returning Novel data include the same set of fields. GetNovelResponse and GetNovelsFilter MUST return identical field sets for consistency.

#### Scenario: Both query methods return same fields
- **WHEN** Calling GetNovelResponse and GetNovelsFilter
- **THEN** Both methods MUST return IsLocked, SharedUserIds, IsAdult fields

### Requirement: Seed data must have correct foreign key references

The system MUST ensure Role and UserRole seed data have matching IDs. UserRole entries referencing Roles must use valid Role IDs.

#### Scenario: UserRole references existing Role
- **WHEN** Application starts and seeds database
- **THEN** UserRole with RoleId "role-admin-000" MUST reference an existing Role with that ID

### Requirement: Rate limiting response must be JSON format

The system MUST return properly formatted JSON responses for rate limiting errors, not plain text strings.

#### Scenario: Rate limit exceeded returns JSON
- **WHEN** Client exceeds rate limit
- **THEN** Response MUST be JSON format matching Response<T> class structure

### Requirement: EF Core packages must match .NET version

The system MUST use Entity Framework Core packages compatible with the target .NET runtime version.

#### Scenario: EF Core version compatible with .NET 8
- **WHEN** Project targets .NET 8.0
- **THEN** EF Core packages MUST be 8.x version for compatibility