using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskTracker.Entities
{
    public class Project
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("team_id")]
        public int TeamId { get; set; }

        [Column("name")]
        public string Name { get; set; }
    }
}
