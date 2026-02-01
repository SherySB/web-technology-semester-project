using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Daily_Task_Manager.Models.Interfaces;
using Daily_Task_Manager.Models.Data_Models;
using Daily_Task_Manager.Hubs;
using Microsoft.AspNetCore.SignalR;
namespace Daily_Task_Manager.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class TaskController : Controller
    {
        private readonly ITaskRepository _taskRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<TaskHub> _hubContext;

        public TaskController(ITaskRepository taskRepository, UserManager<ApplicationUser> userManager, IHubContext<TaskHub> hubContext)
        {
            _taskRepository = taskRepository;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserTask model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Redirect("/Identity/Account/Login");
            }

            model.UserId = user.Id;
            await _taskRepository.AddTaskAsync(model);

            //await _hubContext.Clients.User(user.Id)
            //    .SendAsync("ReceiveTaskNotification", "A new task has been added.");

            return RedirectToAction("Dashboard", "User");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);

            if (task == null || task.UserId != _userManager.GetUserId(User))
            {
                return Unauthorized();
            }

            
            return View(task);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserTask model)
        {
            await _taskRepository.UpdateTaskAsync(model);
            var user = await _userManager.GetUserAsync(User);

           // await _hubContext.Clients.User(user?.Id)
           //     .SendAsync("ReceiveTaskNotification", "A task has been updated.");

            return RedirectToAction("Dashboard", "User");
        }

        public async Task<IActionResult> Details(int id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);

            if (task == null )
            {
                return Unauthorized();
            }

            return View(task);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);
            if (task == null)
                return Unauthorized();
            
            var user = await _userManager.GetUserAsync(User);
            //if (task.UserId != user?.Id)
            //    return Forbid();
            //await _hubContext.Clients.User(user.Id)
            //    .SendAsync("ReceiveTaskNotification", $"Task '{task.Title}' has been deleted.");



            await _taskRepository.DeleteTaskAsync(id);

            await _hubContext.Clients.User(user.Id)
                .SendAsync("TaskDeleted",id);
                
            return Ok();

            //return RedirectToAction("AdminDashboard", "Admin");
        }
        [HttpPost]
        public async Task<IActionResult> Complete(int id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);
            if (task == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (task.UserId != user?.Id)
                return Forbid();

            task.IsCompleted = true;
            await _taskRepository.UpdateTaskAsync(task);

            await _hubContext.Clients.User(user.Id)
                .SendAsync("TaskCompleted", id);

            //var tasks = await _taskRepository.GetTasksByUserAsync(user.Id);
            //return PartialView("_UserTaskTable", tasks);
            return Ok();
        }
    }
}
