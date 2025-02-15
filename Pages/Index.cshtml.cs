using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Collections.Generic;

namespace TaskTracker.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _context;

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public void OnGet()
        {
            int userId = int.Parse(HttpContext.Session.GetString("user_id"));
            AvailableProjects.AddRange(_context.ProjectsUsers.Where(x => x.UserId == userId).Select(x => x.ProjectId));
        }

        public List<int> AvailableProjects { get; set; } = new List<int>();
    }
}
