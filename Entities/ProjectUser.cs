using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskTracker.Entities
{
    [Keyless()]
    [Table("projects_users")]
    public class ProjectUser
    {
        [Column("project_id")]
        public int ProjectId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }
    }
}
