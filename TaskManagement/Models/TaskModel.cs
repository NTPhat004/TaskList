using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace TaskManagement.Models
{
    public class TaskModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public bool IsGroupTask { get; set; }
        public Guid OwnerId { get; set; }
        public Guid? GroupId { get; set; }
        public DateTime CreatedAt { get; set; }

        public UserModel Owner { get; set; }
        public GroupModel? Group { get; set; }
        public List<SubTaskModel> SubTasks { get; set; } = new();
    }

}
