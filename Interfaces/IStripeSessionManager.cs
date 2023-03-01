using Cafet_Backend.Dto;
using Cafet_Backend.Models;
using Stripe.Checkout;

namespace Cafet_Backend.Interfaces;

public interface IStripeSessionManager
{
    Task<Session?> CreateCheckoutSessionForWalletHistory(User user, double amount);
    Task<Session?> CreateCheckoutSessionFor(Order order);
}