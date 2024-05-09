using Microsoft.EntityFrameworkCore;
using ManageServer.Models;

namespace ManageServer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User 테이블의 LoginId에 대해 유니크 인덱스 설정
            modelBuilder.Entity<User>()
                .HasIndex(u => u.LoginId)
                .IsUnique();
        }
    }
}
