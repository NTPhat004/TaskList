using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Models
{
    public class SubTaskModel
    {
        public Guid Id { get; set; }
        public Guid? TaskId { get; set; }
        public string Title { get; set; }
        public Guid? AssignedTo { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }

        public TaskModel Task { get; set; }
        public UserModel? Assignee { get; set; }
    }
}
