using System.ComponentModel.DataAnnotations;

namespace UniversityDbApp.Models;

public class Department
{
    public int Id { get; set; }
    [Required, StringLength(120)] public string Name { get; set; } = string.Empty;
    [Required, StringLength(12)] public string Code { get; set; } = string.Empty;
    [StringLength(500)] public string? Description { get; set; }
    [StringLength(120)] public string? Faculty { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
}
