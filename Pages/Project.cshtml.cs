using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace TaskTracker.Pages
{
    public class ProjectModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ProjectModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            if (ProjectId == null) { Response.Redirect("/index"); Response.CompleteAsync(); return; }

            int userId = int.Parse(HttpContext.Session.GetString("user_id"));

            if (_context.ProjectsUsers.FirstOrDefault(x => x.ProjectId == ProjectId && x.UserId == userId) == null) { Response.Redirect("/index"); Response.CompleteAsync(); return; }

            Columns = _context.TasksColumns.Where(x => x.ProjectId == ProjectId).ToList();
            Tasks = _context.Tasks.Where(x => x.ProjectId == ProjectId).ToList();
        }

        public Entities.Task[] GetTasksForColumnId(int id) => Tasks.Where(x => x.ColumnId == id).ToArray();

        [BindProperty(SupportsGet = true)]
        public int? ProjectId { get; set; }

        public List<TaskColumn> Columns { get; private set; }
        public List<Entities.Task> Tasks { get; private set; }
    }
}
