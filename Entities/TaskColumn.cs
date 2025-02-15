using System.ComponentModel.DataAnnotations.Schema;

namespace TaskTracker.Entities
{
    [Table("tasks_columns")]
    public class TaskColumn
    {
        public TaskColumn(int id, int projectId, string title, int order)
        {
            this.Id = id;
            this.ProjectId = projectId;
            this.Title = title;
            this.Order = order;
        }

        [Column("id")]
        public int Id { get; set; }
        [Column("project_id")]
        public int ProjectId { get; set; }
        [Column("title")]
        public string Title { get; set; }
        [Column("order")]
        public int Order {  get; set; }
    }
}
