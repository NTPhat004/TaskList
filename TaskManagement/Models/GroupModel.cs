using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Models
{
    public class GroupModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public UserModel Creator { get; set; }
        public List<GroupMemberModel> Members { get; set; } = new();
        public List<TaskModel>? Tasks { get; set; } = new();
    }
}
