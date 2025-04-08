using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Models
{
    public class RoleModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
