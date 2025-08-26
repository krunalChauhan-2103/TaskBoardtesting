namespace TaskBoard.Web.ViewModels
{
    public class TaskItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public bool IsDone { get; set; }
        public DateTime CreatedUtc { get; set; }
    }
}
