using Microsoft.AspNetCore.Mvc;
using Daily_Task_Manager.Models.Services;
using Daily_Task_Manager.Models.Interfaces;
using System.Linq;

namespace Daily_Task_Manager.ViewComponents
{
    public class TaskCount: ViewComponent
    {
        private readonly ITaskRepository _taskRepository;

        public TaskCount(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(string UserId)
        {
            var tasks = await _taskRepository.GetTasksByUserAsync(UserId);
            int count = tasks.Count();
            return View(count);
        }
    }
}
