using Lab01WebApi.Entites;   

namespace Lab01WebApi.Services          
{
    public class SubjectService
    {
        // Đặt tên rõ ràng hơn cho danh sách tĩnh để tránh xung đột tên
        private static List<Subject> _subjects = new List<Subject>()
        {
            new Subject { code = "7195-PRAM", Name = "Microsoft Azure", Fee = 200 },
            new Subject { code = "7195-ACMF", Name = "MVC",            Fee = 200 },
            new Subject { code = "7195-JAVA", Name = "Scrip",         Fee = 200 }
        };

        // Trả về tất cả các môn học
        public static List<Subject> GetSubjects()
        {
            return _subjects.ToList();   // trả về bản sao để người gọi không thể thay đổi danh sách gốc
        }

        // Trả về một môn học theo mã (hoặc null nếu không tìm thấy)
        public static Subject? GetSubject(string code)
        {
            return _subjects.SingleOrDefault(x => x.code == code);
        }

        // Thêm một môn học mới
        public static void SaveSubject(Subject subject)
        {
            if (subject == null)
                throw new ArgumentNullException(nameof(subject));

            // Tùy chọn: ngăn trùng mã
            if (_subjects.Any(x => x.code == subject.code))
                throw new InvalidOperationException($"Subject with code '{subject.code}' already exists.");

            _subjects.Add(subject);
        }

        // Xóa theo đối tượng Subject (hoặc theo mã – cả hai cách đều phổ biến)
        public static bool DeleteSubject(Subject subject)
        {
            if (subject == null)
                return false;

            var model = _subjects.SingleOrDefault(x => x.code == subject.code);
            if (model != null)
            {
                _subjects.Remove(model);
                return true;
            }
            return false;
        }

        // Overload tiện lợi chỉ xóa theo mã
        public static bool DeleteSubject(string code)
        {
            var model = _subjects.SingleOrDefault(x => x.code == code);
            if (model != null)
            {
                _subjects.Remove(model);
                return true;
            }
            return false;
        }
    }
}