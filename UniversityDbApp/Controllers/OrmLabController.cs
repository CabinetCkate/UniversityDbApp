using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UniversityDbApp.Data;
using UniversityDbApp.Models;

namespace UniversityDbApp.Controllers;
public class OrmLabController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index() { await LoadCategories(); return View(await db.Categories.Include(c => c.Products).OrderBy(c => c.Name).ToListAsync()); }
    public IActionResult CreateCategory() => View(new Category());
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(Category category) { if (!ModelState.IsValid) return View(category); db.Add(category); await db.SaveChangesAsync(); TempData["Success"] = "Категория добавлена."; return RedirectToAction(nameof(Index)); }
    public async Task<IActionResult> CreateProduct() { await LoadCategories(); return View(new Product()); }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProduct(Product product)
    {
        if (!await db.Categories.AnyAsync(c => c.Id == product.CategoryId)) ModelState.AddModelError(nameof(product.CategoryId), "Выберите существующую категорию.");
        if (!ModelState.IsValid) { await LoadCategories(product.CategoryId); return View(product); }
        db.Add(product); await db.SaveChangesAsync(); TempData["Success"] = "Товар добавлен."; return RedirectToAction(nameof(Index));
    }
    private async Task LoadCategories(int? selected = null) => ViewBag.CategoryId = new SelectList(await db.Categories.OrderBy(c => c.Name).ToListAsync(), "Id", "Name", selected);
}
