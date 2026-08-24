using day04.Models;
using day04.Services;
using Microsoft.AspNetCore.Mvc;

namespace day04.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService productService;
        public ProductController(IProductService productService)
        {
            this.productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? keyword)
        {
            var model = await productService.GetIndexDataAsync(keyword);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var model = await productService.GetByIdAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product newProduct, IFormFile? image)
        {
            if (!ModelState.IsValid) 
            {
                return View(newProduct);
            }
            try
            {
                await productService.CreateAsync(newProduct, image);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("images", ex.Message);
                return View(newProduct);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ActionName("Edit")]
        public async Task<IActionResult> Update(int id, Product product, IFormFile image)
        {
            if(id != product.Id)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return View(product);
            }
            try
            {
                await productService.UpdateAsync(product, image);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("images", ex.Message);
                return View(product);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            await productService.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
