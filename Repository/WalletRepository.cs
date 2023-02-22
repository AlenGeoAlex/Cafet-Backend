using System.Globalization;
using AutoMapper;
using Cafet_Backend.Context;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Manager;
using Cafet_Backend.Models;
using Cafet_Backend.Provider;
using Microsoft.EntityFrameworkCore;

namespace Cafet_Backend.Repository;

public class WalletRepository : IWalletRepository
{
    private readonly CafeContext Context;
    
    private readonly IMapper Mapper;

    private readonly ILogger<WalletRepository> Logger;

    private readonly MailModelManager MailModelManager;
    private readonly IMailService MailService;

    public WalletRepository(CafeContext context, IMapper mapper, MailModelManager mailModelManager, IMailService mailService, ILogger<WalletRepository> walletRepo)
    {
        Context = context;
        Mapper = mapper;
        MailModelManager = mailModelManager;
        MailService = mailService;
        Logger = walletRepo;
    }

    public async Task<bool> Withdraw(int userId, int byUser, double amount, string reason = null)
    {
        if (amount < 1)
            return false;
        
        User? rec = await Context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (rec == null)
            return false;

        User? sender = await Context.Users.FirstOrDefaultAsync(u => u.Id == byUser);
        
        if (sender == null)
            return false;
        
        if (rec.WalletBalance < amount)
            return false;

        rec.WalletBalance -= amount;
        WalletHistory walletHistory = new WalletHistory()
        {
            RechargeStatus = true,
            RecipientId = rec.Id,
            SenderId = sender.Id,
            Credit = false,
            FailReason = null,
            Amount = amount,
            RechargeTime = DateTime.Now,
        };

        await Context.WalletHistories.AddAsync(walletHistory);
        await Context.SaveChangesAsync();
        
        
        
        return true;
    }

    public async Task<bool> Credit(int userId, int byUser, double amount, string reason = null)
    {
        if (amount < 1)
            return false;
        
        User? rec = await Context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (rec == null)
            return false;

        User? sender = null;

        if (userId == byUser)
            sender = rec;
        else
        {
            sender = await Context.Users.FirstOrDefaultAsync(u => u.Id == byUser);
        }

        if (sender == null)
            return false;
        

        rec.WalletBalance += amount;
        WalletHistory walletHistory = new WalletHistory()
        {
            RechargeStatus = true,
            RecipientId = rec.Id,
            SenderId = sender.Id,
            Credit = true,
            FailReason = reason,
            Amount = amount,
            RechargeTime = DateTime.Now,
        };

        
        
        await Context.WalletHistories.AddAsync(walletHistory);
        await Context.SaveChangesAsync();

        bool mailAsync = await MailService.SendMailAsync(MailModelManager.WalletRecharge, rec.EmailAddress,
            new string[] { amount.ToString(CultureInfo.InvariantCulture) });

        if (!mailAsync)
        {
            Logger.LogWarning($"Failed to send wallet recharge email to user {rec.EmailAddress} of recharge {walletHistory.Id}");
        }
        
        return true;
    }

    public async Task<double> GetBalanceOf(int userId)
    {
        User? rec = await Context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (rec == null)
            return 0D;

        return rec.WalletBalance;
    }

    public async Task<List<WalletHistory>> GetWalletTransactionsOf(int userId,
        WalletHistorySpecification? specification = null)
    {
        return await SpecificationProvider<WalletHistory, int>.GetQuery(Context.Set<WalletHistory>(), specification)
            .Include(wh => wh.Sender)
            .Include(wh => wh.Recipient)
            .Where(wh => wh.RecipientId == userId)
            .ToListAsync();
    }
}