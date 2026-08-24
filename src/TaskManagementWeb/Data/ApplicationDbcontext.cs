using Microsoft.EntityFrameworkCore;
using TaskManagementWeb.Models.Entities;

namespace TaskManagementWeb.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }
        public DbSet<ProjectFile> ProjectFiles { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<TimeLog> TimeLogs { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<UserDepartment> UserDepartments { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<NotificationPreference> NotificationPreferences { get; set; }
        public DbSet<ProjectMemberReport> ProjectMemberReports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // UserDepartment Composite Primary Key & Relationships
            modelBuilder.Entity<UserDepartment>()
                .HasKey(ud => new { ud.UserId, ud.DepartmentId });

            modelBuilder.Entity<UserDepartment>()
                .HasOne(ud => ud.User)
                .WithMany(u => u.UserDepartments)
                .HasForeignKey(ud => ud.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserDepartment>()
                .HasOne(ud => ud.Department)
                .WithMany(d => d.UserDepartments)
                .HasForeignKey(ud => ud.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique Email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Unique Department Code
            modelBuilder.Entity<Department>()
                .HasIndex(d => d.Code)
                .IsUnique();

            // Unique ProjectMember
            modelBuilder.Entity<ProjectMember>()
                .HasIndex(pm => new { pm.ProjectId, pm.UserId })
                .IsUnique();
            // Project CreatedByUser relationship
            modelBuilder.Entity<Project>()
                .HasOne(p => p.CreatedByUser)
                .WithMany(u => u.CreatedProjects)
                .HasForeignKey(p => p.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Project ManagerUser relationship
            modelBuilder.Entity<Project>()
                .HasOne(p => p.ManagerUser)
                .WithMany()
                .HasForeignKey(p => p.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // ProjectCode Unique Index
            modelBuilder.Entity<Project>()
                .HasIndex(p => p.ProjectCode)
                .IsUnique();
            // ChatMessage Sender relationship
            modelBuilder.Entity<ChatMessage>()
                .HasOne(cm => cm.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(cm => cm.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
            // ChatMessage Receiver relationship
            modelBuilder.Entity<ChatMessage>()
                .HasOne(cm => cm.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(cm => cm.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            //avoid cascade path
            modelBuilder.Entity<AuditLog>()
                .HasOne(a => a.User)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AuditLog>()
                .HasOne(a => a.Project)
                .WithMany(p => p.AuditLogs)
                .HasForeignKey(a => a.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Announcement>()
                .HasOne(a => a.CreatedByUser)
                .WithMany()
                .HasForeignKey(a => a.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NotificationPreference>()
                .HasOne(np => np.User)
                .WithMany()
                .HasForeignKey(np => np.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectMemberReport>()
                .HasOne(pmr => pmr.Project)
                .WithMany()
                .HasForeignKey(pmr => pmr.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectMemberReport>()
                .HasOne(pmr => pmr.ReporterManager)
                .WithMany()
                .HasForeignKey(pmr => pmr.ReporterManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectMemberReport>()
                .HasOne(pmr => pmr.TargetUser)
                .WithMany()
                .HasForeignKey(pmr => pmr.TargetUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}