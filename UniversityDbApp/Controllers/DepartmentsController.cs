using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityDbApp.Data;
using UniversityDbApp.Models;

namespace UniversityDbApp.Controllers;
public class DepartmentsController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index() => View(await db.Departments.Include(d => d.Teachers).OrderBy(d => d.Name).ToListAsync());
    public async Task<IActionResult> Details(int id) { var d = await db.Departments.Include(x => x.Teachers).FirstOrDefaultAsync(x => x.Id == id); return d is null ? NotFound() : View(d); }
    public IActionResult Create() => View(new Department());
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Department department)
    {
        if (!ModelState.IsValid) return View(department);
        department.CreatedAt = department.UpdatedAt = DateTime.UtcNow; db.Add(department);
        try { await db.SaveChangesAsync(); TempData["Success"] = "Кафедра создана."; return RedirectToAction(nameof(Index)); }
        catch (DbUpdateException) { ModelState.AddModelError(nameof(department.Code), "Код уже используется."); return View(department); }
    }
    public async Task<IActionResult> Edit(int id) { var d = await db.Departments.FindAsync(id); return d is null ? NotFound() : View(d); }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Department department)
    {
        if (id != department.Id) return BadRequest(); if (!ModelState.IsValid) return View(department);
        department.UpdatedAt = DateTime.UtcNow; db.Update(department);
        try { await db.SaveChangesAsync(); TempData["Success"] = "Изменения сохранены."; return RedirectToAction(nameof(Index)); }
        catch (DbUpdateException) { ModelState.AddModelError(nameof(department.Code), "Код уже используется."); return View(department); }
    }
    public async Task<IActionResult> Delete(int id) { var d = await db.Departments.Include(x => x.Teachers).FirstOrDefaultAsync(x => x.Id == id); return d is null ? NotFound() : View(d); }
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var d = await db.Departments.Include(x => x.Teachers).FirstOrDefaultAsync(x => x.Id == id); if (d is null) return NotFound();
        if (d.Teachers.Count != 0) { TempData["Error"] = "Нельзя удалить кафедру: сначала переведите или удалите связанных преподавателей."; return RedirectToAction(nameof(Index)); }
        db.Remove(d); await db.SaveChangesAsync(); TempData["Success"] = "Кафедра удалена."; return RedirectToAction(nameof(Index));
    }
}
