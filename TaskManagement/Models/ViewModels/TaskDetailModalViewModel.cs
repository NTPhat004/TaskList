namespace TaskManagement.Models.ViewModels
{
    public class TaskDetailModalViewModel
    {
        public SubTaskModel SubTask { get; set; }
        public List<TaskModel> Task { get; set; }
        public AssignPopupViewModel AssignPopupViewModel { get; set; }
    }
}
