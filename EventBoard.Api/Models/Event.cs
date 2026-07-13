using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventBoard.Api.Models;

public class Event
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Date is required")]
    public DateTime Date { get; set; }

    [Required(ErrorMessage = "UserId is required")]
    [ForeignKey("User")]
    public Guid UserId { get; set; }

    // Navigation property
    [InverseProperty("Events")]
    public User? User { get; set; }
}
