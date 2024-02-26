using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using TaskAppl.Shared.Interfaces;

namespace TaskAppl.Shared.Models
{
    public class Entity: IEntity
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity), Description("Уникальный идентификатор записи")]
        public int Id { get; set; }

        [Description("Дата и время создания записи")]
        public DateTime CreatedAt { get; set; }

        [Description("Дата и время обновления записи")]
        public DateTime UpdatedAt { get; set; }
    }
}
