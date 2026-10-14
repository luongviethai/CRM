using Microsoft.AspNetCore.Identity;

namespace CRM.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<int>
{
    public required string FullName { get; set; }
}
