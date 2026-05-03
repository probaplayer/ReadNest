## ADDED Requirements

### Requirement: Image expires after 15 minutes if not used
When an image is uploaded but not assigned to any entity (Novel/Volumn/Content) within 15 minutes, the system SHALL mark it as expired and include it in the next cleanup cycle.

#### Scenario: Uploaded image expires after 15 minutes
- **WHEN** an image is uploaded via /api/images/upload with no entity assignment within 15 minutes
- **THEN** the image's ExpiresAt timestamp is in the past
- **AND** the image SHALL be deleted by the cleanup service in the next cycle

### Requirement: Image is kept when assigned to an entity
When an image is assigned to a Novel, Volumn, or Content, the system SHALL clear the ExpiresAt timestamp to prevent deletion.

#### Scenario: Novel cover image is kept
- **WHEN** user creates a Novel with an uploaded image as cover
- **THEN** the image's ExpiresAt field SHALL be set to null
- **AND** the image SHALL NOT be deleted by cleanup service

#### Scenario: Volumn cover image is kept
- **WHEN** user creates a Volumn with an uploaded image as cover
- **THEN** the image's ExpiresAt field SHALL be set to null
- **AND** the image SHALL NOT be deleted by cleanup service

#### Scenario: Content image is kept
- **WHEN** user creates Content with an uploaded image
- **THEN** the image's ExpiresAt field SHALL be set to null
- **AND** the image SHALL NOT be deleted by cleanup service

### Requirement: Expired images are automatically deleted
The cleanup service SHALL run periodically and delete all images where ExpiresAt IS NOT NULL AND ExpiresAt < DateTime.Now.

#### Scenario: Cleanup deletes expired images
- **WHEN** cleanup service runs
- **THEN** it SHALL find all images where ExpiresAt IS NOT NULL AND ExpiresAt < DateTime.Now
- **AND** delete the image file from disk
- **AND** delete the image record from database