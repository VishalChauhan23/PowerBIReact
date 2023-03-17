using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PowerBIReact.Helpers;

namespace PowerBIReact.Models.DB
{
    public class PowerBiEmbeddedDbContext : IdentityDbContext<ApplicationUser>
    {
        public PowerBiEmbeddedDbContext(DbContextOptions<PowerBiEmbeddedDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }
}