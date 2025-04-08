using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.RegularExpressions;

namespace TaskManagement.Models
{
    
    public class GroupMemberModel
    {
        public enum GroupRole
        {
            Member = 0,
            Admin = 1,
            Viewer = 2
        }
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public Guid UserId { get; set; }
        public GroupRole Role { get; set; }
        public DateTime JoinedAt { get; set; }

        public GroupModel Group { get; set; }
        public UserModel User { get; set; }
    }

}
