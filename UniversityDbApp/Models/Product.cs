using System.ComponentModel.DataAnnotations;

namespace UniversityDbApp.Models;

public class Product
{
    public int Id { get; set; }
    [Required, StringLength(120)] public string Name { get; set; } = string.Empty;
    [StringLength(400)] public string? Description { get; set; }
    [Range(typeof(decimal), "0.01", "999999999.99")] public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}
