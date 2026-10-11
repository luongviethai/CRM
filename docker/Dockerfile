# Chuẩn bị trước cho Ngày 21 (Tuần 3) — build context là thư mục gốc project (Pharse1/CRM):
#   docker build -f docker/Dockerfile .
# Khi gắn vào docker-compose.yml: context: . , dockerfile: docker/Dockerfile

# Stage 1: build + publish bằng SDK image
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy riêng các .csproj trước để tận dụng layer cache của Docker:
# chỉ khi file .csproj đổi (thêm/xoá package) thì bước restore mới chạy lại.
COPY src/CRM.Api/CRM.Api.csproj src/CRM.Api/
COPY src/CRM.Application/CRM.Application.csproj src/CRM.Application/
COPY src/CRM.Domain/CRM.Domain.csproj src/CRM.Domain/
COPY src/CRM.Infrastructure/CRM.Infrastructure.csproj src/CRM.Infrastructure/
RUN dotnet restore src/CRM.Api/CRM.Api.csproj

COPY src/ src/
RUN dotnet publish src/CRM.Api/CRM.Api.csproj -c Release -o /app/publish --no-restore

# Stage 2: chạy bằng runtime image (nhỏ hơn SDK image rất nhiều, không có công cụ build)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "CRM.Api.dll"]
