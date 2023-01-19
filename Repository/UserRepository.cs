using System.Security.Cryptography;
using System.Text;
using Cafet_Backend.Context;
using Cafet_Backend.Dto;
using Cafet_Backend.Dto.InputDtos;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Manager;
using Cafet_Backend.Models;
using Cafet_Backend.QueryParams;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Cafet_Backend;

public class UserRepository : IUserRepository
{

    private readonly CafeContext CafeContext;
    private readonly MailModelManager modelManager;
    private readonly IMailService mailService;
    private readonly IConfiguration Configuration;
    private readonly ImageProviderManager ImageProviderManager;
    private readonly ILogger<UserRepository> Logger;
    internal static readonly char[] chars =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray(); 
    public UserRepository(CafeContext cafeContext, MailModelManager modelManager, IMailService mailService, ImageProviderManager imageProviderManager, IConfiguration configuration ,ILogger<UserRepository> logger)
    {
        CafeContext = cafeContext;
        this.modelManager = modelManager;
        this.mailService = mailService;
        this.Logger = logger;
        this.ImageProviderManager = imageProviderManager;
        this.Configuration = configuration;
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

        //await CartRepository.ClearCart(userOfId.CartId);
        //OrderRepository.DeleteOrderDataOfAsync(userOfId.Id);
        userOfId.Deleted = true;
        return true;
    }

    public async Task<User?> ResetPassword(int id)
    {
        User? userOfId = await GetUserOfId(id);
        if (userOfId == null)
            return null;

        return await ResetPassword(userOfId);
    }

    public async Task<User?> ResetPassword(User userOfId)
    {
        HMACSHA512 hasher = new HMACSHA512();
        string password = GetUniqueKey(10);
        byte[] salt = hasher.Key;
        byte[] passwordHash = hasher.ComputeHash(Encoding.UTF8.GetBytes(password));
        
        string[] passwordPlaceholder = new string[1];
        passwordPlaceholder[0] = password;
        bool emailSend = await mailService.SendMailAsync(modelManager.PasswordResetMailModel, userOfId.EmailAddress , passwordPlaceholder);
        if (!emailSend)
            return null;
        
        userOfId.PasswordHash = passwordHash;
        userOfId.UserSalt = salt;
        CafeContext.Update(userOfId);
        await CafeContext.SaveChangesAsync();
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
            string[] passwordPlaceholder = new string[1];
            passwordPlaceholder[0] = password;
          bool sendMail =  await mailService.SendMailAsync(modelManager.PasswordResetMailModel, user.EmailAddress, passwordPlaceholder);
          if (sendMail == false)
              return null;
        }

        CafeContext.Users.Add(user);
        await CafeContext.SaveChangesAsync();
        
        return user;
    }

    public async Task<User?> UpdateUser(ProfileUpdate profileUpdate)
    {

        User? user = await CafeContext.Users.FirstOrDefaultAsync(e => e.EmailAddress == profileUpdate.EmailAddress);

        if (user == null)
            return null;

        byte[] passwordHash = user.PasswordHash;
        byte[] passwordSalt = user.UserSalt;
        string imageFileName = user.ProfileImage;

        if (profileUpdate.ShouldUpdatePassword && profileUpdate.Password != null)
        {
            HMACSHA512 hasher = new HMACSHA512();
            passwordSalt = hasher.Key;
            passwordHash = hasher.ComputeHash(Encoding.UTF8.GetBytes(profileUpdate.Password));

            string clientAddress = Configuration["client"];
            string resetUrl = $"{clientAddress}auth/reset/{profileUpdate.EmailAddress}";
            string[] param = new[] { resetUrl };
            bool mailSend = await mailService.SendMailAsync(modelManager.PasswordChangeAlert, profileUpdate.EmailAddress, param);
            if (!mailSend)
            {
                Logger.LogError("Failed to send password change alert to user "+user.EmailAddress);
                return null;
            }
            else
            {
                Logger.Log(LogLevel.Information, "Successfully send password change alert to user");
            }
        }


        if (profileUpdate.ShouldUpdateImage)
        {

            if (imageFileName != "default.png")
            {
                ImageProviderManager.UserImageProvider.Delete(imageFileName);
                Logger.LogInformation("Successfully deleted "+imageFileName);

            }
            
            string fileName = $"{Guid.NewGuid().ToString().Replace("-", "")}-{profileUpdate.ImageFile.FileName.Replace("-", "")}";
            fileName = fileName.Trim();
            string fullFile = ImageProviderManager.UserImageProvider.AsFileName(fileName);
            using (FileStream fStream = new FileStream(fullFile, FileMode.Create))
            {
                await profileUpdate.ImageFile.CopyToAsync(fStream);
                imageFileName = fileName;
                Logger.LogInformation("Successfully uploaded "+imageFileName);
            }
        }

        user.FirstName = profileUpdate.FirstName;
        user.LastName = profileUpdate.LastName;
        user.ProfileImage = imageFileName;
        user.PhoneNumber = profileUpdate.PhoneNumber;
        user.PasswordHash = passwordHash;
        user.UserSalt = passwordSalt;

        await CafeContext.SaveChangesAsync();
        return user;
    }

    public async Task<List<EmailQueryDto>> GetEmailAddress(string emailSearch)
    {
        return await CafeContext.Users
            .Where(user => user.EmailAddress.Contains(emailSearch) && !user.Deleted)
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