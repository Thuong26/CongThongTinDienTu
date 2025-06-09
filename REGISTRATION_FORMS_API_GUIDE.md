# Hướng dẫn sử dụng API Đơn đăng ký theo Phòng ban

## Các API mới đã được thêm

### 1. Lấy tất cả đơn đăng ký theo phòng ban (từ token)
```
GET /api/staff/forms/department
```
**Mô tả:** Lấy danh sách tất cả đơn đăng ký (cả đã xử lý và chưa xử lý) thuộc phòng ban của cán bộ đang đăng nhập.

**Response:**
- Trả về danh sách đơn đăng ký có thông tin phòng ban
- Sắp xếp theo thời gian đăng mới nhất trước
- Bao gồm cả TenPB (tên phòng ban)

### 2. Lấy đơn đăng ký chờ xử lý theo phòng ban (từ token)
```
GET /api/staff/forms/department/pending
```
**Mô tả:** Lấy danh sách chỉ những đơn đăng ký đang chờ xử lý (TrangThai = 1) thuộc phòng ban của cán bộ đang đăng nhập.

**Response:**
- Chỉ trả về đơn có TrangThai = 1 (đang xử lý)
- Lọc theo phòng ban của cán bộ
- Sắp xếp theo thời gian đăng mới nhất trước

### 3. Lấy tất cả đơn đăng ký theo mã phòng ban
```
GET /api/staff/forms/by-department/{maPB}
```
**Mô tả:** Lấy danh sách tất cả đơn đăng ký thuộc phòng ban được chỉ định qua parameter.

**Parameters:**
- `maPB` (path): Mã phòng ban (required)

**Response:**
- Trả về tất cả đơn đăng ký của phòng ban chỉ định
- Sắp xếp theo thời gian đăng mới nhất trước

### 4. Lấy đơn đăng ký chờ xử lý theo mã phòng ban
```
GET /api/staff/forms/by-department/{maPB}/pending
```
**Mô tả:** Lấy danh sách các đơn đăng ký đang chờ xử lý thuộc phòng ban được chỉ định qua parameter.

**Parameters:**
- `maPB` (path): Mã phòng ban (required)

**Response:**
- Chỉ trả về đơn có TrangThai = 1 (đang xử lý)
- Lọc theo mã phòng ban được truyền vào
- Sắp xếp theo thời gian đăng mới nhất trước

### 5. API hiện tại (đã có sẵn)
```
GET /api/staff/forms
```
**Mô tả:** Lấy tất cả đơn đăng ký chờ xử lý trong toàn hệ thống (không lọc theo phòng ban).

## So sánh các API

| API | Lọc phòng ban | Lọc trạng thái | Cách lấy mã phòng ban | Mục đích sử dụng |
|-----|---------------|----------------|----------------------|------------------|
| `/api/staff/forms` | ❌ | ✅ (chỉ pending) | - | Xem tất cả đơn chờ xử lý hệ thống |
| `/api/staff/forms/department` | ✅ | ❌ | Từ token cán bộ | Xem tất cả đơn của phòng ban hiện tại |
| `/api/staff/forms/department/pending` | ✅ | ✅ (chỉ pending) | Từ token cán bộ | Xem đơn chờ xử lý của phòng ban hiện tại |
| `/api/staff/forms/by-department/{maPB}` | ✅ | ❌ | Từ parameter | Xem tất cả đơn của phòng ban chỉ định |
| `/api/staff/forms/by-department/{maPB}/pending` | ✅ | ✅ (chỉ pending) | Từ parameter | Xem đơn chờ xử lý của phòng ban chỉ định |

## Ví dụ sử dụng

### Lấy đơn đăng ký của phòng ban "PB001":
```
GET /api/staff/forms/by-department/PB001
```

### Lấy đơn chờ xử lý của phòng ban "PB002":
```
GET /api/staff/forms/by-department/PB002/pending
```

## Lưu ý
- Tất cả API đều yêu cầu JWT token hợp lệ
- API với parameter `{maPB}` cho phép xem đơn của bất kỳ phòng ban nào
- API không có parameter chỉ lấy đơn của phòng ban từ token cán bộ đăng nhập
- API trả về mảng rỗng nếu không có đơn nào
- Response format nhất quán với các API khác trong hệ thống 