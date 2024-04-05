using DatingApp.API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext : IdentityDbContext<AppUser,AppRole,int,
        IdentityUserClaim<int>,AppUserRole,IdentityUserLogin<int>,
        IdentityRoleClaim<int>,IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Message { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>().
                HasMany(u => u.UserRoles).
                WithOne(u => u.User).
                HasForeignKey(ur => ur.UserId).
                IsRequired();

            modelBuilder.Entity<AppRole>().
                HasMany(u => u.UserRoles).
                WithOne(u => u.Role).
                HasForeignKey(ur => ur.RoleId).
                IsRequired();


            modelBuilder.Entity<UserLike>().
                HasKey(k => new { k.SourceUserID, k.TargetUserID });

            modelBuilder.Entity<UserLike>()
                .HasOne(x => x.SourceUser)
                .WithMany(x => x.LikedUsers)
                .HasForeignKey(f => f.SourceUserID)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<UserLike>()
                .HasOne(x => x.TargetUser)
                .WithMany(x => x.LikedByUsers)
                .HasForeignKey(f => f.TargetUserID)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<Message>()
                .HasOne(x => x.Recipient)
                .WithMany(x => x.MessagesReceived)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Message>()
                .HasOne(x => x.Sender)
                .WithMany(x => x.MessagesSend)
                .OnDelete(DeleteBehavior.NoAction);

        }

    }
}
