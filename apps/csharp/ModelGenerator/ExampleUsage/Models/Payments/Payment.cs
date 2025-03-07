using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Main.Models.Orders;
using ModelGenerator.Models;

namespace Main.Models.Payments;

public class Payment : BaseEntity
{
    [Required]
    [ForeignKey("Order")]
    public int OrderId { get; set; }
        
    public virtual Order Order { get; set; }
        
    [Required]
    public PaymentType PaymentMethod { get; set; }
        
    [Required]
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        
    [Required]
    public DateTime PaymentDate { get; set; }
        
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
}