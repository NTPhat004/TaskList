using Microsoft.EntityFrameworkCore;
using System.Data;
using TaskManagement.Models;
using System.Text.RegularExpressions;
namespace TaskManagement.Data
{
    public class TaskListDbContext : DbContext
    {
        public TaskListDbContext(DbContextOptions<TaskListDbContext> options) : base(options) { }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<GroupModel> Groups { get; set; }
        public DbSet<GroupMemberModel> GroupMembers { get; set; }
        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<SubTaskModel> SubTasks { get; set; }
        public DbSet<ActivityLogModel> ActivityLogs { get; set; }
        public DbSet<GroupInvitationModel> GroupInvitation { get; set; }
        public DbSet<SubTaskAssignmentModel> SubTaskAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1️ Cấu hình bảng Users
            modelBuilder.Entity<UserModel>(entity =>
            {
                entity.ToTable("Users");

                entity.HasKey(u => u.Id);

                entity.Property(u => u.Username)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(u => u.Email)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.Property(u => u.GoogleId)
                      .HasMaxLength(100);

                entity.Property(u => u.ProfilePicture)
                      .HasMaxLength(500);

                entity.Property(u => u.AuthProvider)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(u => u.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                // Quan hệ với GroupMemberModel (1 User có thể thuộc nhiều nhóm)
                entity.HasMany(u => u.GroupMemberships)
                      .WithOne(gm => gm.User)
                      .HasForeignKey(gm => gm.UserId)
                      .OnDelete(DeleteBehavior.Cascade); // Khi User bị xóa, membership cũng bị xóa

                // Quan hệ với TaskModel (1 User có thể sở hữu nhiều Task)
                entity.HasMany(u => u.OwnedTasks)
                      .WithOne(t => t.Owner)
                      .HasForeignKey(t => t.OwnerId)
                      .OnDelete(DeleteBehavior.Restrict); // Tránh vòng lặp xóa

                entity.HasMany(u => u.AssignedSubTasks)
                      .WithOne(a => a.User)
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 3️ Cấu hình bảng Groups
            modelBuilder.Entity<GroupModel>(entity =>
            {
                entity.ToTable("Groups");

                entity.HasKey(g => g.Id);

                entity.Property(g => g.Name)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.Property(g => g.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                // Quan hệ với UserModel (người tạo nhóm)
                entity.HasOne(g => g.Creator)
                      .WithMany()
                      .HasForeignKey(g => g.CreatedBy)
                      .OnDelete(DeleteBehavior.Restrict); // Tránh vòng lặp xóa

                // Quan hệ với GroupMemberModel (1 nhóm có nhiều thành viên)
                entity.HasMany(g => g.Members)
                      .WithOne(gm => gm.Group)
                      .HasForeignKey(gm => gm.GroupId)
                      .OnDelete(DeleteBehavior.Cascade); // Khi Group bị xóa, các GroupMember cũng bị xóa

                // Quan hệ với TaskModel (1 nhóm có thể có nhiều Task)
                entity.HasMany(g => g.Tasks)
                      .WithOne(t => t.Group)
                      .HasForeignKey(t => t.GroupId)
                      .OnDelete(DeleteBehavior.Cascade); // Khi Group bị xóa, các Task của nhóm cũng bị xóa
            });

            // 4️ Cấu hình bảng GroupMembers
            modelBuilder.Entity<GroupMemberModel>(entity =>
            {
                entity.ToTable("GroupMembers");

                entity.HasKey(gm => gm.Id);

                entity.Property(gm => gm.JoinedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                // Quan hệ với GroupModel (1 Group có nhiều thành viên)
                entity.HasOne(gm => gm.Group)
                      .WithMany(g => g.Members)
                      .HasForeignKey(gm => gm.GroupId)
                      .OnDelete(DeleteBehavior.Cascade); // Khi Group bị xóa, các GroupMember cũng bị xóa

                // Quan hệ với UserModel (1 User có thể thuộc nhiều nhóm)
                entity.HasOne(gm => gm.User)
                      .WithMany(u => u.GroupMemberships)
                      .HasForeignKey(gm => gm.UserId)
                      .OnDelete(DeleteBehavior.Restrict); // Tránh vòng lặp xóa

                entity.Property(gm => gm.Role)
                      .HasConversion<int>(); // Lưu enum dưới dạng int
            });

            // 5️ Cấu hình bảng Tasks
            modelBuilder.Entity<TaskModel>(entity =>
            {
                entity.ToTable("Tasks");

                entity.HasKey(t => t.Id);

                entity.Property(t => t.Title)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.Property(t => t.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                // Quan hệ với UserModel (1 User có thể sở hữu nhiều Task)
                entity.HasOne(t => t.Owner)
                      .WithMany(u => u.OwnedTasks)
                      .HasForeignKey(t => t.OwnerId)
                      .OnDelete(DeleteBehavior.Restrict); // Tránh vòng lặp xóa

                // Quan hệ với GroupModel (1 Task có thể thuộc về 1 Group)
                entity.HasOne(t => t.Group)
                      .WithMany(g => g.Tasks)
                      .HasForeignKey(t => t.GroupId)
                      .OnDelete(DeleteBehavior.Cascade); // Khi Group bị xóa, Task trong nhóm cũng bị xóa

                // Quan hệ với SubTaskModel (1 Task có thể có nhiều SubTask)
                entity.HasMany(t => t.SubTasks)
                      .WithOne(st => st.Task)
                      .HasForeignKey(st => st.TaskId)
                      .OnDelete(DeleteBehavior.Cascade); // Khi Task bị xóa, các SubTask cũng bị xóa
            });

            // 6️ Cấu hình bảng SubTasks
            modelBuilder.Entity<SubTaskModel>(entity =>
            {
                entity.ToTable("SubTasks");

                entity.HasKey(st => st.Id);

                entity.Property(st => st.Title)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.Property(st => st.DueDate)
                      .IsRequired(false);

                entity.Property(st => st.IsCompleted)
                      .HasDefaultValue(false);

                entity.Property(st => st.CompletedAt)
                      .IsRequired(false);

                entity.Property(st => st.CreatedBy)
                        .IsRequired();

                entity.HasOne(s => s.CreatedByUser)
                      .WithMany()
                      .HasForeignKey(s => s.CreatedBy)
                      .OnDelete(DeleteBehavior.Restrict);

                // Quan hệ với TaskModel (1 Task có nhiều SubTask)
                entity.HasOne(s => s.Task)
                         .WithMany(t => t.SubTasks)
                         .HasForeignKey(s => s.TaskId)
                         .OnDelete(DeleteBehavior.Cascade); // Xóa Task thì SubTask cũng bị xóa
            });

            // 7️ Cấu hình bảng ActivityLogs
            modelBuilder.Entity<ActivityLogModel>(entity =>
            {
                entity.ToTable("ActivityLogs");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Action)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Details)
                    .HasMaxLength(1000);

                entity.Property(e => e.Timestamp)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.Source)
                    .HasConversion<string>() // Lưu Enum dưới dạng string
                    .IsRequired()
                    .HasMaxLength(50);

                // Khóa ngoại đến User
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Có thể thêm Index để truy vấn nhanh theo User hoặc Source
                entity.HasIndex(e => e.UserId);

                // Không bắt buộc nhưng vẫn có thể map các liên kết nếu muốn sử dụng navigation sau này
                entity.Property(e => e.RelatedGroupId).IsRequired(false);
                entity.Property(e => e.RelatedTaskId).IsRequired(false);
                entity.Property(e => e.RelatedSubTaskId).IsRequired(false);
            });

            // 8 Cấu hình GroupInvitation
            modelBuilder.Entity<GroupInvitationModel>(entity =>
            {
                entity.ToTable("GroupInvitation");

                entity.HasKey(e => e.Id);

                // Quan hệ với Group
                entity.HasOne(e => e.Group)
                      .WithMany()
                      .HasForeignKey(e => e.GroupId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Quan hệ với Inviter (người gửi)
                entity.HasOne(e => e.Inviter)
                      .WithMany(u => u.SentInvitations)
                      .HasForeignKey(e => e.InviterId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Quan hệ với Invitee (người nhận)
                entity.HasOne(e => e.Invitee)
                      .WithMany(u => u.ReceivedInvitations)
                      .HasForeignKey(e => e.InviteeId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Email người nhận (bắt buộc)
                entity.Property(e => e.InviteeEmail)
                      .IsRequired()
                      .HasMaxLength(256);

                // Enum lưu dưới dạng int
                entity.Property(e => e.Status)
                      .HasConversion<int>()
                      .IsRequired();

                // Thời điểm gửi
                entity.Property(e => e.SentAt)
                      .IsRequired();
            });

            // 9 Cấu hình SubTaskAssignment
            modelBuilder.Entity<SubTaskAssignmentModel>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Quan hệ: Một SubTask có nhiều SubTaskAssignment
                entity.HasOne(e => e.SubTask)
                    .WithMany(t => t.Assignments)
                    .HasForeignKey(e => e.SubTaskId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Quan hệ: Một User có nhiều SubTaskAssignment
                entity.HasOne(e => e.User)
                    .WithMany(u => u.AssignedSubTasks)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.SubTaskId, e.UserId }).IsUnique();
            });
        }
    }
}
