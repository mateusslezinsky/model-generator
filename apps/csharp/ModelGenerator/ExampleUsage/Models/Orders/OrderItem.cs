using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Main.Models.Products;
using ModelGenerator.Models;

namespace Main.Models.Orders;

public class OrderItem : BaseEntity
{
    [Required]
    public int OrderId { get; set; }
        
    [ForeignKey("OrderId")]
    public virtual Order Order { get; set; }
        
    [Required]
    public int ProductId { get; set; }
        
    [ForeignKey("ProductId")]
    public virtual Product Product { get; set; }
        
    [Required]
    public int Quantity { get; set; }
        
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }
}
