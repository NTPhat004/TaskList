using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Models
{
    public class GroupInvitationModel
    {
        // Enum định nghĩa các trạng thái của lời mời
        public enum InvitationStatus
        {
            Pending = 0,    // Chờ xác nhận
            Accepted = 1,   // Đã chấp nhận
            Rejected = 2    // Đã từ chối
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid GroupId { get; set; }
        public GroupModel Group { get; set; }

        public Guid InviterId { get; set; }
        public UserModel Inviter { get; set; }

        public Guid? InviteeId { get; set; }
        public UserModel? Invitee { get; set; }

        public string InviteeEmail { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        // Gán giá trị mặc định cho Status khi tạo mới GroupInvitation
        public InvitationStatus Status { get; set; } = InvitationStatus.Pending;
    }
}
