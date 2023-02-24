using Cafet_Backend.Models;

namespace Cafet_Backend.Interfaces;

public interface IWalletRepository
{

    Task<bool> Withdraw(int userId, int byUser, double amount, string reason = null);

    Task<bool> Credit(int userId, int byUser, double amount, string reason = null, bool sendMail = true);

    Task<double> GetBalanceOf(int userId);

    Task<List<WalletHistory>> GetWalletTransactionsOf(int userId, WalletHistorySpecification? specification = null);
}