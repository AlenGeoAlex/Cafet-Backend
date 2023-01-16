using System.ComponentModel.DataAnnotations;

namespace Cafet_Backend.Dto;

public class EmailQueryDto
{
    [Required]
    public string EmailAddress { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public double Wallet { get; set; }
}