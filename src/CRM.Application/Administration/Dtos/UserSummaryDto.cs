namespace CRM.Application.Administration.Dtos;

public record UserSummaryDto(int Id, string Email, string FullName, IReadOnlyList<string> Roles);
