using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace CRM.Api.OpenApi;

// Project dùng Microsoft.AspNetCore.OpenApi (native) + Scalar cho UI, không dùng Swashbuckle —
// nên không có AddSecurityDefinition/AddSecurityRequirement như plan gốc ghi. Cách tương đương
// ở đây là 1 document transformer: tự thêm khai báo "có security scheme Bearer" vào tài liệu
// OpenAPI sinh ra, rồi gắn yêu cầu đó vào từng operation. Scalar tự đọc khai báo này và hiện ra
// nút "Authorize" để nhập token, không cần cấu hình gì thêm ở phía Scalar.
//
// Lưu ý: Microsoft.OpenApi bản 2.x (đi kèm .NET 10) đổi hết namespace từ Microsoft.OpenApi.Models
// sang Microsoft.OpenApi, và đổi OpenApiReference chung thành từng loại Reference riêng theo
// component (ở đây là OpenApiSecuritySchemeReference) — khác hẳn code mẫu cũ hay thấy trên mạng.
public class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var scheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Dán access token nhận được từ POST /api/auth/login (không cần gõ chữ 'Bearer ')."
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = scheme;

        var schemeReference = new OpenApiSecuritySchemeReference("Bearer", document);

        var securityRequirement = new OpenApiSecurityRequirement
        {
            { schemeReference, [] }
        };

        if (document.Paths is null)
        {
            return Task.CompletedTask;
        }

        foreach (var pathItem in document.Paths.Values)
        {
            if (pathItem.Operations is null)
            {
                continue;
            }

            foreach (var operation in pathItem.Operations.Values)
            {
                operation.Security ??= [];
                operation.Security.Add(securityRequirement);
            }
        }

        return Task.CompletedTask;
    }
}
