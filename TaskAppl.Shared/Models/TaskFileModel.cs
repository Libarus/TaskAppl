using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskAppl.Shared.Models
{
    /// <summary>
    /// Модель описывающая файл задачи
    /// </summary>
    public class TaskFileModel : Entity
    {
        [Required]
        public required int TaskId { get; set; }

        /// <summary>
        /// Имя файла
        /// </summary>
        [Required]
        public required string FileName { get; set; }

        /// <summary>
        /// Комментарий к файлу
        /// </summary>
        public string? Comment { get; set; }
    }
}
