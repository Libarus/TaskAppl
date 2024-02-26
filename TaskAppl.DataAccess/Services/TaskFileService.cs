using TaskAppl.DataAccess.Interfaces;
using TaskAppl.Shared.Models;
using TaskAppl.DataAccess.Repositories;

namespace TaskAppl.DataAccess.Services
{
    public class TaskFileService : GenericRepository<TaskFileModel>, ITaskFileService
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="context"></param>
        public TaskFileService(ApplicationDbContext context) : base(context)
        {
        }
    }
}
