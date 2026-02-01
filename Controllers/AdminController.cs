using Daily_Task_Manager.Models.Data_Models;
using Daily_Task_Manager.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Daily_Task_Manager.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITaskRepository _taskRepository;

        public AdminController(UserManager<ApplicationUser> userManager, ITaskRepository taskRepository)
        {
            _userManager = userManager;
            _taskRepository = taskRepository;
        }

        public async Task<IActionResult> AdminDashboard()
        {
            int TotalUsers = _userManager.Users.Count();
            var tasks = await _taskRepository.GetAllTasksAsync();
            int TotalTasks= tasks.Count();
            var model = new
            {
                TotalUsers,
                TotalTasks
            };

            return View("AdminDashboard", model);
        }

        public IActionResult ManageUsers()
        {
            var users = _userManager.Users.ToList();
            return View(users);  
        }

        public async Task<IActionResult> AllTasks()
        {
            var tasks = await _taskRepository.GetAllTasksAsync();
            return View(tasks);   
        }

        public async Task<IActionResult> UserTasks(string id)
        {
            if (id == null)
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var tasks = await _taskRepository.GetTasksByUserAsync(id);

            ViewBag.UserName = user.Email;

            var model = new
            {
                User = user,
                Tasks = tasks
            };

            return View("UserTasks", model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> LoadAllTasksAjax()
        {
            var tasks = await _taskRepository.GetAllTasksAsync();
            return PartialView("_AllTasksTable", tasks);
        }
    }
}
