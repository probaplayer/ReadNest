## Context

Hiện tại khi upload ảnh, image được lưu vào DB ngay lập tức nhưng có thể chưa được gán vào Novel/Volumn/Content (thường là cover image). Nếu user:
- Refresh trang trước khi submit form tạo novel
- Tạo novel thất bại (validation error, network error,...)
→ Ảnh "mồ côi" sẽ tồn tại trong DB và thư mục Uploads nhưng ko dùng đến.

ScheduleService hiện tại chạy 5 tiếng/lần, chỉ xóa ảnh ko tham chiếu bởi bất kỳ entity nào - nhưng nếu ảnh đó mới upload và chưa kịp gán (vd: mới 1 phút trước) thì vẫn bị xóa nhầm.

## Goals / Non-Goals

**Goals:**
- Xóa ảnh tạm chưa dùng sau 15 phút
- Giữ ảnh đang được sử dụng (đã gán vào Novel/Volumn/Content)
- Retry xóa ảnh thất bại (file lock, permission,...)
- Thêm soft delete foundation cho tương lai

**Non-Goals:**
- Thay đỏi cách upload ảnh hiện tại (vẫn upload trước)
- Real-time cleanup (vẫn qua ScheduleService)
- Fullsoft delete implementation cho tất cả entities

## Decisions

### 1. Expiration timestamp (`ExpiresAt`)

**Chọn:** Thêm `ExpiresAt` vào Image model

**Rationale:**
- Simple, clear semantics: "ảnh này hết hạn lúc..."
- Dễ query: `WHERE ExpiresAt IS NOT NULL AND ExpiresAt < NOW`
- Khi gán vào entity → set `ExpiresAt = null` để "giữ lại"

**Alternative considered:**
- `LastAccessedDate` - cần cập nhật mỗi lần user xem → performance hit ❌
- Separate tracking table - phức tạp hơn cần thiết ❌

### 2. 15 phút expiration

**Chọn:** 15 phút (theo yêu cầu user)

**Rationale:**
- Đủ thời gian để user hoàn thành form tạo novel
- Ko quá lâu để ảnh "mồ côi" chiếm dung lượng

### 3. Cleanup logic

**Chọn:** Xóa các ảnh có `ExpiresAt IS NOT NULL AND ExpiresAt < DateTime.Now`

**Rationale:**
- Chỉ xóa ảnh đã "hết hạn" - đảm bảo user có đủ 15p
- Ko xóa ảnh đang dùng (ExpiresAt = null)

**Alternative:**
- Giữ logic cũ + thêm expiration check → phức tạp, redundancy ❌

### 4. Retry mechanism

**Chọn:** Lưu failed deletions vào DB table riêng, retry lần sau

**Rationale:**
- Ko rely vào in-memory retry (service restart sẽ mất)
- Retry sau 5 tiếng (khi ScheduleService chạy lại) là acceptable

**Alternative considered:**
- Polly/RetryPolicy - thêm dependency, overkill cho cleanup task ❌

### 5. Soft delete foundation

**Chọn:** Thêm `IsDeleted` vào BaseModel, chưa apply vào query

**Rationale:**
- Preparation cho tương lai
- Ko thay đổi behavior hiện tại
- Khi cần thì chỉ cần thêm WHERE IsDeleted = false vào queries

## Risks / Trade-offs

| Risk | Mitigation |
|------|------------|
| User mất ảnh vừa upload chưa kịp gán | 15 phút đủ thời gian, có thể tăng lên |
| DB migration fail khi thêm cột | Tạo migration riêng, test trước deploy |
| File lock khi xóa | Retry mechanism qua DB table |
| Null reference khi gán image | Check null trước khi set ExpiresAt = null |

## Open Questions

- **Q:** Ảnh upload trước khi feature này được deploy sẽ có ExpiresAt = null, có bị xóa nhầm ko?
- **A:** Ko, vì chỉ xóa ảnh có ExpiresAt IS NOT NULL. Ảnh cũ sẽ ko bị ảnh hưởng.

- **Q:** Có nên chạy cleanup thường hơn (vd: mỗi 30p) thay vì 5 tiếng?
- **A:** Hiện tại giữ 5 tiếng cho đơn giản, có thể thay đổi sau nếu cần.