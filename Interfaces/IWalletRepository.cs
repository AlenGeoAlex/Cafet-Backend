using Cafet_Backend.Models;

namespace Cafet_Backend.Interfaces;

public interface IWalletRepository
{

    Task<bool> Withdraw(int userId, int byUser, double amount);

    Task<bool> Credit(int userId, int byUser, double amount);

    Task<double> GetBalanceOf(int userId);

    Task<List<WalletHistory>> GetWalletTransactionsOf(int userId, WalletHistorySpecification? specification = null);
}