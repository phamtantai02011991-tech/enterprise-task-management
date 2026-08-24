using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using restest.Data;
using restest.Models;

namespace restest.Controllers
{
    public class PetController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PetController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Pet
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var pets = await _context.Pets.ToListAsync();
            return View(pets);
        }

        // GET: Pet/Create
        [HttpGet]
        public IActionResult Create()
        {
            var pet = new Pet
            {
                Status = PetStatus.Healthy,
                IsActive = true,
                Age = 1,
                Price = 100
            };
            return View("CreateOrEdit", pet);
        }

        // GET: Pet/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _context.Pets.FindAsync(id);
            if (pet == null)
            {
                return NotFound();
            }

            return View("CreateOrEdit", pet);
        }

        // POST: Pet/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(Pet pet)
        {
            if (!ModelState.IsValid)
            {
                return View("CreateOrEdit", pet);
            }

            if (pet.Id == 0)
            {
                _context.Add(pet);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm mới thành công!";
            }
            else
            {
                _context.Update(pet);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Pet/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet != null)
            {
                _context.Pets.Remove(pet);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Pet/DeleteMultiple
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMultiple(List<int> selectedIds)
        {
            if (selectedIds != null && selectedIds.Count > 0)
            {
                var petsToDelete = await _context.Pets.Where(p => selectedIds.Contains(p.Id)).ToListAsync();
                _context.Pets.RemoveRange(petsToDelete);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã xóa {petsToDelete.Count} thú cưng được chọn!";
            }
            else
            {
                TempData["ErrorMessage"] = "Vui lòng chọn ít nhất một mục để xóa!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

