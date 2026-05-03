## ADDED Requirements

### Requirement: Failed image deletions are retried
When an image deletion fails (file lock, permission error, etc), the system SHALL record the failure and retry in the next cleanup cycle.

#### Scenario: Failed deletion is retried
- **WHEN** cleanup service attempts to delete an image but the file operation fails
- **THEN** the failure SHALL be logged in the ImageCleanupFailures table
- **AND** the image SHALL be retried in the next cleanup cycle

### Requirement: Cleanup service skips retried images that already succeeded
If an image deletion previously failed but the file no longer exists (manually cleaned or external process), the service SHALL delete the failed record and not report as error.

#### Scenario: Image already deleted externally
- **WHEN** cleanup service finds a failed deletion record for an image that no longer exists on disk
- **THEN** it SHALL delete the failure record from the table
- **AND** log this as information (not error)