
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, Role, string,
                                        IdentityUserClaim<string>, ApplicationUserRole,
                                        IdentityUserLogin<string>, IdentityRoleClaim<string>, 
                                        IdentityUserToken<string>>
    {





        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {



            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUserRole>(userRole =>
            {
            //add composed primary key (user id ,role id)
            userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

            userRole.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

            userRole.HasOne(ur => ur.User)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();


                modelBuilder.Entity<Role>().HasData(

                        new Role
                        {
                            Name = "Admin",
                            NormalizedName = "ADMIN"
                        },
                        new Role
                        {
                            Name = "Employee",
                            NormalizedName = "EMPLOYEE"
                        });

            });

       

        //  modelBuilder.Entity<ResetPassword>().HasOne(rp => rp.User).WithMany(r => r.UserResetsPassword).HasForeignKey(u => u.UserId);
        }

        public DbSet<ResetPassword> ResetPasswords { get; set; }


        public DbSet<Company> Companies { get; set; }




    }
}
