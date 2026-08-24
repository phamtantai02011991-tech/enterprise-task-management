using day08.Core.Interfaces;
using day08.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace day08.Controllers
{
    public class InventoryController : Controller
    {
        private readonly IInventoryService inventoryService;
        public InventoryController(IInventoryService inventoryService)
        {
            this.inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var res = await inventoryService.GetStockListAsync();
            return View(res);
        }

        [HttpGet]
        public async Task<IActionResult> StockTransaction()
        {
            ViewBag.products = await inventoryService.GetStockListAsync();
            return View(new StockTransactionViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> StockTransaction(StockTransactionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.products = await inventoryService.GetStockListAsync();
                return View(model);
            }
            var res = await inventoryService.ProcessStockTransactionAsync(model);
            if (!res.IsSuccess) 
            {
                ModelState.AddModelError(string.Empty, res.Message);
                ViewBag.products = await inventoryService.GetStockListAsync();
                return View(model);
            }
            TempData["SuccessMsg"] = res.Message;
            return RedirectToAction("Index");
        }
    }
}
