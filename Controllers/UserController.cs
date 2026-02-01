using Daily_Task_Manager.Models.Data_Models;
using Daily_Task_Manager.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using System.Threading.Tasks;
using System.Text.Json;
using System.Linq;


namespace Daily_Task_Manager.Controllers
{
    [Authorize(Policy = "UserOnly")]
    public class UserController : Controller
    {
        private readonly ITaskRepository _taskRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(ITaskRepository taskRepository, UserManager<ApplicationUser> userManager)
        {
            _taskRepository = taskRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);

            
            var tasks = await _taskRepository.GetTasksByUserAsync(user.Id) ?? new List<UserTask>();
            ViewBag.userid = user.Id;
            var count = await _taskRepository.GetTasksByUserAsync(user.Id);
            // Debug output
            Console.WriteLine($"{count.Count()}");
            Console.WriteLine($"Tasks found for user {user.Email}: {tasks.Count()}");

            return View(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> Complete(int id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);
            if (task == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (task.UserId != user.Id)
                return Forbid();

            task.IsCompleted = true;
            await _taskRepository.UpdateTaskAsync(task);

            
            var tasks = await _taskRepository.GetTasksByUserAsync(user.Id);
            return PartialView("_UserTaskTable", tasks);
        }
        public async Task<IActionResult> UserDashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            var tasks = await _taskRepository.GetTasksByUserAsync(user.Id) ?? new List<UserTask>();    
            return View(tasks);
        }

        public async Task<IActionResult> LoadTasksAjax()
        {
            var user = await _userManager.GetUserAsync(User);
            var tasks = await _taskRepository.GetTasksByUserAsync(user.Id);
            return PartialView("_UserTaskTable", tasks);
        }
    }
}
