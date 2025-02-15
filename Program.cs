global using static TaskTracker.Program;
global using TaskTracker.Entities;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Mvc;
using Google.Protobuf.WellKnownTypes;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;
using System;

namespace TaskTracker
{
    public class Program
    {
        public static ApplicationDbContext db;

        public static IConfigurationRoot Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        public static double SessionTimeoutMinutes = -1;
        public static string EncryptionKey = "";
        public static string EncryptionIv = "";
        public static int EncryptingIterations = 1;
        public static string UniqueIdSalt = Guid.NewGuid().ToString();

        public static List<WebUser> ActiveWebUsers = new List<WebUser>();

        public static void Main(string[] args)
        {
            SessionTimeoutMinutes = Configuration.GetValue<double>("SessionTimeoutMinutes");
            EncryptionKey = Configuration.GetValue<string>("EncryptionKey");
            EncryptionIv = Configuration.GetValue<string>("EncryptionIv");
            EncryptingIterations = Configuration.GetValue<int>("EncryptingIterations");

            var builder = WebApplication.CreateBuilder(args);
            
            // Add services to the container.
            builder.Services.AddRazorPages();

            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(SessionTimeoutMinutes);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddControllers();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options => {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();

            app.MapRazorPages();

            app.UseWhen(context => context.Request.Path.StartsWithSegments("/api"), appBuilder => { appBuilder.UseMiddleware<ApiKeyAuth>(); });
            app.UseWhen(context => 
                context.Request.Path.StartsWithSegments("/index") || 
                context.Request.Path.StartsWithSegments("/project") ||
                context.Request.Path == "/", appBuilder => { appBuilder.UseMiddleware<SessionAuth>(); });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            Task.Run(() => DeadSessionCleaner(dbContext));

            app.Run();
        }

        private static async Task DeadSessionCleaner(ApplicationDbContext _context)
        {
            while (true)
            {
                Console.WriteLine("Cleaner working");
                await Task.Delay(60000);
                Console.WriteLine(_context.Sessions.ToArray().Length);
                bool remove = false;
                foreach (var session in _context.Sessions.ToArray())
                {
                    Console.WriteLine((DateTime.Now - session.LastUsed).TotalMinutes);
                    if ((DateTime.Now - session.LastUsed).TotalMinutes < SessionTimeoutMinutes) continue;
                    _context.Sessions.Remove(session);
                    remove = true;
                }
                if(remove) _context.SaveChanges();
            }
        }
    }
}
