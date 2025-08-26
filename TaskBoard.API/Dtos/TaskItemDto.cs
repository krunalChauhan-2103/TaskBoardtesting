namespace TaskBoard.API.Dtos
{
    /// <summary>Transport contract for TaskItem across the wire.</summary>
    public sealed class TaskItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public bool IsDone { get; set; }
        public DateTime CreatedUtc { get; set; }
    }
}
