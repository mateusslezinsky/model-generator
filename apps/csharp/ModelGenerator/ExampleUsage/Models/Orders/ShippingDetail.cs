using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ModelGenerator.Models;

namespace Main.Models.Orders;

public class ShippingDetail : BaseEntity
{
    [Required]
    [ForeignKey("Order")]
    public int OrderId { get; set; }
        
    public virtual Order Order { get; set; }
        
    [Required]
    [StringLength(200)]
    public string ShippingAddress { get; set; }
        
    [Required]
    public DateTime EstimatedDelivery { get; set; }
        
    [Required]
    public ShippingStatus Status { get; set; } = ShippingStatus.NotShipped;
}