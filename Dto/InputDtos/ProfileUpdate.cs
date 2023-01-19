namespace Cafet_Backend.Dto.InputDtos;

public class ProfileUpdate
{
    public string EmailAddress { get; set; }
    public string FirstName { get; set; }
    public string LastName{ get; set; }
    public string PhoneNumber{ get; set; }
    public string? Password{ get; set; }
    public IFormFile? ImageFile{ get; set; }

    public bool ShouldUpdatePassword
    {
        get => Password != null && Password.Length >= 8;
    }

    public bool ShouldUpdateImage
    {
        get => ImageFile != null;
    }
    
}