## Context

Khi chạy ứng dụng trong Docker:
- WORKDIR của container là `/app`
- Volume mapping: `./data/sqlite:/app/data` (máy host → container)
- Connection string hiện tại: `Data Source=ReadNestDb.db` → trỏ đến `/app/ReadNestDb.db`
- Database thực tế nằm ở: `/app/data/ReadNestDb.db`

## Goals / Non-Goals

**Goals:**
- Đảm bảo ứng dụng truy cập đúng database file khi chạy trong Docker
- Đảm bảo vẫn hoạt động bình thường khi chạy local (khôngquaDocker)

**Non-Goals:**
- Không thay đổi cấu trúc database
- Không migration dữ liệu

## Decisions

1. **Sửa appsettings.json**: Thay connection string thành `./data/ReadNestDb.db`
   - **Rationale**: Đường dẫn tương đối `./data/` sẽ tạo database cùng vị trí với volume mapping
   - **Alternative considered**: Hardcode đường dẫn tuyệt đối `/app/data/ReadNestDb.db` → bị rejected vì không linh hoạt

2. **Override bằng biến môi trường trong docker-compose.yml**:
   - **Rationale**: Đảm bảo rõ ràng, dễ debug, không phụ thuộc vào config file trong image

## Risks / Trade-offs

- [Risk] Database path không đúng khi chạy local → [Mitigation] Đường dẫn `./data/ReadNestDb.db` hoạt động cả local (Relative to working directory) và Docker

## Migration Plan

1. Build lại Docker image với appsettings.json mới
2. Khởi động lại container (docker-compose up --build)
3. Database cũ từ `./data/sqlite/ReadNestDb.db` sẽ được mount đúng cách

## Open Questions

- (none)