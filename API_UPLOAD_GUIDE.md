# Hướng dẫn sử dụng API Upload File cho Biểu mẫu

## API đã được sửa: `POST /api/staff/templates`

### Thay đổi chính:
- Thay đổi từ `[FromBody]` sang `[FromForm]` để hỗ trợ upload file
- Thêm validation cho file upload
- Tự động lưu file vào thư mục `wwwroot/uploads/forms/`
- Thêm API download file

### Cách sử dụng:

#### 1. Upload biểu mẫu mới với file đính kèm
```
POST /api/staff/templates
Content-Type: multipart/form-data

Form data:
- TenBM: "Tên biểu mẫu" (string)
- File: [file upload] (PDF, DOC, DOCX)
```

#### 2. Tải xuống file biểu mẫu
```
GET /api/staff/templates/{maBM}/download
```

### Giới hạn file:
- Định dạng cho phép: PDF, DOC, DOCX
- Kích thước tối đa: 10MB
- File sẽ được lưu với tên ngẫu nhiên để tránh trùng lặp

### Lưu ý:
- Khi xóa biểu mẫu, file vật lý cũng sẽ bị xóa khỏi server
- File được lưu trong field `LienKet` của model `Form`
- API download sẽ trả về file với tên gốc là tên biểu mẫu 