namespace TaskBoard.Web.ViewModels
{
    public class TaskListViewModel
    {
        public List<TaskItemViewModel> Items { get; set; } = new();
        public string? ErrorMessage { get; set; }
        public string? NewTitle { get; set; }
    }
}
