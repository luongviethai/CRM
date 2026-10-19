using CRM.Application.Administration.Dtos;

namespace CRM.Application.Administration;

public record CreateRoleResult(bool Succeeded, RoleDto? Role, IReadOnlyList<string> Errors);
