namespace Cafet_Backend.Dto;

public class UserDto
{
    public string UserName { get; set; }
    public string UserFirstName { get; set; }
    public string UserLastName { get; set; }
    public string UserEmail { get; set; }
    public string UserImage { get; set; }
    public double WalletBalance { get; set; }
    public string UserRole { get; set; }
    
    public string? PhoneNumber { get; set; }
    public bool Activated { get; set; }
    
    public bool Deleted { get; set; }
    
    public int UserId { get; set; }
}