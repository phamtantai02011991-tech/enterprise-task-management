using Lab01WebApi.Entites;
using Lab01WebApi.Services;          
using Microsoft.AspNetCore.Mvc;

namespace Lab01WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectController : ControllerBase
    {
        // GET: api/Subject
        [HttpGet]
        public IActionResult GetSubjects()
        {
            var subjects = SubjectService.GetSubjects();
            return Ok(subjects);
        }

        // GET: api/Subject/{code}
        [HttpGet("{code}")]
        public IActionResult GetSubject(string code)
        {
            var subject = SubjectService.GetSubject(code);

            if (subject == null)
            {
                return NotFound(new { message = $"Không tìm thấy môn học với mã: {code}" });
            }

            return Ok(subject);
        }

        // POST: api/Subject
        [HttpPost]
        public IActionResult CreateSubject([FromBody] Subject subject)
        {
            if (subject == null)
            {
                return BadRequest(new { message = "Dữ liệu môn học không hợp lệ" });
            }

            try
            {
                SubjectService.SaveSubject(subject);
                // Trả về 201 Created + vị trí resource vừa tạo (chuẩn REST)
                return CreatedAtAction(nameof(GetSubject), new { code = subject.code }, subject);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message }); // Trùng mã
            }
            catch (ArgumentNullException)
            {
                return BadRequest(new { message = "Dữ liệu môn học không được null" });
            }
        }

        // DELETE: api/Subject/{code}
        [HttpDelete("{code}")]
        public IActionResult Delete(string code)
        {
            bool deleted = SubjectService.DeleteSubject(code);

            if (!deleted)
            {
                return NotFound(new { message = $"Không tìm thấy môn học với mã: {code} để xóa" });
            }

            return NoContent(); // 204 No Content – chuẩn khi xóa thành công
        }
    }
}