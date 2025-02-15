using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskTracker.Entities
{
    [Keyless()]
    [Table("teams_users")]
    public class TeamUser
    {
        [Column("team_id")]
        public int TeamId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }
    }
}
