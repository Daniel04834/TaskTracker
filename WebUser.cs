using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;

namespace TaskTracker
{
    public class WebUser
    {
        public static List<WebUser> webUsers = new List<WebUser>();
        private HttpContext context => new HttpContextAccessor().HttpContext;

        private readonly ApplicationDbContext _context;
        public WebUser(ApplicationDbContext context) 
        {
            _context = context;
        }

        public void Initialize(int userId, string username)
        {
            HashSet<int> teams = new HashSet<int>();
            HashSet<int> projects = new HashSet<int>();

            foreach (var item in _context.TeamsUsers.Where(x => x.UserId == userId))
                teams.Add(item.TeamId);
            foreach (var item in _context.ProjectsUsers.Where(x => x.UserId == userId))
                teams.Add(item.ProjectId);

            Guid uniqueId = Guid.NewGuid();
            _context.Database.ExecuteSqlRaw($"INSERT INTO sessions (user_id, unique_id) VALUES ({userId}, '{uniqueId.ToString()}')");

            context.Session.SetString("user_id", userId.ToString());
            context.Session.SetString("unique_id", uniqueId.ToString());
            context.Session.SetString("is_logged", "true");
            context.Session.SetString("username", username);
            context.Session.SetString("teams", JsonConvert.SerializeObject(teams));
            context.Session.SetString("projects", JsonConvert.SerializeObject(projects));
            context.Session.SetString("session_unique_id", uniqueId.ToString());

            this.Session = context.Session;
        }

        public int UserId { get; private set; }
        public bool IsLogged { get; private set; }
        public string Username { get; private set; }
        public int[] Teams { get; private set; }
        public int[] Projects { get; private set; }
        public ISession Session { get; private set; }
    }
}
