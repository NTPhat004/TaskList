using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Models
{
    public class ActivityLogModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid SubTaskId { get; set; }
        public string Action { get; set; }
        public DateTime Timestamp { get; set; }

        public UserModel User { get; set; }
        public SubTaskModel SubTask { get; set; }
    }
}
