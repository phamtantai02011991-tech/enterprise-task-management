using day03.DTOs;
using day03.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace day03.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudentService service;
        public StudentController(IStudentService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index(StudentFilterDto filter)
        {
            var subjects =await service.GetAllSubjectsAsync();
            var scores =await service.GetFilteredScoresAsync(filter);
            var viewModel = new StudentViewModel
            {
                Filter = filter,
                Subjects = new SelectList(subjects,"SubjectId", "SubjectName",filter.SubjectId),
                Scores = scores
            };
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await service.GetScoreForEditAsync(id);
            if (dto == null) return NotFound();
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(StudentScoreDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            var success = await service.UpdateScoreAsync(dto);
            if (success)
            {
                TempData["msg"] = "Done";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["msg"] = "Fail";
            }
          return View(dto);
        }
    }
}
