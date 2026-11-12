using System.ComponentModel.DataAnnotations;

namespace UniversityDbApp.Models;

public class Category
{
    public int Id { get; set; }
    [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
    [StringLength(400)] public string? Description { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
