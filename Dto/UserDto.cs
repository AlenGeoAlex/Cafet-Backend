namespace Cafet_Backend.Dto;

public class UserDto
{
    public string UserEmailAddress { get; set; }
    public string UserFullName { get; set; }
    public string UserRole { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string ImageLink { get; set; }
    public string CartId { get; set; }
}