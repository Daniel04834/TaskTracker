using System.ComponentModel.DataAnnotations.Schema;

namespace TaskTracker.Entities
{
    [Table("teams")]
    public class Team
    {
        [Column("id")]
        public int Id { get; set; }
        
        [Column("name")]
        public string name { get; set; }
    }
}
