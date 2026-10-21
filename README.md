# CRM — Quản lý bán hàng và kho

Project 1, Giai đoạn 1 (Tuần 1-3) của lộ trình tự học backend .NET — **đã hoàn thành** (xem phần [Trạng thái](#trạng-thái) bên dưới cho các khoản nợ kỹ thuật còn lại). Xem bối cảnh nghiệp vụ đầy đủ ở [docs/2-tai-lieu-nghiep-vu.md](docs/2-tai-lieu-nghiep-vu.md), kế hoạch chi tiết từng ngày ở [docs/1-ke-hoach-chi-tiet.md](docs/1-ke-hoach-chi-tiet.md).

## Tech stack
- .NET 10, ASP.NET Core Web API
- PostgreSQL 16 (Alpine), chạy qua Docker Compose
- EF Core (Npgsql) — migration tự áp dụng khi app khởi động (`Database.MigrateAsync()`)
- ASP.NET Core Identity + JWT (access token + refresh token rotation) + permission-based authorization (policy provider động, đọc từ DB)
- FluentValidation
- Clean Architecture: Domain / Application / Infrastructure / Api

## Kiến trúc

```
src/
  CRM.Domain          # Entity thuần, không phụ thuộc layer nào khác
  CRM.Application      # Interface, DTO, Service (use case), Validator — phụ thuộc Domain
  CRM.Infrastructure   # EF Core, AppDbContext, Identity, Fluent config, migrations — implement interface của Application
  CRM.Api              # Controllers, Program.cs (composition root)
```
Chiều phụ thuộc luôn hướng vào trong: `Api → Infrastructure → Application → Domain`. Chi tiết lý do xem phần tổng kết Tuần 1 trong [docs/1-ke-hoach-chi-tiet.md](docs/1-ke-hoach-chi-tiet.md).

Sơ đồ quan hệ dữ liệu đầy đủ (kể cả Identity/permission và StockMovement): [docs/erd.md](docs/erd.md).

## Chạy project

### Cách 1 — Docker Compose (khuyến nghị, chạy end-to-end, không cần cài .NET SDK)

```bash
docker compose up -d
```

Lên cả 2 container: `crm-postgres` (cổng host `5435`) và `crm-api` (cổng host `8090`). Migration tự chạy khi `crm-api` khởi động — kể cả volume Postgres hoàn toàn trống, không cần tự `dotnet ef database update` tay. App seed sẵn role/permission + 1 tài khoản Admin (`admin@crm.local` / `Admin@12345`) và dữ liệu mẫu (Category/Supplier/Customer/Product).

- API: `http://localhost:8090`
- Scalar UI (thử API trực tiếp trên trình duyệt, có nút Authorize dán Bearer token): `http://localhost:8090/scalar/v1`
- Health check: `http://localhost:8090/health`

> Cổng `8090`/`5435` được chọn vì máy dev thường đã có service khác chiếm `8080`/`5432` — đổi lại trong `docker-compose.yml` nếu máy bạn trống các cổng mặc định.

### Cách 2 — Chạy local bằng `dotnet run` (để code/debug)

1. Khởi động riêng Postgres: `docker compose up -d postgres`
2. Áp migration (lần đầu hoặc khi có migration mới):
   ```bash
   dotnet ef database update --project src/CRM.Infrastructure --startup-project src/CRM.Api
   ```
3. Chạy API: `dotnet run --project src/CRM.Api` — connection string lấy từ `src/CRM.Api/appsettings.Development.json` (đã khớp cổng `5435` của Postgres).

## Luồng dùng thử nhanh

```bash
# 1. Đăng nhập lấy access token
curl -X POST http://localhost:8090/api/auth/login -H "Content-Type: application/json" \
  -d '{"email":"admin@crm.local","password":"Admin@12345"}'

# 2. Dùng accessToken ở bước 1 cho mọi request cần quyền
curl http://localhost:8090/api/products -H "Authorization: Bearer <accessToken>"
```
Hoặc dùng Scalar UI (`/scalar/v1`) — nút "Authorize" chỉ cần dán thẳng access token, không cần gõ chữ `Bearer`.

## Endpoint hiện có

### Auth (`/api/auth`) — không cần token
| Route | Method | Ghi chú |
|---|---|---|
| `/register` | POST | Tạo tài khoản, tự gán role `Staff` |
| `/login` | POST | Trả access token (JWT, 60 phút) + refresh token (7 ngày) |
| `/refresh` | POST | Đổi refresh token cũ lấy cặp token mới (rotate), phát hiện reuse |
| `/logout` | POST | Thu hồi refresh token |
| `/me` | GET | Cần token — xem claims trong token hiện tại |

### Quản trị role/permission/user (cần token + đúng permission)
| Resource | Routes | Permission |
|---|---|---|
| Roles | `GET /api/roles`, `GET /{id}`, `POST` | `role.manage` |
| Role permissions | `POST /api/roles/{id}/permissions`, `DELETE /{id}/permissions/{name}` | `permission.manage` |
| Permissions | `GET /api/permissions` | `permission.manage` |
| Users | `GET /api/users`, `POST /{id}/roles`, `DELETE /{id}/roles/{roleName}` | `user.manage` |

### Dữ liệu nền tảng
| Resource | Routes | Permission (view / manage) | Ghi chú |
|---|---|---|---|
| Products | `GET /api/products` (paging+filter+sort), `GET /{id}`, `POST`, `PUT /{id}`, `DELETE /{id}` | công khai (GET) / `product.create`, `product.update`, `product.delete` | Filter: `categoryId`, `name`; sort: `sortBy=name\|price` |
| Categories | `GET /api/categories`, `GET /{id}`, `POST`, `PUT /{id}`, `DELETE /{id}` | `category.view` / `category.manage` | |
| Suppliers | `GET /api/suppliers`, `GET /{id}`, `POST`, `PUT /{id}`, `DELETE /{id}` | `supplier.view` / `supplier.manage` | |
| Customers | `GET /api/customers` (paging+filter+sort), `GET /{id}`, `POST`, `PUT /{id}`, `DELETE /{id}` | `customer.view` / `customer.manage` | Filter: `name`; sort: `sortBy=name\|email` |

### Đơn hàng và kho
| Resource | Routes | Permission | Ghi chú |
|---|---|---|---|
| Orders | `POST /api/orders` | `order.create` | Transaction, trừ kho, check đủ hàng, concurrency-safe (409 nếu đụng request khác) |
| Orders | `POST /api/orders/{id}/cancel` | `order.cancel` | Chỉ hủy được khi đang `Pending`, hoàn kho |
| Stock movements | `POST /api/products/{id}/stock-movements/import`, `POST .../adjustment` | `stockmovement.manual` | Nhập/điều chỉnh kho thủ công, không qua Order, chặn làm âm kho |
| Stock movements | `GET /api/products/{id}/stock-movements` | `stockmovement.view` | Paging, filter theo `type`/`from`/`to` |

Tất cả response lỗi (400/validation, 500/unhandled) đều theo format [`ProblemDetails`](https://datatracker.ietf.org/doc/html/rfc7807) — xem `CRM.Api/Middleware/GlobalExceptionHandler.cs`. Lỗi nghiệp vụ (ví dụ "không đủ hàng") trả `400`/`409` kèm message rõ ràng thay vì exception chung.

## Trạng thái

Cả 3 tuần kế hoạch gốc (cấu trúc/CRUD, auth/permission, đơn hàng/kho) đã hoàn thành về mặt tính năng và đã verify bằng test thủ công (curl/Scalar, kể cả kịch bản 2 request đồng thời tranh chấp tồn kho). Chi tiết từng ngày xem [docs/1-ke-hoach-chi-tiet.md](docs/1-ke-hoach-chi-tiet.md), tiến độ tổng ở `dotnet-roadmap.md` (thư mục gốc repo học tập).

**Nợ kỹ thuật mang sang Project 2** (quyết định có chủ đích, không phải thiếu sót bị bỏ quên):
- **Không có unit test / Serilog** — bỏ qua Ngày 19-20 theo quyết định của người học vì không ảnh hưởng nghiệp vụ; cả 2 sẽ làm ở Project 2 nếu cần. Vì vậy "có test" trong tiêu chí hoàn thành gốc ở `dotnet-roadmap.md` **chưa đạt** — ghi nhận công khai thay vì coi như đã xong.
- Đang dùng `Console`/log mặc định của ASP.NET Core, chưa có structured logging hay log ra file.
