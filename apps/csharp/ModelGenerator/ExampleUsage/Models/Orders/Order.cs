using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Main.Models.Customers;
using Main.Models.Payments;
using ModelGenerator.Models;

namespace Main.Models.Orders;

public class Order : BaseEntity
{
    [Required]
    public int CustomerId { get; set; }
        
    [ForeignKey("CustomerId")]
    public virtual Customer Customer { get; set; }
        
    [Required]
    public DateTime OrderDate { get; set; }
        
    [Required]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
        
    // One-to-many relationship: An Order has multiple OrderItems.
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        
    // One-to-one relationships.
    public virtual Payment Payment { get; set; }
    public virtual ShippingDetail ShippingDetail { get; set; }

    // Computed property that calculates total order amount.
    [NotMapped]
    public decimal TotalAmount => OrderItems != null ? ComputeTotal() : 0m;
        
    private decimal ComputeTotal()
    {
        decimal total = 0;
        foreach (var item in OrderItems)
        {
            total += item.Quantity * item.UnitPrice;
        }
        return total;
    }
}