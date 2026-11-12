using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityDbApp.Data;
using UniversityDbApp.ViewModels;

namespace UniversityDbApp.Controllers;
public class DashboardController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var model = new DashboardViewModel
        {
            DepartmentCount = await db.Departments.CountAsync(), TeacherCount = await db.Teachers.CountAsync(),
            ProductCount = await db.Products.CountAsync(), CategoryCount = await db.Categories.CountAsync(),
            Distribution = await db.Departments.OrderBy(d => d.Name).Select(d => new ValueTuple<string, int>(d.Name, d.Teachers.Count)).ToListAsync(),
            RecentTeachers = await db.Teachers.Include(t => t.Department).OrderByDescending(t => t.HireDate).Take(5).ToListAsync()
        };
        return View(model);
    }
}
