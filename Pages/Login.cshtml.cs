using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace TaskTracker.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public LoginModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public void OnPost() 
        {
            Username = MySqlHelper.EscapeString(Username);
            string passwordMd5 = Functions.HashMD5(Password);

            Entities.User userEntity = _context.Users.FirstOrDefault(x => x.Username == Username && x.Pass == passwordMd5);
            if (userEntity != null) {
                WebUser user = new WebUser(_context);
                user.Initialize(userEntity.Id, Username);
                Response.Redirect("/index");
                return;
            }

            Response.Redirect("/notexist");
            return;
        }

        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }
    }
}
