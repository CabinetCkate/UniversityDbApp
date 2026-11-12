using UniversityDbApp.Models;
namespace UniversityDbApp.ViewModels;
public class DashboardViewModel
{
    public int DepartmentCount { get; set; }
    public int TeacherCount { get; set; }
    public int ProductCount { get; set; }
    public int CategoryCount { get; set; }
    public List<(string Name, int Count)> Distribution { get; set; } = [];
    public List<Teacher> RecentTeachers { get; set; } = [];
}
