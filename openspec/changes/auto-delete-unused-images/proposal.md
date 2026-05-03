## Why

Hiện tại hệ thống upload ảnh lên trước khi tạo novel, nếu user refresh trang hoặc tạo novel thất bại sẽ có ảnh "mồ côi" lưu trong DB và thư mục Uploads nhưng ko dùng đến. ScheduleService chạy mỗi 5 giờ nhưng chỉ xóa ảnh hoàn toàn ko dùng - chưa có cơ chế kiểm tra ảnh "tạm" chưa được gán vào entity nào trong khoảng thời gian ngắn.

## What Changes

1. Thêm trường `ExpiresAt` vào Image model - timestamp cho biết ảnh hết hạn
2. Khi upload ảnh, set `ExpiresAt = DateTime.Now + 15 phút`
3. Khi ảnh được gán vào Novel/Volumn/Content, set `ExpiresAt = null` (đánh dấu đang dùng)
4. ScheduleService xóa các ảnh có `ExpiresAt IS NOT NULL AND ExpiresAt < DateTime.Now` (đã hết hạn và chưa dùng)
5. Thêm retry logic cho việc xóa ảnh thất bại
6. Thêm soft delete (`IsDeleted`) vào BaseModel để chuẩn bị cho tương lai

## Capabilities

### New Capabilities
- **image-expiration**: Tự động xóa ảnh tạm chưa dùng sau 15 phút
- **image-cleanup-retry**: Retry xóa ảnh thất bại khi cleanup lần sau
- **soft-delete**: Thêm IsDeleted vào BaseModel cho các entity

### Modified Capabilities
- (none)

## Impact

- **Models**: Image.cs - thêm ExpiresAt; BaseModel.cs - thêm IsDeleted
- **Services**: FileService.cs - gán ExpiresAt khi upload; ScheduleService.cs - cập nhật query; các Repository khi tạo/cập nhật Novel/Volumn/Content set ExpiresAt = null
- **Database**: Thêm cột ExpiresAt vào Images, IsDeleted vào các bảng