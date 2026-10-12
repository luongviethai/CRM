# Tài liệu nghiệp vụ — Project 1: Quản lý bán hàng và kho

> Tài liệu liên quan: [1-ke-hoach-chi-tiet.md](1-ke-hoach-chi-tiet.md) (kế hoạch theo ngày), [3-danh-sach-kien-thuc.md](3-danh-sach-kien-thuc.md) (kiến thức cần học).

## 1. Mục đích và phạm vi

Xây dựng Web API quản lý bán hàng và kho cho một cửa hàng/doanh nghiệp đơn lẻ (single-tenant, một kho duy nhất). Hệ thống cho phép quản lý danh mục sản phẩm, nhà cung cấp, khách hàng, tạo và hủy đơn hàng với kiểm soát tồn kho chặt chẽ, theo dõi lịch sử biến động kho, và phân quyền người dùng theo role + permission.

Đây là project nền tảng (Giai đoạn 1) — mục tiêu học là nắm vững CRUD có cấu trúc, auth/phân quyền, transaction và concurrency ở mức cơ bản. Không đặt mục tiêu là một hệ thống bán hàng đầy đủ tính năng thương mại.

## 2. Vai trò người dùng (Actor)

| Actor | Mô tả |
|---|---|
| Admin | Toàn quyền hệ thống, quản lý role/permission/user |
| Manager | Quản lý nghiệp vụ: sản phẩm, danh mục, NCC, khách hàng, đơn hàng, kho |
| Staff | Nhân viên bán hàng: tạo đơn hàng, xem thông tin, không được xóa/sửa cấu hình |

> Customer (khách hàng) là một **entity nghiệp vụ** (người mua hàng), không phải actor đăng nhập hệ thống trong phạm vi project này.

## 3. Từ điển dữ liệu (Entity)

### Category
| Field | Kiểu | Ghi chú |
|---|---|---|
| Id | Guid/int | PK |
| Name | string | required, unique |
| Description | string | optional |

### Supplier
| Field | Kiểu | Ghi chú |
|---|---|---|
| Id | Guid/int | PK |
| Name | string | required |
| ContactName | string | optional |
| Phone | string | optional |
| Email | string | optional |
| Address | string | optional |

### Product
| Field | Kiểu | Ghi chú |
|---|---|---|
| Id | Guid/int | PK |
| SKU | string | required, unique |
| Name | string | required |
| Description | string | optional |
| CategoryId | FK → Category | required (1-n: 1 Category có nhiều Product) |
| SupplierId | FK → Supplier | required (1-n: 1 Supplier có nhiều Product) |
| UnitPrice | decimal | >= 0 |
| StockQuantity | int | >= 0 (không được âm) |
| Unit | string | ví dụ: cái, hộp, kg |
| IsActive | bool | ngừng kinh doanh thì set false, không xóa cứng |
| RowVersion | byte[]/uint | concurrency token (xem mục 5) |

### Customer
| Field | Kiểu | Ghi chú |
|---|---|---|
| Id | Guid/int | PK |
| Name | string | required |
| Phone | string | optional |
| Email | string | optional |
| Address | string | optional |

### Order
| Field | Kiểu | Ghi chú |
|---|---|---|
| Id | Guid/int | PK |
| OrderCode | string | mã đơn hiển thị, unique |
| CustomerId | FK → Customer | required |
| OrderDate | DateTime | mặc định thời điểm tạo |
| Status | enum | Pending / Cancelled / Completed |
| TotalAmount | decimal | tổng tiền, tính từ OrderItem |
| CreatedByUserId | FK → User | ai tạo đơn |
| Notes | string | optional |

### OrderItem
| Field | Kiểu | Ghi chú |
|---|---|---|
| Id | Guid/int | PK |
| OrderId | FK → Order | required |
| ProductId | FK → Product | required (đây là bảng nối thể hiện quan hệ n-n Order↔Product) |
| Quantity | int | > 0 |
| UnitPrice | decimal | giá tại thời điểm đặt (snapshot, không lấy giá hiện tại của Product) |
| Subtotal | decimal | Quantity × UnitPrice |

### StockMovement
| Field | Kiểu | Ghi chú |
|---|---|---|
| Id | Guid/int | PK |
| ProductId | FK → Product | required |
| Type | enum | Import (nhập) / Export (xuất) / Adjustment (điều chỉnh) |
| Quantity | int | luôn dương; dấu +/- do Type quyết định, không lưu số âm |
| ReferenceType | enum/string | Order / Manual |
| ReferenceId | Guid/int? | OrderId nếu phát sinh từ đơn hàng, null nếu nhập/điều chỉnh thủ công |
| Note | string | optional, bắt buộc khi Type = Adjustment |
| CreatedAt | DateTime | required |
| CreatedByUserId | FK → User | required |

### User / Role / Permission (ASP.NET Identity + mở rộng)
| Entity | Ghi chú |
|---|---|
| User (ApplicationUser) | kế thừa IdentityUser, thêm FullName |
| Role (ApplicationRole) | Admin, Manager, Staff |
| Permission | danh sách cố định dạng `resource.action`, xem mục 6 |
| RolePermission | bảng nối n-n giữa Role và Permission |
| RefreshToken | TokenHash, UserId, ExpiresAt, RevokedAt, ReplacedByToken |

## 4. Sơ đồ quan hệ

ERD đầy đủ (Mermaid, render trực tiếp trên GitHub/GitLab/VS Code) xem tại [erd.md](erd.md) — luôn cập nhật khớp với entity thật trong code, kèm bảng giải thích delete behavior (Restrict/Cascade).

Các bảng chưa có trong code (sẽ thêm ở Tuần 2-3, ERD cập nhật khi đó): `StockMovement`, `Role`/`Permission` (qua `RolePermission`), `RefreshToken`.

## 5. Quy tắc nghiệp vụ

1. **Tồn kho không bao giờ âm.** Mọi hành động trừ kho phải kiểm tra `StockQuantity >= Quantity cần trừ` trước khi trừ; nếu không đủ, trả lỗi nghiệp vụ, không thực hiện transaction.
2. **Giá trong OrderItem là snapshot** tại thời điểm đặt hàng, không tham chiếu lại `Product.UnitPrice` sau này (đổi giá sản phẩm không ảnh hưởng đơn đã tạo).
3. **Order.Status chỉ đi theo 1 chiều hợp lệ:** `Pending → Completed` hoặc `Pending → Cancelled`. Không có chiều ngược, không sửa đơn đã Completed/Cancelled.
4. **Tạo đơn hàng (Pending) trừ kho ngay**, không chờ đến khi Completed. `Completed` chỉ là đánh dấu hoàn tất giao hàng, không tác động kho.
5. **Hủy đơn chỉ áp dụng khi Status = Pending.** Hủy đơn hoàn kho đúng số lượng đã trừ và ghi `StockMovement` tương ứng.
6. **Mọi thay đổi `StockQuantity` phải có `StockMovement` tương ứng** — không có đường tắt nào chỉnh `StockQuantity` mà không qua ghi nhận movement (coi đây là sổ cái/ledger của kho).
7. **Concurrency:** `Product` dùng concurrency token (`RowVersion`). Khi 2 request cùng sửa 1 Product (ví dụ cùng trừ kho) mà dữ liệu đã bị thay đổi bởi request khác, request sau phải nhận lỗi 409 và được xử lý rõ ràng (báo lại cho client, không âm thầm ghi đè).
8. **Transaction nguyên tử:** tạo đơn (trừ kho + tạo Order/OrderItem/StockMovement) và hủy đơn (hoàn kho + đổi status + StockMovement) đều phải chạy trong 1 transaction — thành công toàn bộ hoặc rollback toàn bộ.
9. **Permission quyết định hành vi, Role chỉ là tập hợp Permission.** Khi kiểm tra quyền trong code, kiểm tra permission (`order.cancel`), không kiểm tra tên role trực tiếp (trừ khi dùng fallback static policy tạm thời theo kế hoạch tuần 2).

## 6. Danh sách Permission và ma trận phân quyền

| Permission | Admin | Manager | Staff |
|---|---|---|---|
| category.view | ✓ | ✓ | ✓ |
| category.manage (create/update/delete) | ✓ | ✓ | |
| supplier.view | ✓ | ✓ | ✓ |
| supplier.manage | ✓ | ✓ | |
| product.view | ✓ | ✓ | ✓ |
| product.create | ✓ | ✓ | |
| product.update | ✓ | ✓ | |
| product.delete | ✓ | | |
| customer.view | ✓ | ✓ | ✓ |
| customer.manage | ✓ | ✓ | ✓ |
| order.view | ✓ | ✓ | ✓ |
| order.create | ✓ | ✓ | ✓ |
| order.cancel | ✓ | ✓ | |
| order.complete | ✓ | ✓ | ✓ |
| stockmovement.view | ✓ | ✓ | ✓ |
| stockmovement.manual (nhập/điều chỉnh thủ công) | ✓ | ✓ | |
| role.manage | ✓ | | |
| permission.manage | ✓ | | |
| user.manage (gán role) | ✓ | | |

> Nếu tuần 2 phải dùng fallback static policy (xem kế hoạch), áp dụng đúng bảng này khi khai báo policy tĩnh trong `Program.cs`.

## 7. User story theo module

**Danh mục & Nhà cung cấp**
- Với vai trò Manager, tôi muốn tạo/sửa/xóa Category và Supplier để tổ chức sản phẩm theo nhóm và nguồn nhập.

**Sản phẩm**
- Với vai trò Manager, tôi muốn thêm sản phẩm mới với SKU, giá, tồn kho ban đầu, thuộc 1 Category và 1 Supplier.
- Với vai trò Staff, tôi muốn xem danh sách sản phẩm (lọc theo danh mục, sắp xếp theo giá) để tư vấn/bán hàng.

**Khách hàng**
- Với vai trò Staff, tôi muốn tạo khách hàng mới ngay khi họ đặt đơn hàng đầu tiên.

**Đơn hàng**
- Với vai trò Staff, tôi muốn tạo đơn hàng cho khách hàng gồm nhiều sản phẩm với số lượng, để hệ thống tự trừ kho và tính tổng tiền.
- Với vai trò Manager, tôi muốn hủy một đơn hàng đặt sai, để hệ thống tự hoàn lại kho đã trừ.
- Với vai trò Staff, tôi muốn đánh dấu đơn hàng đã hoàn tất giao hàng.

**Kho**
- Với vai trò Manager, tôi muốn nhập kho khi nhận hàng từ nhà cung cấp, để tăng tồn kho và lưu lại lịch sử nhập.
- Với vai trò Manager, tôi muốn điều chỉnh kho khi kiểm kê phát hiện lệch số liệu, kèm ghi chú lý do.
- Với vai trò Manager, tôi muốn xem lịch sử biến động kho của 1 sản phẩm để truy vết sai lệch.

**Auth & phân quyền**
- Với vai trò bất kỳ, tôi muốn đăng ký/đăng nhập và nhận JWT để gọi API.
- Với vai trò Admin, tôi muốn tạo role mới và gán permission cho role, để điều chỉnh phân quyền không cần sửa code.
- Với vai trò Admin, tôi muốn gán role cho user.

## 8. Luồng nghiệp vụ chính

**Tạo đơn hàng**
1. Client gửi CustomerId + danh sách (ProductId, Quantity).
2. Validate: khách hàng tồn tại, sản phẩm tồn tại và active, quantity > 0.
3. Với từng sản phẩm: kiểm tra `StockQuantity >= Quantity`; nếu thiếu bất kỳ sản phẩm nào → dừng, trả lỗi, không tạo gì cả.
4. Mở transaction: tạo Order (status=Pending) + OrderItem (snapshot giá), trừ `StockQuantity` từng Product, tạo StockMovement (Type=Export, ReferenceType=Order) cho từng sản phẩm.
5. Tính `TotalAmount`, commit transaction.
6. Trả về Order đã tạo.

**Hủy đơn hàng**
1. Client gửi OrderId cần hủy.
2. Kiểm tra Order.Status == Pending; nếu không → trả lỗi nghiệp vụ (409 hoặc 400 kèm message rõ).
3. Mở transaction: với từng OrderItem, cộng lại `StockQuantity` của Product, tạo StockMovement (Type=Import, ReferenceType=Order, Note="Hoàn kho do hủy đơn"), set Order.Status = Cancelled.
4. Commit transaction.

**Nhập kho / điều chỉnh kho thủ công**
1. Client gửi ProductId, Quantity, Type (Import/Adjustment), Note (bắt buộc nếu Adjustment).
2. Cập nhật `StockQuantity` (cộng nếu Import, cộng/trừ theo dấu chỉ định nếu Adjustment — nhưng vẫn phải đảm bảo không âm).
3. Tạo StockMovement tương ứng (ReferenceType=Manual).

## 9. Tổng hợp API endpoint chính

| Method | Path | Permission | Mô tả |
|---|---|---|---|
| POST | /api/auth/register | (anonymous) | Đăng ký |
| POST | /api/auth/login | (anonymous) | Đăng nhập, trả JWT + refresh token |
| POST | /api/auth/refresh | (anonymous, cần refresh token hợp lệ) | Lấy access token mới |
| POST | /api/auth/logout | (authenticated) | Revoke refresh token |
| GET/POST/PUT/DELETE | /api/categories | category.view / category.manage | CRUD Category |
| GET/POST/PUT/DELETE | /api/suppliers | supplier.view / supplier.manage | CRUD Supplier |
| GET/POST/PUT/DELETE | /api/products | product.view / product.create / product.update / product.delete | CRUD Product, hỗ trợ paging/filter/sort |
| GET/POST/PUT | /api/customers | customer.view / customer.manage | CRUD Customer |
| GET | /api/orders | order.view | Danh sách đơn hàng, paging/filter theo status |
| POST | /api/orders | order.create | Tạo đơn hàng |
| POST | /api/orders/{id}/cancel | order.cancel | Hủy đơn |
| POST | /api/orders/{id}/complete | order.complete | Đánh dấu hoàn tất |
| GET | /api/stock-movements | stockmovement.view | Lịch sử biến động kho, filter theo product/type/thời gian |
| POST | /api/stock-movements | stockmovement.manual | Nhập/điều chỉnh kho thủ công |
| GET/POST | /api/roles | role.manage | Quản lý role |
| POST | /api/roles/{id}/permissions | permission.manage | Gán permission cho role |
| POST | /api/users/{id}/roles | user.manage | Gán role cho user |

## 10. Yêu cầu phi chức năng

- **Validate:** dùng FluentValidation cho mọi request DTO ghi (create/update).
- **Lỗi:** trả về theo chuẩn `ProblemDetails`, có mã lỗi/nghiệp vụ phân biệt được lỗi validate và lỗi nghiệp vụ (ví dụ thiếu kho, sai trạng thái đơn).
- **Phân trang:** mọi API danh sách trả `items`, `totalCount`, `page`, `pageSize`.
- **Logging:** Serilog structured logging, log mọi event nghiệp vụ quan trọng (tạo/hủy đơn, thay đổi kho).
- **Test:** unit test bắt buộc cho service `CreateOrder`, `CancelOrder`, và service xử lý StockMovement.
- **Vận hành:** chạy được bằng `docker compose up` (API + PostgreSQL), có README mô tả cách chạy.

## 11. Ngoài phạm vi (Out of scope)

- Không tích hợp cổng thanh toán thật.
- Không hỗ trợ nhiều kho/nhiều chi nhánh (single warehouse).
- Không hỗ trợ đa tiền tệ.
- Không gửi email/thông báo thật (để lại cho Project 2 — mời thành viên qua email token).
- Không có workflow đơn mua hàng (Purchase Order) đầy đủ với nhà cung cấp — nhập kho chỉ là thao tác đơn giản (xem mục 8).
- Không có UI, chỉ có API test qua Swagger/Postman.
