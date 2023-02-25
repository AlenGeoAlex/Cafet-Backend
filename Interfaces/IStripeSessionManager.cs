using Cafet_Backend.Dto;
using Cafet_Backend.Models;
using Stripe.Checkout;

namespace Cafet_Backend.Interfaces;

public interface IStripeSessionManager
{
    Task<Session?> CreateCheckoutSessionFor(Order order);
}