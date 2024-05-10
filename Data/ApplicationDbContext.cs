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
        public DbSet<PlayerRecord> PlayerRecords { get; set; } // PlayerRecord 테이블 추가

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User 테이블의 LoginId에 대해 유니크 인덱스 설정
            modelBuilder.Entity<User>()
                .HasIndex(u => u.LoginId)
                .IsUnique();

            // PlayerRecord와 User의 관계 설정
            modelBuilder.Entity<PlayerRecord>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId);
        }
    }
}
