# Danh sách kiến thức cần biết — Project 1: Quản lý bán hàng và kho

> Tài liệu liên quan: [1-ke-hoach-chi-tiet.md](1-ke-hoach-chi-tiet.md) (kế hoạch theo ngày), [2-tai-lieu-nghiep-vu.md](2-tai-lieu-nghiep-vu.md) (nghiệp vụ).
> Mỗi mục ghi kèm *vì sao cần* để không học tràn lan — chỉ học đủ dùng cho ngày đó, đào sâu hơn khi gặp lại ở project sau.

## Tuần 1: Cấu trúc và CRUD

### Ngày 1 — Khởi tạo solution
- [ ] Cấu trúc solution .NET nhiều project, `dotnet new sln/classlib/webapi`, `dotnet sln add`
- [ ] Clean Architecture / layered architecture: vai trò của Domain, Application, Infrastructure, Api và vì sao Domain không phụ thuộc project nào khác
- [ ] Project reference (`dotnet add reference`) và quy tắc chiều phụ thuộc
- [ ] `Program.cs` tối giản (minimal hosting model), `launchSettings.json`, `appsettings.json` vs `appsettings.Development.json`
- [ ] Health check cơ bản (`AddHealthChecks`/endpoint đơn giản)
- [ ] Git cơ bản: init, `.gitignore` cho .NET, commit

### Ngày 2 — PostgreSQL + EF Core
- [ ] Docker Compose cơ bản: `services`, `environment`, `ports`, `volumes` (named volume), `healthcheck` — vì sao khai báo trong file thay vì gõ tay `docker run` mỗi lần (tái lập được, versioned cùng code)
- [ ] Dockerfile multi-stage build (stage SDK để build/publish, stage runtime để chạy) — hiểu khái niệm trước, chưa cần dùng ngay (tạo sẵn cho Ngày 21)
- [ ] Connection string PostgreSQL, Npgsql provider
- [ ] EF Core: `DbContext`, `DbSet<T>`, đăng ký qua `AddDbContext`
- [ ] Migration: `dotnet ef migrations add`, `dotnet ef database update`, ý nghĩa bảng `__EFMigrationsHistory`
- [ ] Vì sao không hardcode connection string (đọc từ configuration, user-secrets khi cần)

### Ngày 3 — Entity, quan hệ, ERD
- [ ] Quan hệ 1-n trong EF Core (navigation property 1 chiều/2 chiều, FK convention vs Fluent API)
- [ ] Quan hệ n-n qua bảng nối rõ ràng (join entity) — khác với n-n skip-navigation tự động của EF Core, và vì sao project này chọn join entity tường minh (OrderItem có thêm field Quantity/UnitPrice, không thể dùng n-n ẩn)
- [ ] `IEntityTypeConfiguration<T>` + Fluent API (required, max length, unique index, FK behavior)
- [ ] Công cụ vẽ ERD (dbdiagram.io hoặc draw.io) — ký hiệu 1-n, n-n cơ bản
- [ ] Khái niệm chuẩn hóa dữ liệu (normalization) ở mức cơ bản để biết vì sao tách bảng

### Ngày 4 — Seed data
- [ ] Seed qua `HasData` trong migration (giới hạn: cần giá trị PK cố định) vs seed runtime qua code (chạy lúc start, idempotent)
- [ ] Index trong EF Core (`HasIndex`, `IsUnique`)
- [ ] Cách rollback/migration lại từ đầu khi seed sai (`dotnet ef database drop`, update lại)

### Ngày 5 — DTO, mapping, validate
- [ ] Vì sao không trả Entity trực tiếp ra API (rò rỉ cấu trúc DB, vòng lặp navigation property khi serialize)
- [ ] Mapping: AutoMapper hoặc Mapster (profile/config cơ bản) hoặc map tay — ưu/nhược điểm từng cách
- [ ] FluentValidation: viết Validator, tích hợp vào pipeline (filter hoặc gọi tay trong action)
- [ ] Routing & model binding trong Controller (hoặc Minimal API nếu chọn hướng đó) — route param, query param, body

### Ngày 6 — Paging, filter, sort
- [ ] `IQueryable` composition (Where/OrderBy/Skip/Take) — vì sao gọi `ToListAsync()` cuối cùng, không gọi giữa chừng
- [ ] Pattern paging chung (request: page/pageSize, response: items + totalCount)
- [ ] Sort động cơ bản (switch theo field name, hoặc dynamic LINQ nếu muốn thử)
- [ ] Swagger/OpenAPI: cách khai báo, test API qua Swagger UI

### Ngày 7 — Exception handling, N+1
- [ ] `IExceptionHandler` (ASP.NET Core 8+) hoặc middleware xử lý exception toàn cục
- [ ] `ProblemDetails` (RFC 7807) — cấu trúc chuẩn, cách tùy biến field
- [ ] N+1 query là gì (lazy load từng navigation property trong loop) và cách phát hiện bằng log SQL
- [ ] `Include()` (eager loading) vs `Select()` projection thẳng sang DTO — khi nào dùng cái nào
- [ ] `AsNoTracking()` cho query chỉ đọc (vì sao giảm overhead change tracking)

## Tuần 2: Auth, role, permission

### Ngày 8 — ASP.NET Identity
- [ ] Kiến trúc Identity: `UserManager`, `RoleManager`, `SignInManager`, `IdentityUser`/`IdentityRole`
- [ ] Cách tích hợp Identity vào `DbContext` đã có sẵn (thay vì dùng `IdentityDbContext` riêng)
- [ ] Cơ chế hash password của Identity ở mức hiểu khái niệm (không cần tự implement)
- [ ] Migration cho bảng Identity

### Ngày 9 — JWT access token
- [ ] Cấu trúc JWT: header, payload (claims), signature; claims chuẩn (sub, exp) và claims tự định nghĩa
- [ ] `Microsoft.AspNetCore.Authentication.JwtBearer`: cấu hình `TokenValidationParameters` (issuer, audience, signing key, lifetime)
- [ ] Phát token: `JwtSecurityTokenHandler`, `SymmetricSecurityKey`, `SigningCredentials`
- [ ] `[Authorize]` cơ bản, `HttpContext.User` để đọc claims

### Ngày 10 — Refresh token
- [ ] Vì sao cần refresh token tách riêng access token (access token sống ngắn, refresh token sống dài nhưng kiểm soát được)
- [ ] Lưu refresh token: hash trước khi lưu DB (không lưu plain text) — tương tự lý do hash password
- [ ] Rotate: mỗi lần refresh, phát token mới và vô hiệu token cũ
- [ ] Revoke: logout, hoặc phát hiện refresh token cũ bị tái sử dụng sau khi đã rotate (reuse detection) — rủi ro bị đánh cắp token

### Ngày 11 — Role-based authorization
- [ ] Seed role idempotent (kiểm tra tồn tại trước khi tạo) lúc app khởi động
- [ ] Gán role cho user: `UserManager.AddToRoleAsync`
- [ ] `[Authorize(Roles = "...")]` — cách hoạt động, và hạn chế: cứng trong code, không gán nhiều permission linh hoạt theo từng role tùy biến

### Ngày 12 — Thiết kế permission
- [ ] Permission-based authorization là gì, khác role-based ở điểm nào (permission là hành vi cụ thể, role là tập hợp permission)
- [ ] Mô hình n-n Role-Permission trong DB (so với dùng Claims trực tiếp trên user — đánh đổi giữa 2 cách)
- [ ] Seed permission theo ma trận đã thiết kế trong tài liệu nghiệp vụ

### Ngày 13 — Policy provider động
- [ ] `IAuthorizationRequirement` custom, `AuthorizationHandler<TRequirement>`
- [ ] `IAuthorizationPolicyProvider`: vì sao cần khi số lượng policy (permission) nhiều và không muốn khai báo tay từng cái trong `Program.cs`
- [ ] Cách handler đọc permission của user hiện tại (qua role → RolePermission, hoặc cache để tránh query DB mỗi request)
- [ ] Đánh đổi của fallback static policy (đơn giản, nhanh làm nhưng không linh hoạt, phải sửa code + deploy lại khi đổi phân quyền) — hiểu rõ để biết đây là nợ kỹ thuật tạm, không phải giải pháp cuối

### Ngày 14 — Swagger Bearer, API quản lý role/permission
- [ ] Swagger: `AddSecurityDefinition` (scheme Bearer), `AddSecurityRequirement` để UI có nút nhập token
- [ ] Thiết kế API CRUD role + gán permission theo pattern REST thông thường (không có gì đặc biệt mới, áp lại kiến thức tuần 1)

## Tuần 3: Nghiệp vụ đơn hàng và kho

### Ngày 15 — Transaction khi tạo đơn
- [ ] `DbContext.Database.BeginTransactionAsync()` / `ExecuteTransactionAsync` — transaction rõ ràng khi có nhiều `SaveChanges` hoặc nhiều thay đổi logic liên quan
- [ ] Tính nguyên tử (atomicity): vì sao không được để trừ kho xong rồi lỗi ở bước tạo Order (dữ liệu nửa vời)
- [ ] Thiết kế use case/service nghiệp vụ tách khỏi Controller (Controller mỏng, logic nằm ở Application/service layer)

### Ngày 16 — Trạng thái đơn hàng
- [ ] Thiết kế enum trạng thái và các chuyển trạng thái hợp lệ (state machine đơn giản) — vì sao chặn chuyển trạng thái không hợp lệ ngay ở service, không chỉ dựa vào UI
- [ ] Compensating action: hủy đơn là hành động "bù" lại tác động đã xảy ra (hoàn kho), không phải xóa dữ liệu

### Ngày 17 — Optimistic concurrency
- [ ] Optimistic concurrency token trong EF Core (`[ConcurrencyCheck]`/`RowVersion`, hoặc `xmin` tự nhiên của PostgreSQL qua cấu hình)
- [ ] `DbUpdateConcurrencyException`: khi nào ném ra, cách bắt và xử lý (trả 409, hoặc reload + retry)
- [ ] Phân biệt optimistic (giả định ít đụng độ, kiểm tra khi save) vs pessimistic locking (giả định nhiều đụng độ, lock trước) — hiểu khái niệm, chưa cần dùng pessimistic ở project này
- [ ] Cách tự tạo race condition để test (2 request song song) — dùng công cụ gọi API đồng thời (ví dụ 2 tab Postman bắn cùng lúc, hoặc script nhỏ)

### Ngày 18 — StockMovement (ledger pattern)
- [ ] Ý tưởng "sổ cái" (ledger): không sửa trực tiếp số dư mà ghi lại từng giao dịch, số dư là kết quả suy ra được — áp dụng rút gọn ở đây (vẫn lưu `StockQuantity` trực tiếp trên Product để đọc nhanh, nhưng mọi thay đổi đều có movement kèm theo để truy vết)
- [ ] Enum nhiều giá trị nghiệp vụ (Import/Export/Adjustment) và cách query/filter theo enum trong EF Core

### Ngày 19 — Serilog
- [ ] Serilog setup cơ bản: `Serilog.AspNetCore`, sink Console, sink File (rolling interval theo ngày)
- [ ] Structured logging: message template (`"Order {OrderId} created"`) thay vì nối chuỗi — vì sao dễ query log hơn
- [ ] Minimum level theo environment (Debug ở Development, Information/Warning ở Production)

### Ngày 20 — Unit test
- [ ] xUnit cơ bản: `[Fact]`, `[Theory]`, Assert
- [ ] AAA pattern (Arrange-Act-Assert)
- [ ] Mocking dependency (Moq hoặc NSubstitute) để test service nghiệp vụ độc lập khỏi DB thật
- [ ] Test rollback/lỗi nghiệp vụ (ví dụ: không đủ kho → đảm bảo không có side effect nào xảy ra)

### Ngày 21 — Docker Compose, README
- [ ] `depends_on` với `condition: service_healthy` (API chỉ khởi động khi Postgres đã healthy, không chỉ "đã start") — phần mới so với Ngày 2 (lúc đó compose chỉ có 1 service nên chưa cần)
- [ ] Connection string khi API chạy trong container trỏ tới service khác bằng **tên service** (`Host=postgres`) thay vì `localhost` — vì trong mạng nội bộ của Docker Compose, tên service chính là DNS hostname
- [ ] Viết README kỹ thuật: mục đích, cách chạy, cách test, kiến trúc tổng quan — tập thói quen viết tài liệu đi kèm code

## Checklist tổng hợp "phải giải thích được" cuối Project 1

Dùng để tự kiểm tra trước khi qua Project 2 (trùng với mốc kiểm tra cuối Giai đoạn 1 trong `dotnet-roadmap.md`):
- [ ] Vì sao dùng policy-based authorization thay vì chỉ `[Authorize(Roles=...)]`
- [ ] Cách lưu refresh token an toàn (hash, rotate, revoke, reuse detection)
- [ ] N+1 query là gì và cách đã sửa trong project này
- [ ] Optimistic concurrency hoạt động ra sao, khi nào ném `DbUpdateConcurrencyException`
- [ ] Vì sao transaction là bắt buộc khi tạo/hủy đơn hàng, điều gì xảy ra nếu bỏ transaction
