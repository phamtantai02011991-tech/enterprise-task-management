using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PretestWDA.Data;
using PretestWDA.Models;

namespace PretestWDA.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. ViewEmployeeList (Figure 1)
        [HttpGet]
        public async Task<IActionResult> ViewEmployeeList()
        {
            var employees = await _context.tbEmployees.ToListAsync();
            return View(employees);
        }

        // 2. AddNewEmployee (Figure 3)
        [HttpGet]
        public IActionResult AddNewEmployee()
        {
            var model = new tbEmployee
            {
                EmpDoB = new DateTime(1989, 1, 1)
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewEmployee(tbEmployee employee)
        {
            if (!ModelState.IsValid)
            {
                return View(employee);
            }

            _context.tbEmployees.Add(employee);
            await _context.SaveChangesAsync();

            ViewBag.Message = "Employee added successful.";
            return View(employee);
        }

        // 3. EditOrDeleteEmployee (Figure 2)
        [HttpGet]
        public async Task<IActionResult> EditOrDeleteEmployee(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.tbEmployees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> EditOrDeleteEmployee(tbEmployee employee, string submitButton)
        {
            if (submitButton == "Remove")
            {
                var empToDelete = await _context.tbEmployees.FindAsync(employee.EmpID);
                if (empToDelete != null)
                {
                    _context.tbEmployees.Remove(empToDelete);
                    await _context.SaveChangesAsync();
                }
                ViewBag.Message = "Employee deleted successful.";
                return View(new tbEmployee());
            }

            if (!ModelState.IsValid)
            {
                return View(employee);
            }

            _context.Entry(employee).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            ViewBag.Message = "Employee updated successful.";
            return View(employee);
        }
    }
}
