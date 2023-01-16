using System.Security.Cryptography;
using System.Text;
using Cafet_Backend.Context;
using Cafet_Backend.Dto;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Manager;
using Cafet_Backend.Models;
using Cafet_Backend.QueryParams;
using Microsoft.EntityFrameworkCore;

namespace Cafet_Backend;

public class UserRepository : IUserRepository
{

    private readonly CafeContext CafeContext;
    private readonly MailModelManager modelManager;
    private readonly IMailService mailService;
    private readonly ICartRepository CartRepository;
    internal static readonly char[] chars =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray(); 
    public UserRepository(CafeContext cafeContext, MailModelManager modelManager, IMailService mailService, ICartRepository cartRepository)
    {
        CafeContext = cafeContext;
        this.modelManager = modelManager;
        this.mailService = mailService;
        this.CartRepository = cartRepository;
    }

    public async Task<User?> GetUserOfId(int id)
    {
        User? user = await CafeContext.Users
            .Include(u => u.Cart )
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id);
        return user;
    }

    public async Task<int> Register(User user)
    {
        CafeContext.Users.Add(user);
        return await CafeContext.SaveChangesAsync();
    }

    public Task<User?> GetUserOfEmail(string emailAddress)
    {
        return CafeContext.Users
            .Include(u => u.Cart )
            .Include(u => u.Role)
            .FirstOrDefaultAsync(user => user.EmailAddress == emailAddress);
    }

    public async Task<int> Update(User user)
    {
        CafeContext.Users.Update(user);
        return await CafeContext.SaveChangesAsync();
    }

    public async Task<bool> Exists(string email)
    {
        return await CafeContext.Users.AnyAsync(user => user.EmailAddress == email);
    }

    public async Task<List<User>> GetAllUser()
    {
        return await CafeContext.Users
            .Include(user => user.Role)
            .ToListAsync();
    }

    public async Task<List<User>> GetActiveUsers()
    {
        return await CafeContext.Users
            .Where(u => !u.Deleted)
            .Include(user => user.Role)
            .ToListAsync();    }

    public async Task<bool> DeleteUser(int id)
    {
        User? userOfId = await GetUserOfId(id);

        if (userOfId == null)
            return false;

        await CartRepository.ClearCart(userOfId.CartId);
        userOfId.Deleted = true;
        return true;
    }

    public async Task<User?> ResetPassword(int id)
    {
        User? userOfId = await GetUserOfId(id);
        if (userOfId == null)
            return null;
        
        HMACSHA512 hasher = new HMACSHA512();
        string password = GetUniqueKey(10);
        byte[] salt = hasher.Key;
        byte[] passwordHash = hasher.ComputeHash(Encoding.UTF8.GetBytes(password));

        userOfId.PasswordHash = passwordHash;
        userOfId.UserSalt = salt;

        CafeContext.Update(userOfId);
        await CafeContext.SaveChangesAsync();

        await mailService.SendMailAsync(modelManager.PasswordResetMailModel, userOfId.EmailAddress ,new[] { password });
        
        return userOfId;
    }

    public async Task<User?> TryRegister(RegistrationParam param)
    {
        Role? roleByName = Role.GetByName(param.Role);
        if (roleByName == null)
        {
            return null;
        }

        Role? roleData = await CafeContext.Roles.FirstOrDefaultAsync(role => role.RoleName == param.Role);
        if (roleData == null)
        {
            return null;
        }
        
        HMACSHA512 hasher = new HMACSHA512();

        bool shouldGenPassword = string.IsNullOrEmpty(param.Password);

        string password = param.Password ?? GetUniqueKey(10);
        
        byte[] salt = hasher.Key;
        byte[] passwordHash = hasher.ComputeHash(Encoding.UTF8.GetBytes(password));

        Cart cart = new Cart();
        
        User user  = new User()
        {
            EmailAddress = param.EmailAddress,
            FirstName = param.FirstName,
            LastName = param.LastName,
            Cart = cart,
            CartId = cart.CartId,
            RoleId = roleData.Id,
            Activated = true,
            Deleted = false,
            PasswordHash = passwordHash,
            UserSalt = salt,
            ProfileImage = "default.png",
            WalletBalance = 0.0,
        };

        if (shouldGenPassword)
        {
            await mailService.SendMailAsync(modelManager.PasswordResetMailModel, user.EmailAddress, new[] { password });
        }
        
        return user;
    }

    public async Task<List<EmailQueryDto>> GetEmailAddress(string emailSearch)
    {
        return await CafeContext.Users
            .Where(user => user.EmailAddress.Contains(emailSearch))
            .Select(user => new EmailQueryDto()
            {
                FirstName = user.FirstName,
                EmailAddress = user.EmailAddress,
                LastName = user.LastName,
                Wallet = user.WalletBalance
            })
            .ToListAsync();
    }
    
    public async Task<EmailQueryDto?> GetUserOfEmailAddress(string emailSearch)
    {
        return await CafeContext.Users
            .Where(user => user.EmailAddress == emailSearch)
            .Select(user => new EmailQueryDto()
            {
                FirstName = user.FirstName,
                EmailAddress = user.EmailAddress,
                LastName = user.LastName,
                Wallet = user.WalletBalance
            })
            .FirstOrDefaultAsync();
    }
    
    public async Task SaveAsync()
    {
        await CafeContext.SaveChangesAsync();
        return;
    }
    
    
    
    public static string GetUniqueKey(int size)
    {            
        byte[] data = new byte[4*size];
        using (var crypto = RandomNumberGenerator.Create())
        {
            crypto.GetBytes(data);
        }
        StringBuilder result = new StringBuilder(size);
        for (int i = 0; i < size; i++)
        {
            var rnd = BitConverter.ToUInt32(data, i * 4);
            var idx = rnd % chars.Length;

            result.Append(chars[idx]);
        }

        return result.ToString();
    }
    
}