using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Main.Models.Orders;
using ModelGenerator.Models;

namespace Main.Models.Customers;

public class Customer : BaseEntity
{
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; }
        
    [Required]
    [StringLength(50)]
    public string LastName { get; set; }
        
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; }
        
    // Navigation property: One-to-many relationship with Orders.
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    // Navigation property: One-to-many relationship with Addresses.
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";
}