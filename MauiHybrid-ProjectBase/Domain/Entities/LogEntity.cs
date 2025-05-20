using Dapper.Contrib.Extensions;
using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Dapper.Contrib.Extensions.Table("Logs")]
    public class LogEntity
    {
        [Required]
        [ExplicitKey]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string? Message { get; set; }

        [Required]
        public string? Type { get; set; } = LogType.Information.ToString();

        [Required]
        public DateTime? DateAt { get; set; } = DateTime.UtcNow;
    }
}
