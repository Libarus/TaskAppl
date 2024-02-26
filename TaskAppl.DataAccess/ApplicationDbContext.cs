using Microsoft.EntityFrameworkCore;
using TaskAppl.Shared.Models;

namespace TaskAppl.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<TaskFileModel> TaskFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TaskModel>().ToTable("Tasks");
            modelBuilder.Entity<TaskFileModel>().ToTable("TaskFiles");
        }
    }
}
