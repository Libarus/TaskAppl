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

            #region Entities
            {
                modelBuilder.Entity<TaskModel>().ToTable("Tasks");
                modelBuilder.Entity<TaskFileModel>().ToTable("TaskFiles");
            }
            #endregion

            #region Начальные данны
            {
                modelBuilder.Entity<TaskModel>().HasData(new TaskModel{ Id = 1, Date = DateTime.UtcNow, Name = "Task1", Status = Shared.Enums.TaskStatusEnum.NEW });
                modelBuilder.Entity<TaskModel>().HasData(new TaskModel{ Id = 2, Date = DateTime.UtcNow, Name = "Task2", Status = Shared.Enums.TaskStatusEnum.ACTIVE });
                modelBuilder.Entity<TaskModel>().HasData(new TaskModel{ Id = 3, Date = DateTime.UtcNow, Name = "Task3", Status = Shared.Enums.TaskStatusEnum.COMPLETE });
                
                modelBuilder.Entity<TaskFileModel>().HasData(new TaskFileModel{ Id = 1, TaskId = 1, FileName = "file001.pdf" });
                modelBuilder.Entity<TaskFileModel>().HasData(new TaskFileModel{ Id = 2, TaskId = 1, FileName = "file002.pdf" });
                modelBuilder.Entity<TaskFileModel>().HasData(new TaskFileModel{ Id = 3, TaskId = 2, FileName = "file003.pdf" });
                modelBuilder.Entity<TaskFileModel>().HasData(new TaskFileModel{ Id = 4, TaskId = 3, FileName = "file004.pdf" });
            }
            #endregion

        }
    }
}
