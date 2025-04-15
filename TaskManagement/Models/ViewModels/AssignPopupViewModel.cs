namespace TaskManagement.Models.ViewModels
{
    public class AssignPopupViewModel
    {
        public List<UserModel> GroupMembers { get; set; }
        public List<Guid> AssignedUserIds { get; set; }
    }
}
