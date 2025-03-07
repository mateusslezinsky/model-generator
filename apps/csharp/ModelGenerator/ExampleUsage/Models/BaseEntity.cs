using System.ComponentModel.DataAnnotations;

namespace ModelGenerator.Models;

public class BaseEntity
{
    [Key]
    public int Id { get; set; }
        
    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.Now;
}