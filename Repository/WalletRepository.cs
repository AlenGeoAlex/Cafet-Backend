using AutoMapper;
using Cafet_Backend.Context;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Cafet_Backend.Repository;

public class WalletRepository : IWalletRepository
{
    private readonly CafeContext Context;
    
    private readonly IMapper Mapper;

    public WalletRepository(CafeContext context, IMapper mapper)
    {
        Context = context;
        Mapper = mapper;
    }

    public async Task<bool> Withdraw(int userId, int byUser, double amount)
    {
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

    public async Task<bool> Credit(int userId, int byUser, double amount)
    {
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
            FailReason = null,
            Amount = amount,
            RechargeTime = DateTime.Now,
        };

        await Context.WalletHistories.AddAsync(walletHistory);
        await Context.SaveChangesAsync();
        return true;
    }

    public async Task<double> GetBalanceOf(int userId)
    {
        User? rec = await Context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (rec == null)
            return 0D;

        return rec.WalletBalance;
    }

    public async Task<List<WalletHistory>> GetWalletTransactionsOf(int userId)
    {
        return await Context.WalletHistories
            .Include(wh => wh.Sender)
            .Include(wh => wh.Recipient)
            .Where(wh => wh.RecipientId == userId)
            .ToListAsync();
    }
}