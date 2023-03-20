using Cafet_Backend.Configuration;
using Cafet_Backend.Dto;
using Cafet_Backend.Enums;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Models;
using Microsoft.Extensions.Options;
using Stripe.Checkout;

namespace Cafet_Backend.Manager;

public class StripeSessionManager : IStripeSessionManager
{

    public ILogger<StripeSessionManager> Logger { get; set; }
    
    public StripeConfiguration StripeConfiguration { get; set; }

    public StripeSessionManager(ILogger<StripeSessionManager> logger, IOptions<StripeConfiguration> options)
    {
        Logger = logger;
        StripeConfiguration = options.Value;
    }

    public async Task<Session?> CreateCheckoutSessionForWalletHistory(User user, double amount)
    {
        long orderAmount = Convert.ToInt64(amount * 100);

        Dictionary<string, string> sessionMeta = new Dictionary<string, string>();
        sessionMeta.Add("UserId", user.Id.ToString());
        sessionMeta.Add("Type", PaymentType.Wallet.ToString());

        SessionCreateOptions options = new SessionCreateOptions()
        {
            Mode = "payment",
            Metadata = sessionMeta,
            SuccessUrl = $"{StripeConfiguration.WalletRedirection.Success}",
            LineItems = new List<SessionLineItemOptions>()
            {
                new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        Currency = "INR",
                        UnitAmount = Convert.ToInt64(orderAmount),
                        ProductData = new SessionLineItemPriceDataProductDataOptions()
                        {
                            Name = "Cafet Order"
                        },
                    },
                    Quantity = 1,
                }
            }
        };

        SessionService service = new SessionService();
        return await service.CreateAsync(options);
    }

    public async Task<Session?> CreateCheckoutSessionFor(Order order)
    {
        long orderAmount = Convert.ToInt64(order.OrderAmount * 100);
        string orderId = order.Id.ToString();

        Dictionary<string, string> sessionMeta = new Dictionary<string, string>();
        sessionMeta.Add("OrderId", orderId);
        sessionMeta.Add("Type", PaymentType.Order.ToString());

        SessionCreateOptions options = new SessionCreateOptions()
        {
            Mode = "payment",
            SuccessUrl = $"{StripeConfiguration.Redirections.Success}{orderId}",
            Metadata = sessionMeta,
            LineItems = new List<SessionLineItemOptions>()
            {
                new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        Currency = "INR",
                        UnitAmount = Convert.ToInt64(orderAmount),
                        ProductData = new SessionLineItemPriceDataProductDataOptions()
                        {
                            Name = "Cafet Order"
                        },
                    },
                    Quantity = 1,
                }
            }
        };
        

        SessionService service = new SessionService();
        Session session = await service.CreateAsync(options);

        return session;
    }
}