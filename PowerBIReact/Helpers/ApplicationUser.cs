using Microsoft.AspNetCore.Identity;

namespace PowerBIReact.Helpers
{
    public class ApplicationUser : IdentityUser// IdentityUser<Guid>
    {
        public bool IsPasswordChanged { get; set; }
        public bool IsActive { get; set; }

    }
}
