using Microsoft.AspNetCore.Identity;

namespace RekvalifikaceApp.Models
{
    public class RoleEdit
    {
        public IdentityRole Role { get; set; } = new IdentityRole();
        public IEnumerable<AppUser> RoleMembers { get; set; } = Enumerable.Empty<AppUser>();
        public IEnumerable<AppUser> RoleNonMembers { get; set; } = Enumerable.Empty<AppUser>();
     
    }
}
