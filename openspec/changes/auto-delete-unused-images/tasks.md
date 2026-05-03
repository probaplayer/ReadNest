## 1. Model Changes

- [x] 1.1 Add `ExpiresAt` property to Image model in ReadNest-Model/Models/Image.cs
- [x] 1.2 Add `IsDeleted` property to BaseModel in ReadNest-Model/Models/BaseModel.cs
- [x] 1.3 Create EF Core migration to add ExpiresAt column to Images table and IsDeleted to all tables

## 2. Repository Layer

- [x] 2.1 Update ImageRepository to support updating ExpiresAt (set null when image is used)
- [x] 2.2 Add method to save failed deletion records to ImageCleanupFailures table
- [x] 2.3 Add method to get and clean up failed deletion records

## 3. Service Layer

- [x] 3.1 Update FileService.UploadImageAsync to set ExpiresAt = DateTime.Now + 15 minutes
- [x] 3.2 Add logic to clear ExpiresAt when image is assigned to Novel (in NovelRepository Create/Update)
- [x] 3.3 Add logic to clear ExpiresAt when image is assigned to Volumn (in VolumnRepository)
- [x] 3.4 Add logic to clear ExpiresAt when image is assigned to Content (in ContentRepository)

## 4. ScheduleService Updates

- [x] 4.1 Update cleanup query to delete images where ExpiresAt IS NOT NULL AND ExpiresAt < DateTime.Now
- [x] 4.2 Add retry logic: check ImageCleanupFailures table for previously failed deletions
- [x] 4.3 Add retry success handling: clean up failure records when image is deleted
- [x] 4.4 Update email notification to include retry status

## 5. Testing & Validation

- [ ] 5.1 Run database migration
- [ ] 5.2 Test upload image sets ExpiresAt correctly
- [ ] 5.3 Test creating Novel with image clears ExpiresAt
- [ ] 5.4 Test cleanup deletes expired images
- [ ] 5.5 Test retry mechanism works

## 6. Documentation

- [ ] 6.1 Update API documentation if needed
- [ ] 6.2 Document the 15-minute expiration behavior