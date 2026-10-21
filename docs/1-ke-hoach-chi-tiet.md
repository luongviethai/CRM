# Kế hoạch chi tiết — Project 1: Quản lý bán hàng và kho (Tuần 1-3)

> Thuộc Giai đoạn 1 — Nền tảng core. Nguồn gốc: `dotnet-roadmap.md`.
> Tài liệu liên quan: [2-tai-lieu-nghiep-vu.md](2-tai-lieu-nghiep-vu.md) (nghiệp vụ), [3-danh-sach-kien-thuc.md](3-danh-sach-kien-thuc.md) (kiến thức cần học).
>
> Giả định thời gian: **1,5-2 giờ/ngày** các ngày trong tuần (Thứ 2 - Thứ 6), **3-4 giờ/ngày** Thứ 7 - Chủ nhật.
> Cách dùng: đánh dấu `[x]` khi xong việc. Cuối mỗi tuần, làm phần "Tổng kết tuần" trước khi qua tuần kế. Nếu một ngày không đủ thời gian, dời task dư sang hôm sau **trong cùng tuần**; không dời task dư sang tuần sau (tuần sau có task riêng).

## Mục tiêu chung của Project 1

Dựng được một Web API quản lý bán hàng/kho hoàn chỉnh: CRUD nghiệp vụ, auth + phân quyền theo role/permission, nghiệp vụ đơn hàng có transaction và kiểm soát tồn kho, có test, có Docker Compose, có README. Đây là nền tảng sẽ tái sử dụng ở Project 2 và Giai đoạn 2, nên ưu tiên hiểu *tại sao* làm vậy hơn là chỉ chạy được.

**Hoàn thành Project 1 khi:** chạy được bằng `docker compose up`, có unit test cho service nghiệp vụ, có README + ERD.

---

## Tuần 1: Cấu trúc và CRUD

**Mục tiêu tuần:** có solution chạy được, kết nối PostgreSQL qua EF Core, đủ entity + quan hệ theo ERD, và CRUD Product/Category/Supplier hoàn chỉnh (DTO, validate, paging/filter/sort, exception handling, không N+1).

### Ngày 1 (Thứ 2) — Khởi tạo solution
- [ ] Cài .NET SDK bản LTS mới nhất, kiểm tra `dotnet --version`
- [ ] Tạo solution với cấu trúc layer: `Domain`, `Application`, `Infrastructure`, `Api` (class library cho 3 project đầu, webapi cho Api)
- [ ] Thiết lập chiều phụ thuộc đúng: `Api` → `Application`/`Infrastructure` → `Domain` (Domain không phụ thuộc project nào)
- [ ] Khởi tạo git repo, `.gitignore` cho .NET
- [ ] Chạy được `dotnet run` ở project Api, có 1 endpoint health check trả 200

**Mục tiêu cuối ngày:** `dotnet build` sạch, chạy API trả về health check OK, đã commit lần đầu.

### Ngày 2 (Thứ 3) — PostgreSQL + EF Core
- [x] Chạy PostgreSQL bằng Docker Compose (service `postgres` trong `docker-compose.yml`) để dev — đổi từ kế hoạch gốc "container đơn lẻ" sang Compose ngay từ Ngày 2
- [x] Tạo trước thư mục `docker/` chứa `Dockerfile` build API (multi-stage), chuẩn bị sẵn cho Ngày 21 — chưa gắn service `api` vào compose, chỉ dùng `postgres` cho đến hết Tuần 2
- [x] Cài package `Npgsql.EntityFrameworkCore.PostgreSQL`, tạo `AppDbContext` trong `Infrastructure`
- [x] Cấu hình connection string qua `appsettings.Development.json` + `appsettings.json` (không hardcode)
- [x] Đăng ký `AddDbContext` trong `Program.cs` (qua `AddInfrastructure` extension method)
- [x] Tạo migration đầu tiên (rỗng) và `dotnet ef database update` thành công

> Ghi chú: máy có PostgreSQL 18 cài native chạy Windows Service chiếm cổng 5432, nên service `postgres` trong compose map ra cổng ngoài **5435** (xem connection string trong `appsettings.Development.json` và `docker-compose.yml`).
>
> Từ giờ chạy `docker compose up -d` để bật Postgres thay vì `docker run` tay. API vẫn chạy bằng `dotnet run`/`dotnet watch` trên máy (không chạy trong container) để dev nhanh, không phải rebuild image mỗi lần sửa code.

**Mục tiêu cuối ngày:** API kết nối được PostgreSQL (qua Docker Compose), bảng `__EFMigrationsHistory` xuất hiện trong DB.

### Ngày 3 (Thứ 4) — Thiết kế entity và ERD
- [x] Vẽ ERD cho: Product, Category, Supplier, Customer, Order, OrderItem (dùng Mermaid, lưu tại `docs/erd.md` — `StockMovement` để dành Tuần 3)
- [x] Code entity trong `Domain` với navigation property đúng chiều
- [x] Xác định quan hệ 1-n (Category 1-n Product, Supplier 1-n Product, Order 1-n OrderItem, Customer 1-n Order, User 1-n Order)
- [x] Cấu hình Fluent API (`IEntityTypeConfiguration<T>`) cho khóa ngoại, độ dài field, required
- [x] Tạo migration cho toàn bộ entity, `database update`

**Mục tiêu cuối ngày:** đạt — ERD tại `docs/erd.md`, toàn bộ bảng đã tạo đúng quan hệ trong DB.

### Ngày 4 (Thứ 5) — Seed data
- [ ] Viết seed data cho Category, Supplier, Customer, Product (đủ để test CRUD và nghiệp vụ tuần 3)
- [ ] Chọn cách seed: `HasData` trong migration hoặc seeding service chạy lúc `app.Run()` (ghi rõ lý do chọn)
- [ ] Thêm index cho các field hay truy vấn (ví dụ `Product.SKU` unique)
- [ ] Kiểm tra lại dữ liệu sau khi seed bằng pgAdmin/psql hoặc client DB bất kỳ

**Mục tiêu cuối ngày:** DB có dữ liệu mẫu đủ dùng, không lỗi migration khi seed lại từ đầu.

### Ngày 5 (Thứ 6) — CRUD Product + Category
- [x] CRUD Product: Create/Read/Update/Delete qua Controller (đã chọn Controller thay vì Minimal API)
- [ ] CRUD Category tương tự — dời sang Ngày 6, gộp chung với Supplier/Customer (cùng là CRUD đơn giản không FK, lặp lại pattern Product)
- [x] Tạo DTO riêng cho request/response (không trả entity trực tiếp), map tay bằng extension method (`ToDto`/`ToEntity`/`ApplyTo`) — chọn không dùng Mapster/AutoMapper cho project này
- [x] Tích hợp FluentValidation cho `CreateProductRequest` và `UpdateProductRequest` (`SKU`, `Name`, `Price`, `StockQuantity`, `CategoryId`, `SupplierId`), gắn vào Controller cả 2 action
- [x] Test thủ công qua curl/http file (chưa cài Swagger UI, `AddOpenApi()` hiện chỉ có JSON thô — xem ghi chú kiến thức)

**Mục tiêu cuối ngày:** đạt — CRUD Product chạy đúng qua test thật, có validate đầy đủ cho cả Create và Update. Category dời sang Ngày 6 cùng Supplier/Customer.

> **Đã dọn xong (Ngày 5, "dọn hết một lần"):** 27 warning nullable đã sửa hết (field optional → `string?`, field bắt buộc → `required`, navigation property → `= null!`), migration `FixOptionalFieldsNullability` đã áp dụng; `ProductDto`/`CreateProductRequest`/`UpdateProductRequest.Description` đồng bộ nullable theo entity. Build toàn solution: 0 warning, 0 error.

> Tổng kết chi tiết Ngày 5 (kiến trúc, lỗi thật gặp phải, bài học) xem ở cuối ngày trong hội thoại — đáng đọc lại trước khi làm Category vì sẽ lặp lại đúng pattern.

### Ngày 6 (Thứ 7, nhiều thời gian hơn) — CRUD Supplier/Customer + paging/filter/sort
- [x] CRUD Category, Supplier, Customer (Category dời từ Ngày 5)
- [x] Viết helper paging chung — `PageResult<T>` (generic, dùng lại được cho mọi danh sách)
- [x] Thêm filter (theo CategoryId, theo tên) và sort (theo tên, theo giá) cho API list Product
- [x] Áp dụng paging/filter/sort cho Customer (filter theo tên, sort theo tên/email) — Order sẽ dùng lại pattern này ở tuần 3

**Mục tiêu cuối ngày:** đạt — tất cả API list đều hỗ trợ paging; API Product và Customer hỗ trợ filter + sort.

### Ngày 7 (Chủ nhật, nhiều thời gian hơn) — Exception handling, chống N+1, tổng kết tuần
- [x] Viết global exception handler (`IExceptionHandler`), trả về `ProblemDetails` chuẩn
- [x] Rà lại các query list/detail có navigation property → kiểm tra N+1 bằng cách bật log SQL tạm thời (`LogTo` + `EnableSensitiveDataLogging`)
- [x] Kết quả rà soát — không có N+1 ở API nào, không cần sửa gì thêm:
  - `GET /api/products` (list, paging): đúng 2 query — 1 `COUNT`, 1 `SELECT` có `INNER JOIN Categories/Suppliers` + `LIMIT/OFFSET`. Dùng `Include` vì DTO cần `CategoryName`/`SupplierName`.
  - `GET /api/products/{id}` (detail): 1 query, `INNER JOIN` tương tự.
  - `GET /api/categories`, `/api/suppliers`: 1 query đơn giản, không `JOIN` — đúng vì 2 entity này không có FK ra ngoài, DTO không cần `Include`.
  - `GET /api/customers` (có filter+sort+paging): đúng 2 query — 1 `COUNT` (có `WHERE ... LIKE` khi filter theo tên), 1 `SELECT` có `WHERE/ORDER BY/LIMIT/OFFSET`.
  - Kết luận: vì EF Core không bật lazy loading (không cài `Microsoft.EntityFrameworkCore.Proxies`), và toàn bộ code đều dùng `Include`/projection tường minh thay vì lazy-load navigation property trong vòng lặp, nên không có đường nào sinh ra N+1.
- [ ] Dọn code, commit, viết chú thích ngắn trong README nếu còn thiếu phần nào

**Mục tiêu cuối ngày:** đạt — không có N+1 ở các API đã làm (xác nhận qua log SQL thật), lỗi trả về đúng format `ProblemDetails`.

### Tổng kết tuần 1
Tự trả lời trước khi qua tuần 2:
- [ ] Giải thích được vì sao tách Domain/Application/Infrastructure/Api và chiều phụ thuộc giữa chúng
- [ ] Giải thích được vì sao cần DTO thay vì trả entity trực tiếp
- [ ] Giải thích được N+1 là gì và đã sửa bằng cách nào ở project này
- [ ] Danh sách task còn dư (nếu có) đã dọn xong trong tuần, không mang sang tuần 2
- [ ] Cập nhật dòng "Tuần 1" trong bảng tiến độ ở `dotnet-roadmap.md` (ngày bắt đầu/kết thúc, trạng thái)

---

## Tuần 2: Auth, role, permission

**Mục tiêu tuần:** đăng ký/đăng nhập bằng JWT, có refresh token lưu DB với rotate/revoke, phân quyền 2 lớp role + permission động (hoặc fallback static policy nếu trễ), Swagger hỗ trợ Bearer token.

> Lưu ý từ roadmap: nếu hết tuần mà `IAuthorizationHandler` + policy provider động chưa xong, dùng tạm `[Authorize(Policy="product.create")]` với policy khai báo tĩnh trong `Program.cs`, ghi nợ phần policy provider động để hoàn thiện ở Project 2. **Không bỏ JWT/refresh token** trong mọi trường hợp.

### Ngày 8 (Thứ 2) — ASP.NET Identity
- [x] Cài `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
- [x] Tạo `ApplicationUser : IdentityUser<int>` (thêm `FullName`), `ApplicationRole : IdentityRole<int>` — đặt ở `CRM.Infrastructure/Identity/`, không phải Domain (Domain không được phụ thuộc framework)
- [x] Tích hợp Identity vào `AppDbContext` hiện có: đổi kế thừa `DbContext` → `IdentityDbContext<ApplicationUser, ApplicationRole, int>`, gọi `base.OnModelCreating` trước
- [x] Xóa entity tạm `Domain/Entities/User.cs`; `Order` bỏ navigation property `CreatedByUser`, chỉ giữ `CreatedBy` (int scalar FK) — quan hệ sang `AspNetUsers` cấu hình 1 chiều ở `OrderConfiguration` (`HasOne<ApplicationUser>().WithMany()`, không có nav property phía Domain)
- [x] Migration `AddAspNetIdentity`, `database update` — đã áp dụng, xác nhận bằng `\dt` thấy đủ 7 bảng `AspNet*`
- [x] Đăng ký `AddIdentity<ApplicationUser, ApplicationRole>().AddEntityFrameworkStores<AppDbContext>()` — đặt trong `Infrastructure/DependencyInjection.cs` (không phải trực tiếp `Program.cs`) để nhất quán với cách đăng ký DbContext đã có
- [x] Thêm `<FrameworkReference Include="Microsoft.AspNetCore.App" />` vào `CRM.Infrastructure.csproj` — `AddIdentity` sống trong shared framework ASP.NET Core, class library thường không tự có reference này

**Mục tiêu cuối ngày:** đạt — bảng `AspNetUsers`, `AspNetRoles`,... xuất hiện trong DB, build chạy sạch 0 warning/error, API cũ (Product/Category/Supplier/Customer) vẫn chạy đúng sau khi đổi `AppDbContext`.

### Ngày 9 (Thứ 3) — Đăng ký/Đăng nhập + JWT access token
- [x] `IIdentityService`/`ITokenService` ở Application (chỉ dùng kiểu nguyên thủy/record, không lộ `ApplicationUser`); `IdentityService`/`JwtTokenService` implement ở Infrastructure
- [x] Endpoint Register (`POST /api/auth/register`, dùng `UserManager.CreateAsync`)
- [x] Endpoint Login (`POST /api/auth/login`): verify password qua `UserManager.CheckPasswordAsync`, phát access token JWT (claims: NameIdentifier/Email/Name/Role)
- [x] Cấu hình `AddAuthentication().AddJwtBearer(...)` với issuer/audience/signing key từ config (`appsettings.Development.json` → `Jwt` section); ghi đè default scheme vì `AddIdentity` tự đặt cookie làm mặc định; bật lại `MapInboundClaims = true` (mặc định .NET 8+ là `false`, sẽ phá `[Authorize(Roles=...)]` nếu không set)
- [x] Endpoint `GET /api/auth/me` (`[Authorize]`) để xác nhận JWT hoạt động — trả về toàn bộ claims thật trong token

**Mục tiêu cuối ngày:** đạt — test thật bằng curl: register → 201, login → 200 kèm JWT, `/me` không token → 401, `/me` có token → 200 với đúng claims (userId=1, email, fullName).

**Mục tiêu cuối ngày:** đăng ký → đăng nhập → nhận JWT → gọi được endpoint có `[Authorize]`.

### Ngày 10 (Thứ 4) — Refresh token
- [x] Entity `RefreshToken` (`TokenHash`, `UserId`, `CreatedAtUtc`, `ExpiresAtUtc`, `RevokedAtUtc`, `ReplacedByTokenHash`) — đặt ở `CRM.Infrastructure/Identity/` cùng `ApplicationUser` (không phải business entity nên không ở Domain)
- [x] `POST /api/auth/refresh`: nhận refresh token cũ → kiểm tra hợp lệ/chưa revoke/chưa hết hạn → phát access token mới + refresh token mới (rotate, 1 `SaveChangesAsync` duy nhất cho cả revoke-cũ + tạo-mới)
- [x] `POST /api/auth/logout`: revoke refresh token hiện tại
- [x] Reuse detection: refresh token đã bị revoke mà bị dùng lại → revoke **toàn bộ** refresh token còn sống của user đó, buộc login lại từ đầu
- [x] Lưu `TokenHash` (SHA-256), không lưu plain text — chỉ trả plain token 1 lần duy nhất lúc phát hành

**Mục tiêu cuối ngày:** đạt — test thật bằng curl: login → refresh (rotate, token mới khác token cũ) → dùng lại token cũ → 401 + token mới cũng bị revoke theo (reuse detection) → logout → dùng lại sau logout → 401. Verify trực tiếp trong Postgres: `TokenHash` là hash hex, không phải token gốc.

### Ngày 11 (Thứ 5) — Role-based authorization
- [x] Seed 3 role: Admin, Manager, Staff lúc khởi động app (`IdentitySeeder.SeedRolesAsync`, chạy ở MỌI môi trường — idempotent, check `RoleExistsAsync` trước khi tạo)
- [x] Gán role cho user khi đăng ký (mặc định Staff, trong `IdentityService.RegisterAsync`); seed thêm 1 tài khoản `admin@crm.local` (role Admin) ở Development để có thể test ngay — không có đường nào "tự phong" Admin đầu tiên qua API nếu không seed sẵn
- [x] Áp `[Authorize(Roles = "Admin,Manager")]` tạm cho `DELETE /api/products/{id}` và `DELETE /api/categories/{id}` (Ngày 12-13 sẽ thay bằng permission-based policy)

**Mục tiêu cuối ngày:** đạt — test thật: không token → `401`; token Staff → `403` (phân biệt đúng với 401); token Admin → `404` (qua được authorization, service đúng báo not found vì id test không tồn tại). Xác nhận role claim thật nằm trong token qua `/api/auth/me`.

### Ngày 12 (Thứ 6) — Thiết kế permission
- [x] `PermissionCatalog` — 19 hằng số `resource.action` khớp đúng ma trận ở tài liệu nghiệp vụ mục 6 (tránh gõ tay chuỗi rải rác, sẽ dùng lại cho `[Authorize(Policy=...)]` ở Ngày 13)
- [x] Entity `Permission` + bảng nối `RolePermission` (many-to-many Role-Permission, đặt ở `CRM.Infrastructure/Identity/` cùng `ApplicationUser`/`RefreshToken` — không phải business entity)
- [x] Seed permission + ma trận gán theo role (`IdentitySeeder.SeedPermissionsAsync`/`SeedRolePermissionsAsync`, chạy ở MỌI môi trường, idempotent) — Admin = tất cả 19, Manager = 15, Staff = 9
- [x] Migration `AddPermissions` + kiểm tra dữ liệu seed đúng

**Mục tiêu cuối ngày:** đạt — verify trực tiếp trong Postgres: tổng 19 permission, số lượng đúng theo role (Admin 19 / Manager 15 / Staff 9), danh sách permission của Staff khớp chính xác ma trận. Chạy app lần 2 xác nhận không tạo trùng dữ liệu (idempotent).

### Ngày 13 (Thứ 7, nhiều thời gian hơn) — Policy provider động
- [x] `PermissionRequirement` (`IAuthorizationRequirement`) — chỉ chứa tên permission, không có logic
- [x] `PermissionAuthorizationHandler` (`AuthorizationHandler<PermissionRequirement>`) — đọc role từ claim của user, query `RolePermissions` thật trong DB để quyết định `Succeed`
- [x] `PermissionPolicyProvider` (`IAuthorizationPolicyProvider`) — tự "chế" `AuthorizationPolicy` theo tên permission ngay lúc được hỏi, validate tên qua `PermissionCatalog` trước, không khớp thì giao cho `DefaultAuthorizationPolicyProvider`
- [x] Áp `[Authorize(Policy = PermissionCatalog.ProductCreate/.ProductUpdate/.ProductDelete)]` cho Product, `[Authorize(Policy = PermissionCatalog.CategoryManage)]` cho Category Delete — thay thế hoàn toàn `[Authorize(Roles=...)]` tạm ở Ngày 11
- [x] **Fix 1 lỗi thật phát hiện qua việc chuyển sang permission**: `[Authorize(Roles="Admin,Manager")]` ở Ngày 11 cho cả Manager xóa Product — sai so với ma trận (chỉ Admin có `product.delete`). Permission-based policy sửa đúng lại.

**Mục tiêu cuối ngày:** đạt — test thật với 3 role (Staff/Manager/Admin) qua 4 endpoint: Staff bị chặn đúng `403` ở `product.create`/`category.manage`; Manager tạo Product thành công (`201`) nhưng bị chặn đúng `403` khi xóa Product (khác hành vi sai ở Ngày 11); Admin qua hết; không token → `401`.

### Ngày 14 (Chủ nhật, nhiều thời gian hơn) — API quản lý role/permission, Swagger Bearer, tổng kết tuần
- [x] API CRUD Role: `GET/POST /api/roles` (permission `role.manage`), `GET /api/roles/{id}` — trả kèm danh sách permission của role
- [x] API gán/bỏ permission cho role: `POST/DELETE /api/roles/{id}/permissions[/{permissionName}]` (permission `permission.manage`); `GET /api/permissions` liệt kê permission hợp lệ để chọn
- [x] API gán/bỏ role cho user: `POST/DELETE /api/users/{id}/roles[/{roleName}]` (permission `user.manage`); `GET /api/users` liệt kê user kèm role hiện có
- [x] Bearer token cho OpenAPI: project dùng `AddOpenApi()` native + Scalar (không phải Swashbuckle) nên không có `AddSecurityDefinition`/`AddSecurityRequirement` như plan gốc — thay bằng `BearerSecuritySchemeTransformer` (`IOpenApiDocumentTransformer`) tự thêm security scheme + gắn vào mọi operation
- [x] Test lại toàn bộ luồng qua curl (tương đương test qua UI): login → dùng token gọi API có policy — **phát hiện và sửa 1 bug thật**: `UserManager.AddToRoleAsync` throw exception (không trả `IdentityResult.Failed`) khi role không tồn tại → API trả `500` thay vì `404`; đã thêm check tồn tại trước khi gọi

**Mục tiêu cuối ngày:** đạt — quản lý được role/permission/user-role qua API, đều có permission-policy đúng (test qua cả Admin và Staff), OpenAPI doc có security scheme Bearer gắn đúng vào mọi operation (verify trực tiếp trong JSON trả về), Scalar UI (`/scalar/v1`) load được. Code chưa commit — chờ xác nhận trước khi commit.

### Tổng kết tuần 2
Tự trả lời trước khi qua tuần 3:
- [ ] Giải thích được vì sao dùng policy-based thay vì chỉ `[Authorize(Roles=...)]`
- [ ] Giải thích được cách lưu refresh token an toàn (hash, rotate, revoke)
- [ ] Nếu dùng fallback static policy: đã ghi rõ nợ kỹ thuật trong README/kế hoạch Project 2 chưa
- [ ] Cập nhật dòng "Tuần 2" trong bảng tiến độ ở `dotnet-roadmap.md`

---

## Tuần 3: Nghiệp vụ đơn hàng và kho

**Mục tiêu tuần:** luồng tạo/hủy đơn hàng có transaction và kiểm soát tồn kho đúng, StockMovement ghi nhận đầy đủ, có Serilog, có unit test cho service nghiệp vụ, Docker Compose chạy được, README + ERD hoàn chỉnh.

### Ngày 15 (Thứ 2) — Tạo đơn hàng
- [ ] Thiết kế use case `CreateOrder`: nhận CustomerId + danh sách (ProductId, Quantity)
- [ ] Mở transaction: tạo Order + OrderItem, trừ `Product.StockQuantity`, tạo `StockMovement` loại Export (Reference = OrderId), commit transaction
- [ ] Tính `Order.TotalAmount` = tổng (Quantity × UnitPrice tại thời điểm đặt)
- [ ] Nếu có lỗi ở bất kỳ bước nào → rollback toàn bộ (không trừ kho một phần)

**Mục tiêu cuối ngày:** tạo đơn hàng thành công → kho trừ đúng, có StockMovement tương ứng; giả lập lỗi giữa transaction → không có gì bị thay đổi trong DB.

### Ngày 16 (Thứ 3) — Hủy đơn hàng
- [ ] Thiết kế trạng thái Order: Pending → Cancelled / Completed (chỉ hủy được khi đang Pending)
- [ ] Use case `CancelOrder`: hoàn kho (+Quantity vào `Product.StockQuantity`), tạo `StockMovement` loại hoàn kho (Reference = OrderId), set status = Cancelled, trong transaction
- [ ] Chặn hủy đơn đã Completed/đã Cancelled (trả lỗi rõ ràng, không phải exception chung)

**Mục tiêu cuối ngày:** hủy đơn Pending → kho được hoàn đúng số lượng; hủy đơn đã Completed bị chặn với lỗi nghiệp vụ rõ ràng.

### Ngày 17 (Thứ 4) — Chống tồn kho âm + concurrency cơ bản
- [ ] Thêm kiểm tra tồn kho đủ trước khi trừ (trả lỗi nghiệp vụ nếu không đủ, không để DB tự chặn bằng constraint)
- [ ] Thêm concurrency token (`RowVersion`/`xmin`) cho `Product` để tránh 2 request trừ kho cùng lúc ghi đè nhau
- [ ] Xử lý `DbUpdateConcurrencyException`: retry hoặc trả lỗi 409 rõ ràng cho client
- [ ] Viết thử 1 kịch bản gọi 2 request tạo đơn cùng lúc trên cùng sản phẩm tồn kho thấp để quan sát hành vi (script hoặc test thủ công)

**Mục tiêu cuối ngày:** không thể tạo đơn khiến tồn kho âm; có xử lý rõ ràng khi 2 giao dịch đụng nhau trên cùng sản phẩm.

### Ngày 18 (Thứ 5) — StockMovement đầy đủ
- [ ] Hoàn thiện entity `StockMovement` với đủ loại: Import (nhập hàng từ NCC), Export (bán hàng), Adjustment (điều chỉnh/kiểm kê)
- [ ] API nhập kho thủ công (Import) và điều chỉnh kho thủ công (Adjustment) — không qua Order
- [ ] API xem lịch sử StockMovement theo sản phẩm, có paging/filter theo loại và khoảng thời gian

**Mục tiêu cuối ngày:** có thể nhập/điều chỉnh kho thủ công, và tra được lịch sử biến động kho của 1 sản phẩm.

### Ngày 19 (Thứ 6) — Serilog — **BỎ QUA**
> Quyết định của người học: bỏ qua Ngày 19 vì không ảnh hưởng nghiệp vụ. Logging mặc định của ASP.NET Core vẫn hoạt động (console). Ghi nợ kỹ thuật, mang sang Project 2 nếu cần.

### Ngày 20 (Thứ 7, nhiều thời gian hơn) — Unit test — **BỎ QUA**
> Quyết định của người học: bỏ qua Ngày 20 vì không ảnh hưởng nghiệp vụ. Hệ quả: tiêu chí "có test" trong `dotnet-roadmap.md` chưa đạt — ghi nhận công khai ở phần tổng kết bên dưới, không coi như đã hoàn thành. Toàn bộ nghiệp vụ Tuần 3 (CreateOrder/CancelOrder/concurrency/StockMovement) đã được verify bằng test thủ công (curl thật, kể cả kịch bản 2 request đồng thời) thay cho unit test tự động.

### Ngày 21 (Chủ nhật, nhiều thời gian hơn) — Docker Compose, README, ERD, tổng kết Project 1
> Cập nhật: `docker-compose.yml` (service `postgres`) và `docker/Dockerfile` (build API) đã tạo sẵn từ Ngày 2. Việc còn lại ở ngày này chỉ là **gắn thêm service `api`** vào compose, không phải viết từ đầu.
- [x] Thêm service `api` vào `docker-compose.yml`: build từ `docker/Dockerfile`, `depends_on: postgres (condition: service_healthy)`, connection string trỏ `Host=postgres;Port=5432` (qua env var `ConnectionStrings__Default`, không sửa appsettings). Cổng host đổi thành `8090` (máy dev đã có service khác chiếm `8080`).
- [x] Kiểm tra `docker/Dockerfile` — build thử image thành công, không cần sửa gì (đã đúng từ Ngày 2).
- [x] **Phát hiện + sửa 1 bug khi test "chạy từ đầu"**: `Program.cs` trước đây không tự áp migration — trên volume Postgres hoàn toàn trống, app crash ngay ở bước seed role vì bảng `AspNetRoles` chưa tồn tại (trước giờ luôn chạy được vì DB dev local đã có migration áp sẵn bằng tay, chưa ai test lại từ đầu thật sự). Thêm `await dbContext.Database.MigrateAsync();` vào đúng khối seed không-điều-kiện-môi-trường. Verify lại bằng Postgres + container hoàn toàn mới (network/volume cô lập, không đụng `crm-postgres` thật): tự tạo đủ 18 bảng, seed role/permission/admin, health check `200`, login thành công.
- [x] Chạy `docker compose up -d api` thật lên `crm-postgres` đang chạy — 2 container `Up`, `GET /api/products` trả dữ liệu thật qua container.
- [x] Hoàn thiện README: mô tả project, 2 cách chạy (Docker Compose / `dotnet run`), link ERD, bảng đầy đủ endpoint + permission yêu cầu, nợ kỹ thuật.
- [x] Rà lại toàn bộ tiêu chí hoàn thành Project 1 — **phát hiện thêm 1 lỗ hổng permission thật**: `CategoriesController` (GET/POST/PUT), `SuppliersController`, `CustomersController` gần như không có `[Authorize]` nào dù `PermissionCatalog` đã định nghĩa đủ `category.view/manage`, `supplier.view/manage`, `customer.view/manage` từ Ngày 12 — nghĩa là bất kỳ ai, kể cả chưa đăng nhập, đều gọi được các API này. Đã sửa đúng theo ma trận ở mục 9 `docs/2-tai-lieu-nghiep-vu.md`, verify lại: gọi không token → `401`; gọi có token Admin → `200` như cũ.

**Mục tiêu cuối ngày:** đạt — `docker compose up` chạy được từ máy sạch (volume trống), README đầy đủ. **Chưa đạt:** "test pass" (không có unit test, xem Ngày 20).

### Tổng kết tuần 3 (và tổng kết Project 1)
- [x] Giải thích được vì sao cần transaction khi tạo/hủy đơn hàng — xem phần lý thuyết Ngày 15/16 trong hội thoại: nhiều thay đổi (Order + OrderItem + Product.StockQuantity + StockMovement) phải được coi là 1 đơn vị duy nhất, tránh trạng thái "lưng chừng" nếu lỗi giữa chừng (ví dụ đã trừ kho nhưng chưa tạo được Order).
- [x] Giải thích được cơ chế optimistic concurrency đang dùng hoạt động ra sao — `xmin` (cột hệ thống Postgres, shadow property) được EF Core tự thêm vào `WHERE` của câu `UPDATE Product`; nếu giá trị đã đổi do request khác ghi trước, `UPDATE` khớp 0 dòng → EF tự ném `DbUpdateConcurrencyException` → bắt lại, trả `409`. Đã verify thật bằng 2 request đồng thời ở Ngày 17 (1 thành công `201`, 1 nhận đúng `409`, tồn kho cuối cùng chính xác, không bị lost-update).
- [x] Giải thích được StockMovement giải quyết vấn đề gì — `Product.StockQuantity` chỉ là "số dư hiện tại" (giống số dư tài khoản ngân hàng), không trả lời được "vì sao". `StockMovement` là sổ cái append-only, mỗi dòng ghi rõ +/- bao nhiêu, loại gì (Import/Export/Adjustment), liên quan nguồn nào (`Reference`) — cho phép truy vết/kiểm toán lịch sử kho mà chỉ 1 con số không bao giờ làm được.
- [x] Đối chiếu lại "Hoàn thành Project 1 khi" ở `dotnet-roadmap.md`: `docker compose up` ✓ (verify thật, kể cả từ volume trống), README ✓, **test ✗** (bỏ qua Ngày 20 theo quyết định có chủ đích — xem ghi chú Ngày 20).
- [x] Cập nhật dòng "Tuần 2" và "Tuần 3" trong bảng tiến độ ở `dotnet-roadmap.md`.
- [x] Nợ kỹ thuật mang qua Project 2:
  - Không có unit test (Ngày 20 bỏ qua) và không có Serilog/structured logging (Ngày 19 bỏ qua).
  - Đã phát hiện và sửa 2 gap thật trong chính Ngày 21 (migration tự động lúc khởi động, permission thiếu ở Category/Supplier/Customer) — nhắc nhở: nên "rà lại từ đầu trên máy sạch" sớm hơn, không đợi tới ngày cuối cùng mới phát hiện.
  - Policy provider động (Ngày 13) đã hoàn thành đúng kế hoạch, không còn là nợ kỹ thuật như ghi chú dự phòng ban đầu trong `dotnet-roadmap.md`.

---

## Nếu trễ tiến độ

Thứ tự cắt giảm khi không đủ thời gian trong 3 tuần (ưu tiên giữ phần lõi để qua Giai đoạn 1 không bị hổng kiến thức):
1. Cắt trước: paging/filter/sort nâng cao (giữ paging cơ bản), API nhập/điều chỉnh kho thủ công chi tiết (giữ luồng order trừ/hoàn kho).
2. Cắt tiếp nếu vẫn trễ: policy provider động → dùng fallback static policy (đã ghi trong roadmap gốc), ghi nợ kỹ thuật.
3. **Không cắt:** JWT + refresh token, transaction khi tạo/hủy đơn, chống tồn kho âm, unit test cho service nghiệp vụ — đây là phần cốt lõi của cả Giai đoạn 1.
