using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ModelGenerator.Models;

namespace Main.Models.Products;


public class Product : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
        
    [StringLength(500)]
    public string Description { get; set; }
        
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
        
    // Foreign key: Each Product belongs to one Category.
    [Required]
    public int CategoryId { get; set; }
    [ForeignKey("CategoryId")]
    public virtual Category Category { get; set; }
        
    // Foreign key: Each Product is supplied by one Supplier.
    [Required]
    public int SupplierId { get; set; }
    [ForeignKey("SupplierId")]
    public virtual Supplier Supplier { get; set; }
}