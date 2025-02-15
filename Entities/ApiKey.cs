using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskTracker.Entities
{
    [Table("api_keys")]
    public class ApiKey
    {
        [Key]
        [Column("key")]
        public string Key { get; set; }
        
        [Column("user_id")]
        public int UserId { get; set; }
    }
}
