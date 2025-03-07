using System.ComponentModel.DataAnnotations;
using ModelGenerator.Models;

namespace Main.Models.Products;

public class Category : BaseEntity
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; }
        
    [StringLength(250)]
    public string Description { get; set; }
        
    // Navigation property: One Category has many Products.
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}