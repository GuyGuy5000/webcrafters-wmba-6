using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using wmbaApp.Models;

namespace wmbaApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Unique constraint for division ID
            builder.Entity<ApplicationRole>()
                .HasIndex(d => d.DivID)
                .IsUnique();

            //Unique constraint for team ID
            builder.Entity<ApplicationRole>()
                .HasIndex(d => d.TeamID)
                .IsUnique();
        }
    }
}