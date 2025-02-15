using Mysqlx.Crud;
using System.Diagnostics;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace TaskTracker
{
    public class ApiKeyAuth
    {
        private readonly RequestDelegate _next;
        public ApiKeyAuth(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ApplicationDbContext _context)
        {
            if(Configuration.GetValue<bool>("DISABLE_AUTHENTICATION")) { await _next(context); return; }
            
            int userId = Convert.ToInt32(context.Request.Headers["user-id"]);
            string key = context.Request.Headers["x-api-key"];
            bool found = _context.ApiKeys.FirstOrDefault(x => x.Key == key) != null;
            if (!found)
            {
                key = Functions.Decrypt(key, EncryptionKey, EncryptionIv);
                var session = _context.Sessions.ToArray().FirstOrDefault(x => x.UserId == userId && x.UniqueId.ToString() == key);
                found = session != null;
                if (found) session.LastUsed = DateTime.Now;
                _context.SaveChanges();
            }
            if (!found)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid authentication!");
                return;
            }

            await _next(context);
        }
    }
}
