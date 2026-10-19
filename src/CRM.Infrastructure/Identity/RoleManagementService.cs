using CRM.Application.Administration;
using CRM.Application.Administration.Dtos;
using CRM.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Identity;

public class RoleManagementService : IRoleManagementService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly AppDbContext _context;

    public RoleManagementService(RoleManager<ApplicationRole> roleManager, AppDbContext context)
    {
        _roleManager = roleManager;
        _context = context;
    }

    public async Task<IReadOnlyList<RoleDto>> GetAllRolesAsync()
    {
        var roles = await _context.Roles.ToListAsync();

        var rolePermissions = await _context.RolePermissions
            .Include(rp => rp.Permission)
            .ToListAsync();

        return roles
            .Select(r => new RoleDto(
                r.Id,
                r.Name!,
                rolePermissions.Where(rp => rp.RoleId == r.Id).Select(rp => rp.Permission.Name).ToList()))
            .ToList();
    }

    public async Task<RoleDto?> GetRoleByIdAsync(int id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role is null)
        {
            return null;
        }

        var permissionNames = await _context.RolePermissions
            .Where(rp => rp.RoleId == id)
            .Select(rp => rp.Permission.Name)
            .ToListAsync();

        return new RoleDto(role.Id, role.Name!, permissionNames);
    }

    public async Task<CreateRoleResult> CreateRoleAsync(string name)
    {
        var role = new ApplicationRole { Name = name };
        var result = await _roleManager.CreateAsync(role);

        if (!result.Succeeded)
        {
            return new CreateRoleResult(false, null, result.Errors.Select(e => e.Description).ToList());
        }

        return new CreateRoleResult(true, new RoleDto(role.Id, role.Name!, []), []);
    }

    public async Task<bool> AssignPermissionAsync(int roleId, string permissionName)
    {
        var roleExists = await _context.Roles.AnyAsync(r => r.Id == roleId);
        if (!roleExists)
        {
            return false;
        }

        var permission = await _context.Permissions.FirstOrDefaultAsync(p => p.Name == permissionName);
        if (permission is null)
        {
            return false; // tên permission không tồn tại trong catalog
        }

        var alreadyAssigned = await _context.RolePermissions
            .AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permission.Id);

        if (alreadyAssigned)
        {
            return true; // idempotent — gán lại cái đã có thì coi như thành công, không báo lỗi
        }

        _context.RolePermissions.Add(new RolePermission { RoleId = roleId, PermissionId = permission.Id });
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemovePermissionAsync(int roleId, string permissionName)
    {
        var rolePermission = await _context.RolePermissions
            .Include(rp => rp.Permission)
            .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.Permission.Name == permissionName);

        if (rolePermission is null)
        {
            return false;
        }

        _context.RolePermissions.Remove(rolePermission);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IReadOnlyList<string>> GetAllPermissionNamesAsync()
    {
        return await _context.Permissions
            .Select(p => p.Name)
            .OrderBy(name => name)
            .ToListAsync();
    }
}
