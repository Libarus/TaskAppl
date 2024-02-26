using System.ComponentModel.DataAnnotations;
using TaskAppl.Shared.Enums;
using TaskAppl.Shared.Interfaces;

namespace TaskAppl.Shared.Models
{
    /// <summary>
    /// Модель описывающая задачу
    /// </summary>
    public class TaskModel: Entity
    {
        [Required]
        public required DateTime Date { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public required TaskStatusEnum Status { get; set; }
    }
}
