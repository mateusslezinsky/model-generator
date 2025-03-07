using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ModelGenerator.Models;

namespace Main.Models.Customers;

public class Address : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string Street { get; set; }
        
    [Required]
    [StringLength(50)]
    public string City { get; set; }
        
    [Required]
    [StringLength(50)]
    public string State { get; set; }
        
    [Required]
    [StringLength(20)]
    public string ZipCode { get; set; }
        
    [Required]
    [StringLength(50)]
    public string Country { get; set; }

    // Foreign key and navigation property: Many addresses belong to one Customer.
    [Required]
    public int CustomerId { get; set; }
    [ForeignKey("CustomerId")]
    public virtual Customer Customer { get; set; }
}