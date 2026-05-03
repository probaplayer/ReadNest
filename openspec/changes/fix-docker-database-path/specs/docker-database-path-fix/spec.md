## ADDED Requirements

### Requirement: Docker container truy cập đúng database file
Hệ thống SHALL truy cập đúng file database khi chạy trong Docker container, và SHALL không gây lỗi "no such table: Images".

#### Scenario: Docker khởi động với đường dẫn đúng
- **KHI** Docker container khởi động
- **THEN** ứng dụng kết nối database tại `/app/data/ReadNestDb.db` (thông qua volume mapping)
- **AND** không có lỗi "no such table: Images"

#### Scenario: ScheduleService truy vấn Images table
- **KHI** ScheduleService.ExecuteAsync chạy
- **THEN** truy vấn thực hiện thành công trên bảng Images
- **AND** không có exception SQLite Error 1

#### Scenario: Local development vẫn hoạt động
- **KHI** ứng dụng chạy local (không qua Docker)
- **THEN** database được tạo/tìm ở `./data/ReadNestDb.db`
- **AND** tất cả chức năng hoạt động bình thường