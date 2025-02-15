using Mysqlx.Crud;
using System.Diagnostics;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace TaskTracker
{
    public class ProjectAuth
    {
        public void Configure(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (Configuration.GetValue<bool>("DISABLE_AUTHENTICATION")) { await next(); return; }

                int userId = Convert.ToInt32(context.Request.Headers["user-id"]);
                int projectId = Convert.ToInt32(context.Request.RouteValues["projectId"]);

                var _context = context.RequestServices.GetService<ApplicationDbContext>();

                bool found = _context.ProjectsUsers.FirstOrDefault(x => x.UserId == userId && x.ProjectId == projectId) != null;
                if (!found)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Not authorized for action!");
                    return;
                }

                await next();
            });
        }
    }
}
