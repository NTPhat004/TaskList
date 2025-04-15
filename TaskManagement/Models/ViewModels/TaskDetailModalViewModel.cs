namespace TaskManagement.Models.ViewModels
{
    public class TaskDetailModalViewModel
    {
        public SubTaskModel SubTask { get; set; }
        public List<GroupMemberModel> GroupMembers { get; set; }
        public List<TaskModel> Task { get; set; }
    }
}
