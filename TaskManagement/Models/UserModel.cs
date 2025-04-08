using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Models
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string? GoogleId { get; set; }
        public string? ProfilePicture { get; set; }
        public string AuthProvider { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<GroupMemberModel> GroupMemberships { get; set; } = new();
        public List<TaskModel> OwnedTasks { get; set; } = new();

        public List<SubTaskModel> AssignedTasks { get; set; } = new();

        // Điều hướng tới lời mời
        public ICollection<GroupInvitationModel> SentInvitations { get; set; }
        public ICollection<GroupInvitationModel> ReceivedInvitations { get; set; }

    }

}
