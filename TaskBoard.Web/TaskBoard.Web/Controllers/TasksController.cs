using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using TaskBoard.Web.Services;
using TaskBoard.Web.ViewModels;

namespace TaskBoard.Web.Controllers
{
    public class TasksController : Controller
    {
        private readonly ITaskApiClient _api;
        private readonly IMapper _mapper;

        public TasksController(ITaskApiClient api, IMapper mapper)
        {
            _api = api;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var dtos = await _api.GetTasksAsync();
            var vm = new TaskListViewModel
            {
                Items = _mapper.Map<List<TaskItemViewModel>>(dtos)
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskListViewModel form)
        {
            if (string.IsNullOrWhiteSpace(form.NewTitle) || form.NewTitle.Length > 200)
            {
                var dtos = await _api.GetTasksAsync();
                var vm = new TaskListViewModel
                {
                    Items = _mapper.Map<List<TaskItemViewModel>>(dtos),
                    ErrorMessage = "Title is required and must be ≤ 200 characters."
                };
                return View("Index", vm);
            }

            var created = await _api.CreateAsync(form.NewTitle);
            if (created is null)
            {
                var dtos = await _api.GetTasksAsync();
                var vm = new TaskListViewModel
                {
                    Items = _mapper.Map<List<TaskItemViewModel>>(dtos),
                    ErrorMessage = "Failed to create task (API error)."
                };
                return View("Index", vm);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int id, bool isDone)
        {
            var dto = await _api.GetTaskAsync(id);
            if (dto is null) { TempData["Error"] = "Task not found."; return RedirectToAction(nameof(Index)); }

            var ok = await _api.UpdateAsync(id, dto.Title, isDone);
            if (!ok) TempData["Error"] = "Failed to update task.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _api.DeleteAsync(id);
            if (!ok) TempData["Error"] = "Failed to delete task.";
            return RedirectToAction(nameof(Index));
        }
    }
}
