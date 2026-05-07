## 1. Sửa cấu hình Database Path

- [x] 1.1 Sửa appsettings.json: đổi `Data Source=ReadNestDb.db` thành `Data Source=./data/ReadNestDb.db`
- [x] 1.2 Thêm biến môi trường `ConnectionStrings__DefaultConnection=/app/data/ReadNestDb.db` trong docker-compose.yml

## 2. Verify

- [x] 2.1 Build Docker image mới
- [x] 2.2 Test khởi động container: kiểm tra không có lỗi "no such table: Images"