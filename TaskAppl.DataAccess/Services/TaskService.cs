using TaskAppl.DataAccess.Interfaces;
using TaskAppl.Shared.Models;
using TaskAppl.DataAccess.Repositories;

namespace TaskAppl.DataAccess.Services
{
    public class TaskService : GenericRepository<TaskModel>, ITaskService
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="context"></param>
        public TaskService(ApplicationDbContext context) : base(context)
        {
        }
    }
}
