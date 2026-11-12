using System.ComponentModel.DataAnnotations;

namespace UniversityDbApp.Models;

public class Teacher
{
    public int Id { get; set; }
    [Required, StringLength(80)] public string FirstName { get; set; } = string.Empty;
    [Required, StringLength(80)] public string LastName { get; set; } = string.Empty;
    [Required, EmailAddress, StringLength(180)] public string Email { get; set; } = string.Empty;
    [Phone, StringLength(32)] public string? PhoneNumber { get; set; }
    [StringLength(120)] public string? Position { get; set; }
    [StringLength(120)] public string? AcademicDegree { get; set; }
    [DataType(DataType.Date)] public DateTime HireDate { get; set; } = DateTime.Today;
    public int DepartmentId { get; set; }
    public Department? Department { get; set; }
}
