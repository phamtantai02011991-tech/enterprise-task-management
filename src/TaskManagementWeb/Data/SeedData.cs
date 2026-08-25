using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagementWeb.Data;
using TaskManagementWeb.Models.Entities;
using TaskManagementWeb.Models.Enums;

namespace TaskManagementWeb.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            await context.Database.EnsureCreatedAsync();


            // 1. Seed Roles
            if (!await context.Roles.AnyAsync())
            {
                context.Roles.AddRange(
                    new Role { RoleName = "Admin" },
                    new Role { RoleName = "Manager" },
                    new Role { RoleName = "Employee" }
                );

                await context.SaveChangesAsync();
            }


            // 2. Seed Departments
            if (!await context.Departments.AnyAsync())
            {
                context.Departments.AddRange(
                    new Department
                    {
                        Name = "Software Development & Technology",
                        Code = "DEV",
                        Description = "Develop software solutions and cloud infrastructure"
                    },

                    new Department
                    {
                        Name = "UI/UX Design Department",
                        Code = "UIUX",
                        Description = "Design user experience and product interfaces"
                    },

                    new Department
                    {
                        Name = "Project Management Office",
                        Code = "PMO",
                        Description = "Manage and coordinate enterprise projects"
                    }
                );

                await context.SaveChangesAsync();
            }


            // 3. Seed Users
            if (!await context.Users.AnyAsync())
            {
                var admin = new User
                {
                    FullName = "System Admin",
                    Email = "admin@taskflow.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    RoleId = 1,
                    CreatedAt = DateTime.UtcNow
                };


                var manager = new User
                {
                    FullName = "Project Manager Alex",
                    Email = "manager@taskflow.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager@123"),
                    RoleId = 2,
                    CreatedAt = DateTime.UtcNow
                };


                var employee1 = new User
                {
                    FullName = "Employee John Doe",
                    Email = "employee@taskflow.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee@123"),
                    RoleId = 3,
                    CreatedAt = DateTime.UtcNow
                };


                var employee2 = new User
                {
                    FullName = "Employee Jane Smith",
                    Email = "jane@taskflow.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee@123"),
                    RoleId = 3,
                    CreatedAt = DateTime.UtcNow
                };


                context.Users.AddRange(admin, manager, employee1, employee2);
                await context.SaveChangesAsync();

                // Seed UserDepartments (Many-to-Many)
                var devDept = await context.Departments.FirstOrDefaultAsync(d => d.Code == "DEV");
                var uiuxDept = await context.Departments.FirstOrDefaultAsync(d => d.Code == "UIUX");
                var pmoDept = await context.Departments.FirstOrDefaultAsync(d => d.Code == "PMO");

                if (devDept != null && uiuxDept != null && pmoDept != null)
                {
                    context.UserDepartments.AddRange(
                        new UserDepartment { UserId = admin.Id, DepartmentId = pmoDept.Id, IsPrimary = true },
                        new UserDepartment { UserId = manager.Id, DepartmentId = pmoDept.Id, IsPrimary = true },
                        new UserDepartment { UserId = manager.Id, DepartmentId = devDept.Id, IsPrimary = false },
                        new UserDepartment { UserId = employee1.Id, DepartmentId = devDept.Id, IsPrimary = true },
                        new UserDepartment { UserId = employee1.Id, DepartmentId = uiuxDept.Id, IsPrimary = false },
                        new UserDepartment { UserId = employee2.Id, DepartmentId = uiuxDept.Id, IsPrimary = true }
                    );

                    await context.SaveChangesAsync();
                }
            }



            // 4. Seed Projects & Tasks
            if (!await context.Projects.AnyAsync())
            {
                var managerUser = await context.Users
                    .FirstOrDefaultAsync(u => u.Email == "manager@taskflow.com");

                var emp1 = await context.Users
                    .FirstOrDefaultAsync(u => u.Email == "employee@taskflow.com");

                var emp2 = await context.Users
                    .FirstOrDefaultAsync(u => u.Email == "jane@taskflow.com");

                var adminUser = await context.Users
                    .FirstOrDefaultAsync(u => u.Email == "admin@taskflow.com");

                var existingProjects = await context.Projects.ToListAsync();
                if (existingProjects.Any())
                {
                    var p1 = existingProjects.FirstOrDefault(p => p.Id == 1);
                    if (p1 != null)
                    {
                        p1.Status = ProjectStatus.Active;
                        p1.Priority = ProjectPriority.High;
                        if (managerUser != null) p1.ManagerId = managerUser.Id;
                    }

                    var p2 = existingProjects.FirstOrDefault(p => p.Id == 2);
                    if (p2 != null)
                    {
                        p2.Status = ProjectStatus.Planning;
                        p2.Priority = ProjectPriority.Medium;
                        if (managerUser != null) p2.ManagerId = managerUser.Id;
                    }

                    // Add a completed project if not exists
                    if (!existingProjects.Any(p => p.Status == ProjectStatus.Completed))
                    {
                        var p3 = new Project
                        {
                            ProjectCode = "PRJ-2026-000",
                            Title = "Legacy Database Migration & Infrastructure Setup",
                            Description = "Migrated legacy SQL Server database to new Cloud Infrastructure with 100% data integrity.",
                            StartDate = DateTime.Today.AddDays(-60),
                            EndDate = DateTime.Today.AddDays(-10),
                            Status = ProjectStatus.Completed,
                            Priority = ProjectPriority.High,
                            ManagerId = managerUser?.Id,
                            Progress = 100,
                            CreatedByUserId = adminUser?.Id ?? 1,
                            CreatedAt = DateTime.UtcNow.AddDays(-60),
                            UpdatedAt = DateTime.UtcNow.AddDays(-10)
                        };
                        context.Projects.Add(p3);
                    }

                    await context.SaveChangesAsync();
                }

                if (managerUser != null && !existingProjects.Any())
                {
                    var project1 = new Project
                    {
                        ProjectCode = "PRJ-2026-001",
                        Title = "Task Management Web Application (Mini Project 1)",
                        Description = "Build a task management system using ASP.NET Core MVC, Entity Framework Core, and SQL Server 2022.",
                        StartDate = DateTime.Today.AddDays(-7),
                        EndDate = DateTime.Today.AddDays(23),
                        Status = ProjectStatus.Active,
                        Priority = ProjectPriority.High,
                        ManagerId = managerUser.Id,
                        Progress = 35,
                        CreatedByUserId = managerUser.Id,
                        CreatedAt = DateTime.UtcNow.AddDays(-7),
                        UpdatedAt = DateTime.UtcNow
                    };


                    var project2 = new Project
                    {
                        ProjectCode = "PRJ-2026-002",
                        Title = "E-Commerce Microservices System",
                        Description = "Research and design a microservices infrastructure for electronic payment services.",
                        StartDate = DateTime.Today,
                        EndDate = DateTime.Today.AddDays(60),
                        Status = ProjectStatus.Planning,
                        Priority = ProjectPriority.Medium,
                        ManagerId = managerUser.Id,
                        Progress = 0,
                        CreatedByUserId = managerUser.Id,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };


                    context.Projects.AddRange(project1, project2);

                    await context.SaveChangesAsync();


                    if (emp1 != null && emp2 != null && adminUser != null)
                    {
                        var task1 = new TaskItem
                        {
                            Title = "Design SQL Server Database & ERD",
                            Description = "Design Roles, Users, Projects, Tasks tables and their foreign key relationships.",
                            Priority = TaskPriority.High,
                            Status = TaskStatusEnum.Completed,
                            Deadline = DateTime.Today.AddDays(-2),
                            ProjectId = project1.Id,
                            AssignedUserId = emp1.Id
                        };


                        var task2 = new TaskItem
                        {
                            Title = "Develop ASP.NET Core Controllers & Services",
                            Description = "Build MVC Controllers and connect them with application services.",
                            Priority = TaskPriority.High,
                            Status = TaskStatusEnum.InProgress,
                            Deadline = DateTime.Today.AddDays(3),
                            ProjectId = project1.Id,
                            AssignedUserId = emp1.Id
                        };


                        context.TaskItems.AddRange(task1, task2);

                        await context.SaveChangesAsync();



                        // 5. Seed TimeLogs
                        context.TimeLogs.AddRange(
                            new TimeLog
                            {
                                TaskItemId = task1.Id,
                                UserId = emp1.Id,
                                HoursSpent = 4.5f,
                                Description = "Analyzed ERD design and created database migration"
                            },

                            new TimeLog
                            {
                                TaskItemId = task2.Id,
                                UserId = emp1.Id,
                                HoursSpent = 6.0f,
                                Description = "Developed MVC Controllers and Services"
                            }
                        );



                        // 6. Seed Project Files
                        context.ProjectFiles.AddRange(
                            new ProjectFile
                            {
                                ProjectId = project1.Id,
                                UploadedByUserId = managerUser.Id,
                                FileName = "Project_Requirement_Doc.pdf",
                                FilePath = "/uploads/Project_Requirement_Doc.pdf",
                                FileSize = 2450000
                            },

                            new ProjectFile
                            {
                                ProjectId = project1.Id,
                                UploadedByUserId = emp2.Id,
                                FileName = "UI_Design.png",
                                FilePath = "/uploads/UI_Design.png",
                                FileSize = 1820000
                            }
                        );



                        // 7. Seed Chat Messages
                        context.ChatMessages.AddRange(
                            new ChatMessage
                            {
                                SenderId = managerUser.Id,
                                ReceiverId = emp1.Id,
                                MessageText = "Hi John, how is the Task Management project progress?",
                                SentAt = DateTime.UtcNow.AddHours(-3)
                            },

                            new ChatMessage
                            {
                                SenderId = emp1.Id,
                                ReceiverId = managerUser.Id,
                                MessageText = "I have completed the database design and I am working on the controllers.",
                                SentAt = DateTime.UtcNow.AddHours(-2)
                            },

                            new ChatMessage
                            {
                                SenderId = adminUser.Id,
                                ReceiverId = managerUser.Id,
                                MessageText = "Alex, please review this week's KPI report.",
                                SentAt = DateTime.UtcNow.AddHours(-1)
                            }
                        );



                        // 8. Seed Notifications for all users if not exists
                        if (context.Notifications.Count() < 4)
                        {
                            context.Notifications.AddRange(
                                new Notification
                                {
                                    UserId = adminUser.Id,
                                    Title = "Hệ thống Quản trị sẵn sàng",
                                    Message = "Toàn bộ dữ liệu dự án, nhân sự và phòng ban đã được đồng bộ thành công.",
                                    IsRead = false,
                                    CreatedAt = DateTime.UtcNow.AddHours(-1)
                                },
                                new Notification
                                {
                                    UserId = managerUser.Id,
                                    Title = "Bổ nhiệm Trưởng dự án",
                                    Message = "Admin đã bổ nhiệm bạn làm Trưởng dự án [PRJ-2026-001] Task Management Web Application.",
                                    IsRead = false,
                                    CreatedAt = DateTime.UtcNow.AddHours(-2)
                                },
                                new Notification
                                {
                                    UserId = emp1.Id,
                                    Title = "Phân công Công việc mới",
                                    Message = "Bạn vừa được phân công thực hiện công việc [Develop ASP.NET Core Controllers & Services].",
                                    IsRead = false,
                                    CreatedAt = DateTime.UtcNow.AddMinutes(-30)
                                },
                                new Notification
                                {
                                    UserId = emp2.Id,
                                    Title = "Tham gia Dự án mới",
                                    Message = "Bạn đã được thêm vào đội ngũ dự án [PRJ-2026-002] E-Commerce Microservices System.",
                                    IsRead = false,
                                    CreatedAt = DateTime.UtcNow.AddHours(-4)
                                }
                            );
                        }

                        // 9. Seed Announcements
                        if (!context.Announcements.Any())
                        {
                            context.Announcements.AddRange(
                                new Announcement
                                {
                                    Title = "Chào mừng đến với Hệ thống Quản trị Doanh nghiệp TaskManagement 2026",
                                    Content = "Hệ thống Quản trị Dự án & Phân công Công việc toàn diện đã chính thức hoạt động. Mọi thành viên vui lòng cập nhật tiến độ công việc hàng ngày.",
                                    Type = "Info",
                                    IsActive = true,
                                    IsPinned = true,
                                    CreatedAt = DateTime.UtcNow,
                                    CreatedByUserId = adminUser.Id
                                },
                                new Announcement
                                {
                                    Title = "Lịch nghiệm thu và rà soát tiến độ các dự án Quý 3",
                                    Content = "Đề nghị các Trưởng dự án (Project Managers) rà soát lại toàn bộ Tasks và đôn đốc thành viên nộp báo cáo nghiệm thu đúng hạn.",
                                    Type = "Warning",
                                    IsActive = true,
                                    IsPinned = false,
                                    CreatedAt = DateTime.UtcNow.AddHours(-5),
                                    CreatedByUserId = adminUser.Id
                                }
                            );
                        }

                        await context.SaveChangesAsync();
                    }
                }
            }
        }


        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();

            var hashedBytes = sha256.ComputeHash(
                Encoding.UTF8.GetBytes(password)
            );

            return Convert.ToBase64String(hashedBytes);
        }
    }
}