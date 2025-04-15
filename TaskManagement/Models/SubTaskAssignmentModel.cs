namespace TaskManagement.Models
{
    public class SubTaskAssignmentModel
    {
        public Guid Id { get; set; }
        public Guid SubTaskId { get; set; }
        public Guid UserId { get; set; }

        public SubTaskModel SubTask { get; set; }
        public UserModel User { get; set; }
    }
}
