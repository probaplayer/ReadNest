## Why

Khi chạy ứng dụng trong Docker, database được tạo ở đường dẫn sai (/app/ReadNestDb.db) thay vì đường dẫn đúng (/app/data/ReadNestDb.db). Điều này khiến ứng dụng truy cập database rỗng không có table Images, gây ra lỗi "no such table: Images" khi ScheduleService chạy.

## What Changes

- Sửa connection string mặc định trong appsettings.json thành đường dẫn tương đối `./data/ReadNestDb.db`
- Thêm biến môi trường `ConnectionStrings__DefaultConnection` trong docker-compose.yml để override đường dẫn khi chạy trong container

## Capabilities

### New Capabilities
- docker-database-path-fix: Đảm bảo ứng dụng truy cập đúng database file khi chạy trong Docker

### Modified Capabilities
- (none)

## Impact

- appsettings.json: Thay đổi connection string mặc định
- docker-compose.yml: Thêm biến môi trường cho đường dẫn database