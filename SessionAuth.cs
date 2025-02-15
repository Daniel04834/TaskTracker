using System.Diagnostics;
using Task = System.Threading.Tasks.Task;

namespace TaskTracker
{
    public class SessionAuth
    {
        private readonly RequestDelegate _next;
        public SessionAuth(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ApplicationDbContext _context)
        {
            if (Configuration.GetValue<bool>("DISABLE_AUTHENTICATION")) { await _next(context); return; }

            int userId = Convert.ToInt32(context.Session.GetString("user_id"));
            string? key = context.Session.GetString("unique_id");
            bool found = false;
            if (key != null)
            {
                var session = _context.Sessions.ToArray().FirstOrDefault(x => x.UserId == userId && x.UniqueId.ToString() == key);
                found = session != null;
                if (found) session.LastUsed = DateTime.Now;
                _context.SaveChanges();
            }
            if (!found)
            {
                context.Response.StatusCode = 401;
                context.Response.Redirect("/login");
                return;
            }

            await _next(context);

        }
    }
}
