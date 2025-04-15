using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Models
{
    public class SubTaskModel
    {
        public Guid Id { get; set; }
        public Guid? TaskId { get; set; }
        public string Title { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }

        public Guid CreatedBy { get; set; }

        // Quan hệ điều hướng
        public TaskModel Task { get; set; }
        public UserModel CreatedByUser { get; set; }

        // Nhiều người được giao SubTask này
        public ICollection<SubTaskAssignmentModel> Assignments { get; set; }
    }
}
