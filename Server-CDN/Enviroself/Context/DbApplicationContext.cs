using Enviroself.Areas.Media.Features.Entity;
using Enviroself.Areas.User.Features.Account.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Enviroself.Context
{
    public class DbApplicationContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public DbApplicationContext(DbContextOptions<DbApplicationContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<ApplicationUserLogin> AplicationUserLogins { get; set; }
        public DbSet<MediaFile> MediaFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable(name: "User");
                entity.HasMany(u => u.UserRoles).WithOne().HasForeignKey(x => x.UserId);
            });

            modelBuilder.Entity<ApplicationRole>(entity =>
            {
                entity.ToTable(name: "UserRole");
                entity.HasMany(u => u.UserRoles).WithOne().HasForeignKey(x => x.RoleId);
            });

            modelBuilder.Entity<IdentityRoleClaim<int>>(entity =>
            {
                entity.ToTable(name: "UserRoleClaims");
            });

            modelBuilder.Entity<ApplicationUserLogin>(entity =>
            {
                entity.ToTable(name: "UserLogins");
                entity.HasKey(c => c.Id);
                entity.HasOne(c => c.User).WithMany().HasForeignKey(c => c.UserId);
            });

            modelBuilder.Entity<IdentityUserToken<int>>(entity =>
            {
                entity.ToTable(name: "UserTokens");
            });

            modelBuilder.Entity<IdentityUserClaim<int>>(entity =>
            {
                entity.ToTable(name: "UserClaims");
            });

            modelBuilder.Entity<IdentityUserRole<int>>(entity =>
            {
                entity.ToTable(name: "UserXUserRole");
            });
        }
    }
}
