## ADDED Requirements

### Requirement: Entities support soft delete flag
All entities inheriting from BaseModel SHALL support an IsDeleted flag that can be used for soft delete functionality.

#### Scenario: User is soft deleted
- **WHEN** an admin marks a user as deleted
- **THEN** the user's IsDeleted field SHALL be set to true
- **AND** the user SHALL be excluded from normal queries (when soft delete is enabled)

### Requirement: BaseModel includes IsDeleted
The BaseModel class SHALL include an IsDeleted boolean property defaulting to false.

#### Scenario: New entity has IsDeleted defaulting to false
- **WHEN** a new entity is created
- **THEN** the IsDeleted property SHALL default to false

## Open for Implementation

The following are NOT implemented in this change - they are foundation for future use:
- Filtering queries by IsDeleted = false
- Hard delete vs soft delete API endpoints
- Restore deleted entities

This is a foundation-only change. The actual soft delete query filtering will be implemented in separate changes when needed.