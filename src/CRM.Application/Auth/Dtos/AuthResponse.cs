namespace CRM.Application.Auth.Dtos;

public record AuthResponse(string AccessToken, DateTime ExpiresAtUtc, string Email, string FullName);
