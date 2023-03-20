using System.ComponentModel.DataAnnotations;
using Cafet_Backend.Models;

namespace Cafet_Backend.Specification;


public class UserActivitySpecificationParam
{
    public string? From { get; set; }
    
    public string? To { get; set; }
    
    [Required]
    public string User { get; set; }
    
    public int? Type { get; set; }
}