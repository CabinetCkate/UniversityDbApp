using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UniversityDbApp.Data;
using UniversityDbApp.Models;

namespace UniversityDbApp.Controllers;
public class TeachersController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index(string? search, int? departmentId)
    {
        var q = db.Teachers.Include(t => t.Department).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) q = q.Where(t => (t.FirstName + " " + t.LastName).Contains(search) || t.Email.Contains(search));
        if (departmentId.HasValue) q = q.Where(t => t.DepartmentId == departmentId.Value);
        await LoadDepartments(departmentId); ViewBag.Search = search;
        return View(await q.OrderBy(t => t.LastName).ThenBy(t => t.FirstName).ToListAsync());
    }
    public async Task<IActionResult> Details(int id) { var t = await db.Teachers.Include(x => x.Department).FirstOrDefaultAsync(x => x.Id == id); return t is null ? NotFound() : View(t); }
    public async Task<IActionResult> Create() { await LoadDepartments(); return View(new Teacher()); }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Teacher teacher)
    {
        if (!await db.Departments.AnyAsync(d => d.Id == teacher.DepartmentId)) ModelState.AddModelError(nameof(teacher.DepartmentId), "Выберите существующую кафедру.");
        if (!ModelState.IsValid) { await LoadDepartments(teacher.DepartmentId); return View(teacher); }
        db.Add(teacher);
        try { await db.SaveChangesAsync(); TempData["Success"] = "Преподаватель добавлен."; return RedirectToAction(nameof(Index)); }
        catch (DbUpdateException) { ModelState.AddModelError(nameof(teacher.Email), "Этот адрес уже зарегистрирован."); await LoadDepartments(teacher.DepartmentId); return View(teacher); }
    }
    public async Task<IActionResult> Edit(int id) { var t = await db.Teachers.FindAsync(id); if (t is null) return NotFound(); await LoadDepartments(t.DepartmentId); return View(t); }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Teacher teacher)
    {
        if (id != teacher.Id) return BadRequest();
        if (!await db.Departments.AnyAsync(d => d.Id == teacher.DepartmentId)) ModelState.AddModelError(nameof(teacher.DepartmentId), "Выберите существующую кафедру.");
        if (!ModelState.IsValid) { await LoadDepartments(teacher.DepartmentId); return View(teacher); }
        db.Update(teacher);
        try { await db.SaveChangesAsync(); TempData["Success"] = "Изменения сохранены."; return RedirectToAction(nameof(Index)); }
        catch (DbUpdateException) { ModelState.AddModelError(nameof(teacher.Email), "Этот адрес уже зарегистрирован."); await LoadDepartments(teacher.DepartmentId); return View(teacher); }
    }
    public async Task<IActionResult> Delete(int id) { var t = await db.Teachers.Include(x => x.Department).FirstOrDefaultAsync(x => x.Id == id); return t is null ? NotFound() : View(t); }
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id) { var t = await db.Teachers.FindAsync(id); if (t is null) return NotFound(); db.Remove(t); await db.SaveChangesAsync(); TempData["Success"] = "Преподаватель удалён."; return RedirectToAction(nameof(Index)); }
    private async Task LoadDepartments(int? selected = null) => ViewBag.DepartmentId = new SelectList(await db.Departments.OrderBy(d => d.Name).ToListAsync(), "Id", "Name", selected);
}
