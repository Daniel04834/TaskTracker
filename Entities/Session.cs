using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskTracker.Entities
{
    [Table("sessions")]
    public class Session
    {
        [Column("user_id")]
        public int UserId { get; set; }

        [Key()]
        [Column("unique_id")]
        public Guid UniqueId { get; set; }

        [Column("last_used")]
        public DateTime LastUsed { get; set; }
    }
}
