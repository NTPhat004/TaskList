using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Models
{
    public class ActivityLogModel
    {
        public enum ActivitySourceType
        {
            Inbox,
            PersonalTask,
            GroupTask
        }
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public ActivitySourceType Source { get; set; } // Enum chỉ nguồn hoạt động
        public string Action { get; set; } // tên hành động: CreatedTask, AcceptedInvitation, etc.
        public string? Details { get; set; } // nội dung mô tả chi tiết
        public DateTime Timestamp { get; set; }

        // Liên kết dữ liệu (tuỳ loại nguồn)
        public Guid? RelatedGroupId { get; set; }
        public Guid? RelatedTaskId { get; set; }
        public Guid? RelatedSubTaskId { get; set; }

        public UserModel User { get; set; }
    }
}
