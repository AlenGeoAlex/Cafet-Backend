using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cafet_Backend.Models;

public class User : KeyedEntity
{
    public static readonly User DefaultAdmin = new User()
    {
        Id = -1,
        FirstName = "Cafet",
        LastName = "Admin",
        CartId = Models.Cart.DummyCart.CartId,
        EmailAddress = "adminstrator@cafet.com",
        RoleId = Models.Role.Administrator.Id,
        Password = "cafet-admin",
        WalletBalance = 0.0,
        ProfileImage = "default.png",
        Activated = true,
        Deleted = false,
    };
    [MaxLength(30)] public string FirstName { get; set; }
    [MaxLength(30)] public string LastName { get; set; }

    [NotMapped]
    public string FullName
    {
        get => FirstName + " " + LastName;
    }
    
    [MaxLength(40)] [Required] public string EmailAddress { get; set; }
    
    [Required] public string Password { get; set; }
    
    [ForeignKey("RoleId")]
    public Role Role { get; set; }
    
    [Required] public int RoleId { get; set; }

    [Column(TypeName = "Smallmoney")]
    [Required]
    public double WalletBalance { get; set; }
    
    public string ProfileImage { get; set; }
    
    [Required]
    public bool Activated { get; set; } = false;
    
    [Required]
    public bool Deleted { get; set; } = false;
    
    [ForeignKey("CartId")]
    public Cart Cart { get; set; }
    
    [Required]
    public Guid CartId { get; set; }

}