namespace Daily_Task_Manager.Models.Data_Models
{
    public class UserTask
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public string UserId { get; set; }
        public ApplicationUser? User { get; set; } = null;
    }
}
