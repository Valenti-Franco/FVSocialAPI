using Microsoft.EntityFrameworkCore;
using SocialAPI.Entities;

namespace SocialAPI.DBContexts

{
    public class Context : DbContext
    {




        public Context(DbContextOptions<Context> options) : base(options)
        {

        }
        public DbSet<EUser> User { get; set; }
        public DbSet<EPost> Post { get; set; }
        public DbSet<EComment> Comment { get; set; }
        public DbSet<ELike> Like { get; set; }
        public DbSet<EFollowing> Following { get; set; }

        public DbSet<EImageUser> ImageUser { get; set; }
        public DbSet<EImageHeader> ImageHeader { get; set; }

        public DbSet<EImagePost> ImagePost { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EUser>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<EUser>()
                .HasIndex(u => u.Email)
                .IsUnique();
            modelBuilder.Entity<EUser>()
                .HasData(
                new EUser("User")
                {
                    Id = 1,
                    Name = "User",
                    LastName = "",
                    Email = "User@gmail.com",
                    Password = "YOUR_PASSWORD_HASSHEADO",
                    Status = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    verifyToken = "Adas",
                    resetPassToken = "",
                }
              


            );

            modelBuilder.Entity<EPost>().HasData(
                new EPost("Hello World by User")
                {
                    Id = 1,
                    UserId = 1, 
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    LikesCount = 1,
                }
            );

          

            modelBuilder.Entity<ELike>().HasData(
                new ELike()
                {
                    Id = 1,
                    UserId =1,
                    PostId = 1, 
                }
            );



        }
    }
}
