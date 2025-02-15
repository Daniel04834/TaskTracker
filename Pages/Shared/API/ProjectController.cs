using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskTracker.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Task = System.Threading.Tasks.Task;

namespace TaskTracker.Pages.Shared.API
{
    [MiddlewareFilter(typeof(ProjectAuth))]
    [Route("api/[controller]/{projectId}")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectController(ApplicationDbContext context)
        {
            _context = context;
        }
        static ProjectController()
        {
            Task.Run(async () =>
            {
                int id = new Random().Next(0, 100);
                while (true)
                {
                    await Task.Delay(100);
                    if (!actionQueue.TryDequeue(out object[] action)) continue;
                    Console.WriteLine("working " + id);


                    if ((string)action[0] == "AddUpdatedTaskColumn")
                    {
                        updatedTaskColumns.Add((string)action[1], (Entities.TaskColumn)action[2]);
                        Task.Run(async () =>
                        {
                            await Task.Delay(10000);
                            actionQueue.Enqueue(new object[] { "RemoveUpdatedTaskColumn", (string)action[1] });
                        });
                    }
                    else if ((string)action[0] == "AddUpdatedTask")
                    {
                        updatedTasks.Add((string)action[1], (Entities.Task)action[2]);
                        Task.Run(async () =>
                        {
                            await Task.Delay(10000);
                            actionQueue.Enqueue(new object[] { "RemoveUpdatedTask", (string)action[1] });
                        });
                    }
                    else if ((string)action[0] == "RemoveUpdatedTaskColumn")
                    {
                        updatedTaskColumns.Remove((string)action[1]);
                    }
                    else if ((string)action[0] == "RemoveUpdatedTask")
                    {
                        updatedTasks.Remove((string)action[1]);
                    }
                }
            });
        }

        private static ConcurrentQueue<object[]> actionQueue = new ConcurrentQueue<object[]>();

        private static Dictionary<string, Entities.TaskColumn> updatedTaskColumns = new Dictionary<string, Entities.TaskColumn>();
        private static Dictionary<string, Entities.Task> updatedTasks = new Dictionary<string, Entities.Task>();

        [HttpGet("column/get/{columnId}")]
        public ObjectResult GetColumn([FromRoute] int projectId, [FromRoute] int columnId)
        {
            TaskColumn column = _context.TasksColumns.FirstOrDefault(x => x.Id == columnId && x.ProjectId == projectId);
            if (column == null) return StatusCode(400, "Bad request");

            return StatusCode(200, column);
        }
        
        [HttpPost("column/new")]
        public ObjectResult NewColumn([FromRoute] int projectId, string title)
        {
            if (_context.Projects.FirstOrDefault(x => x.Id == projectId) == null) return StatusCode(400, "Bad request");
            int columnId = _context.TasksColumns.FirstOrDefault() != null ? _context.TasksColumns.Max(x => x.Id) + 1 : 1;
            int order = _context.TasksColumns.FirstOrDefault() != null ? _context.TasksColumns.Max(x => x.Order) + 1 : 1;
            Entities.TaskColumn taskColumn = new TaskColumn(columnId, projectId, MySqlHelper.EscapeString(title), order);
            _context.TasksColumns.Add(taskColumn);
            _context.SaveChanges();

            AddUpdatedTaskColumn(taskColumn);

            return StatusCode(200, "OK");
        }
        
        [HttpPatch("column/{columnId}/update")]
        public ObjectResult UpdateColumn([FromRoute] int projectId, [FromRoute] int columnId, string? title, int? order)
        {
            Entities.TaskColumn column = _context.TasksColumns.FirstOrDefault(x => x.Id == columnId && x.ProjectId == projectId);
            if (column == null) return StatusCode(400, "Bad request");

            if (title != null) column.Title = MySqlHelper.EscapeString(title);
            if (order != null) column.Order = (int)order;
            _context.SaveChanges();

            AddUpdatedTaskColumn(column);

            return StatusCode(200, "Completed");
        }
        
        [HttpGet("column/get/updated")]
        public ObjectResult GetUpdatedColumns([FromRoute] int projectId)
        {
            return StatusCode(200, updatedTaskColumns);
        }

        [HttpPost("task/new")]
        public ObjectResult NewTask([FromRoute]int projectId, int columnId, string title, int order)
        {
            int taskId = _context.Tasks.FirstOrDefault() != null ? _context.Tasks.Max(x => x.Id)+1 : 1;
            Entities.Task task = new Entities.Task(taskId, projectId, columnId, MySqlHelper.EscapeString(title), "", order, false);
            _context.Tasks.Add(task);
            _context.SaveChanges();

            AddUpdatedTask(task);

            return StatusCode(200, "OK");
        }
        
        [HttpGet("task/get/{taskId}")]
        public ObjectResult GetTask([FromRoute]int projectId, [FromRoute]int taskId)
        {
            Entities.Task task = _context.Tasks.FirstOrDefault(x => x.Id == taskId);
            if (task == null) return StatusCode(400, "Bad request");

            return StatusCode(200, task);
        }
        
        [HttpGet("task/get/updated")]
        public ObjectResult GetUpdatedTasks([FromRoute]int projectId)
        {
            return StatusCode(200, updatedTasks);
        }
        
        [HttpGet("task/get/all")]
        public ObjectResult GetAllTasks([FromRoute]int projectId)
        {
            Entities.Task[] tasks = _context.Tasks.Where(x => x.ProjectId == projectId).ToArray();
            if (tasks.Length == 0) return StatusCode(400, "Bad request");

            return StatusCode(200, tasks);
        }
        
        [HttpPatch("task/{taskId}/update")]
        public ObjectResult TaskUpdate([FromRoute]int projectId, [FromRoute]int taskId, int? columnId, string? title, string? description, int? order, bool? completed)
        {
            Entities.Task task = _context.Tasks.FirstOrDefault(x => x.Id == taskId && x.ProjectId == projectId);
            if (task == null) return StatusCode(400, "Bad request");

            if (columnId != null) task.ColumnId = (int)columnId;
            if (title != null) task.Title = MySqlHelper.EscapeString(title);
            if (description != null) task.Description = MySqlHelper.EscapeString(description);
            if (order != null) task.Order = (int)order;
            if (completed != null) task.Completed = (bool)completed;
            _context.SaveChanges();

            AddUpdatedTask(task);

            return StatusCode(200, "Completed");
        }

        private bool updatingTaskColumnsBusy = false;
        private void AddUpdatedTaskColumn(TaskColumn taskColumn)
        {
            string guid = Guid.NewGuid().ToString();
            actionQueue.Enqueue(new object[] { "AddUpdatedTaskColumn", guid, taskColumn });
        }

        private bool updatingTasksBusy = false;
        private void AddUpdatedTask(Entities.Task task)
        {
            string guid = Guid.NewGuid().ToString();
            actionQueue.Enqueue(new object[] { "AddUpdatedTask", guid, task });
        }
    }
}
