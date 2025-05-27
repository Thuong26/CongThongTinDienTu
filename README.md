# Student Service Portal

## Yêu cầu hệ thống
- .NET 8.0 SDK
- Docker (nếu deploy bằng Docker)
- SQL Server

## Cài đặt và chạy locally

1. Clone repository:
```bash
git clone [repository-url]
cd StudentServicePortal
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Chạy ứng dụng:
```bash
dotnet run
```

## Deploy lên Railway

1. Đăng ký tài khoản Railway tại https://railway.app/

2. Cài đặt Railway CLI (tùy chọn):
```bash
npm i -g @railway/cli
```

3. Login vào Railway:
```bash
railway login
```

4. Link project với repository:
```bash
railway link
```

5. Deploy project:
```bash
railway up
```

## Cấu hình biến môi trường trên Railway

Thêm các biến môi trường sau trong Railway Dashboard:

- `ASPNETCORE_ENVIRONMENT`: Production
- `ConnectionStrings__DefaultConnection`: [Railway Database Connection String]
- `Jwt__Secret`: [Your JWT Secret]
- `Jwt__Issuer`: [Your Railway Domain]
- `Jwt__Audience`: [Your Railway Domain]
- `EmailSettings__SmtpServer`: [Your SMTP Server]
- `EmailSettings__SmtpPort`: 587
- `EmailSettings__SenderEmail`: [Your Email]
- `EmailSettings__SenderPassword`: [Your Email Password]
- `EmailSettings__EnableSSL`: true

## Cấu trúc thư mục

- `Controllers/`: Chứa các API controllers
- `Models/`: Chứa các model classes
- `Services/`: Chứa business logic
- `Repositories/`: Chứa data access logic
- `Data/`: Chứa database context và migrations
- `Configurations/`: Chứa các file cấu hình
- `Middlewares/`: Chứa các middleware components
- `Utils/`: Chứa các utility functions

## License

[Your License] 