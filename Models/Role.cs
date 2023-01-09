using System.ComponentModel.DataAnnotations;

namespace Cafet_Backend.Models;

public class Role : KeyedEntity
{

    public static readonly Role Administrator = new Role()
    {
        Id = 1,
        RoleName = "Admin",
        RoleDescription = "This role has access to all the administrative privilege on the system"
    };

    public static readonly Role CafetStaff = new Role()
    {
        Id = 2,
        RoleName = "Staff",
        RoleDescription = "This role defines the cafeteria staff"
    };
    
    public static readonly Role Customer = new Role()
    {
        Id = 3,
        RoleName = "Customer",
        RoleDescription = "This role defines the customer."
    };
    
    public static Role? GetByName(string roleName)
    {
        
        switch (roleName.ToUpper())
        {
            case "ADMIN":
                return Role.Administrator;
            case "STAFF":
                return Role.CafetStaff;
            case "CUSTOMER":
                return Role.Customer;
            default: return null;
        }
    }
    
    [MaxLength(30)] public string RoleName { get; set; }
    public string RoleDescription { get; set; }

    public override bool Equals(object? obj)
    {
        if ((obj == null) || this.GetType() != obj.GetType())
        {
            return false;
        }
        else {
            Role p = (Role) obj;
            return (Id == p.Id) && (RoleName == p.RoleName);
        }
    }

    protected bool Equals(Role other)
    {
        return RoleName == other.RoleName && RoleDescription == other.RoleDescription;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(RoleName, RoleDescription);
    }
}