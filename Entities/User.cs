using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskTracker.Entities
{
    [Table("users")]
    public class User
    {
        [Key()]
        [Column("id")]
        public int Id { get; set; }
        
        [Column("username")]
        public string Username { get; set; }
        
        [Column("pass")]
        public string Pass { get; set; }
    }
}
