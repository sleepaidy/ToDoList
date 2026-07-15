using Microsoft.EntityFrameworkCore;
using ToDoList.Data.Models;

namespace ToDoList.Data
{
    public class WebContext : DbContext
    {
        public DbSet<TaskData> Tasks { get; set; }
        public DbSet<UserData> Users { get; set; }

        public WebContext(DbContextOptions<WebContext> options):base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserData>(entity =>
            {
                entity.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);

                entity.HasIndex(u => u.Name)
                    .IsUnique();
            });
        }
    }
}
