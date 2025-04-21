using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Dapper.Contrib.Extensions;

namespace Domain.Entities
{
    [Dapper.Contrib.Extensions.Table("Logs")]
    [PrimaryKey(nameof(Id))]
    public class LogEntity
    {
        [ExplicitKey]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string? Message { get; set; }

        [Required]
        public LogType Type { get; set; } = LogType.Information;

        public DateTime? DateInsert { get; set; } = DateTime.UtcNow;
    }
}
