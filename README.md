# CRM — Quản lý bán hàng và kho

Project 1, Giai đoạn 1 (Tuần 1-3) của lộ trình tự học backend .NET. Xem bối cảnh đầy đủ ở [docs/2-tai-lieu-nghiep-vu.md](docs/2-tai-lieu-nghiep-vu.md), kế hoạch chi tiết từng ngày ở [docs/1-ke-hoach-chi-tiet.md](docs/1-ke-hoach-chi-tiet.md).

## Tech stack
- .NET 10, ASP.NET Core Web API
- PostgreSQL 16 (Alpine), chạy qua Docker Compose
- EF Core (Npgsql)
- FluentValidation
- Clean Architecture: Domain / Application / Infrastructure / Api

## Kiến trúc

```
src/
  CRM.Domain          # Entity thuần, không phụ thuộc layer nào khác
  CRM.Application      # Interface, DTO, Service (use case), Validator — phụ thuộc Domain
  CRM.Infrastructure   # EF Core, AppDbContext, Fluent config, migrations — implement interface của Application
  CRM.Api              # Controllers, Program.cs (composition root)
```
Chiều phụ thuộc luôn hướng vào trong: `Api → Infrastructure → Application → Domain`. Chi tiết lý do xem phần tổng kết Tuần 1 trong [docs/1-ke-hoach-chi-tiet.md](docs/1-ke-hoach-chi-tiet.md).

Sơ đồ quan hệ dữ liệu: [docs/erd.md](docs/erd.md).

## Chạy project

1. Khởi động PostgreSQL:
   ```bash
   docker compose up -d
   ```
   Container `crm-postgres` lắng nghe ở cổng host `5435` (map vào `5432` trong container) — xem lý do ở `docker-compose.yml`.

2. Áp dụng migration (lần đầu hoặc khi có migration mới):
   ```bash
   cd src/CRM.Api
   dotnet ef database update --project ../CRM.Infrastructure
   ```

3. Chạy API:
   ```bash
   dotnet run --project src/CRM.Api
   ```
   Ở môi trường Development, app tự seed dữ liệu mẫu (Category/Supplier/Customer/Product) nếu bảng `Categories` đang rỗng — xem `CRM.Infrastructure/Persistence/DbInitializer.cs`.

Connection string mặc định (`src/CRM.Api/appsettings.Development.json`) đã khớp với cấu hình trong `docker-compose.yml` — không cần chỉnh gì nếu chạy đúng theo 2 bước trên.

## API docs

Khi chạy ở Development:
- OpenAPI spec (JSON thô): `GET /openapi/v1.json`
- UI tương tác (Scalar): `/scalar/v1`

## Endpoint hiện có

| Resource | Routes | Ghi chú |
|---|---|---|
| Products | `GET /api/products` (paging+filter+sort), `GET /api/products/{id}`, `POST`, `PUT /{id}`, `DELETE /{id}` | Filter: `categoryId`, `name`; sort: `sortBy=name\|price`, `sortDescending` |
| Categories | `GET /api/categories`, `GET /{id}`, `POST`, `PUT /{id}`, `DELETE /{id}` | CRUD thuần, chưa paging |
| Suppliers | `GET /api/suppliers`, `GET /{id}`, `POST`, `PUT /{id}`, `DELETE /{id}` | CRUD thuần, chưa paging |
| Customers | `GET /api/customers` (paging+filter+sort), `GET /{id}`, `POST`, `PUT /{id}`, `DELETE /{id}` | Filter: `name`; sort: `sortBy=name\|email`, `sortDescending` |

Tất cả response lỗi (400/validation, 500/unhandled) đều theo format [`ProblemDetails`](https://datatracker.ietf.org/doc/html/rfc7807) — xem `CRM.Api/Middleware/GlobalExceptionHandler.cs`.

Order, OrderItem, User: đã có entity + migration, **chưa có CRUD** — việc của Tuần 2-3 (auth/role/permission trước, rồi đơn hàng).

## Trạng thái

Tuần 1 (cấu trúc + CRUD) đã hoàn thành — chi tiết trong `dotnet-roadmap.md` ở thư mục gốc repo học tập và phần "Tổng kết tuần 1" trong `docs/1-ke-hoach-chi-tiet.md`.
