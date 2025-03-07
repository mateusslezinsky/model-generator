using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ModelGenerator.Models;

namespace Main.Models.Products;

public class Supplier : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string CompanyName { get; set; }
        
    [Required]
    [StringLength(50)]
    public string ContactName { get; set; }
        
    [Required]
    [Phone]
    [StringLength(20)]
    public string Phone { get; set; }
        
    // Navigation property: One Supplier supplies many Products.
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
        
    // Demonstrates use of a generic collection type that is not directly mapped.
    [NotMapped]
    public Dictionary<string, string> AdditionalInfo { get; set; } = new Dictionary<string, string>();
}