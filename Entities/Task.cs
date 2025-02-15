using System.ComponentModel.DataAnnotations.Schema;

namespace TaskTracker.Entities
{
    public class Task
    {
        public Task(int id, int projectId, int columnId, string title, string description, int order, bool completed)
        {
            this.Id = id;
            this.ProjectId = projectId;
            this.ColumnId = columnId;
            this.Title = title;
            this.Description = description;
            this.Order = order;
            this.Completed = completed;
        }

        [Column("id")]
        public int Id { get; set; }
        [Column("project_id")]
        public int ProjectId { get; set; }
        [Column("column_id")]
        public int ColumnId { get; set; }
        [Column("title")]
        public string Title { get; set; }
        [Column("description")]
        public string Description { get; set; }
        [Column("order")]
        public int Order { get; set; }
        [Column("completed")]
        public bool Completed { get; set; }
    }
}
