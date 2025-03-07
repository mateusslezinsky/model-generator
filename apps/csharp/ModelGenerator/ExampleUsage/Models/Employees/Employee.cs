using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Main.Models.Orders;
using ModelGenerator.Models;

namespace Main.Models.Employees;

public partial class Employee : BaseEntity
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
        
    // Self-referencing relationship: an Employee may have a Manager.
    public int? ManagerId { get; set; }
    [ForeignKey("ManagerId")]
    public virtual Employee Manager { get; set; }
        
    // One-to-many: an Employee can have many Subordinates.
    public virtual ICollection<Employee> Subordinates { get; set; } = new List<Employee>();

    // Navigation property: an Employee can be responsible for multiple Orders.
    public virtual ICollection<Order> OrdersHandled { get; set; } = new List<Order>();

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";
}

public partial class Employee
{
    // Example of an indexer property.
    private Dictionary<string, string> _attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    public string this[string key]
    {
        get => _attributes.ContainsKey(key) ? _attributes[key] : null;
        set => _attributes[key] = value;
    }
}