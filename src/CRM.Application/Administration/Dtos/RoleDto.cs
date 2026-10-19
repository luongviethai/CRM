namespace CRM.Application.Administration.Dtos;

public record RoleDto(int Id, string Name, IReadOnlyList<string> Permissions);
