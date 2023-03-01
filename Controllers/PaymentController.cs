using System.Text.Json;
using Cafet_Backend.Abstracts;
using Cafet_Backend.Enums;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;
using Order = Cafet_Backend.Models.Order;
using StripeConfiguration = Cafet_Backend.Configuration.StripeConfiguration;

namespace Cafet_Backend.Controllers;

public class PaymentController : AbstractController
{

    private readonly ILogger<PaymentController> Logger;
    private readonly IOrderRepository OrderRepository;
    private readonly IWalletRepository WalletRepository;
    private readonly Configuration.StripeConfiguration StripeConfiguration;

    public PaymentController(ILogger<PaymentController> logger, IOrderRepository orderRepository,IWalletRepository walletRepository,  IOptions<StripeConfiguration> options)
    {
        Logger = logger;
        OrderRepository = orderRepository;
        StripeConfiguration = options.Value;
        WalletRepository = walletRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Base()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                "whsec_1ea6d4153b17a0ea8859c6b4812942807de64f6e9c06c644f5ffbf877d71395d"
            );

            Session? session = null;
            Console.WriteLine(stripeEvent.Type);
            switch (stripeEvent.Type)
            {
                //case Events.CheckoutSessionAsyncPaymentSucceeded:
                case Events.CheckoutSessionCompleted:
                {
                    session = stripeEvent.Data.Object as Session;
                    if (session == null)
                    {
                        return BadRequest("The checkout session is unknown");
                    }


                    if (session.PaymentStatus != "paid")
                    {
                        return BadRequest("The checkout session is unknown");
                    }

                    PaymentType verifyPaymentType = await VerifyPaymentType(session);
                    if (verifyPaymentType == PaymentType.Unknown)
                    {
                        return BadRequest("Failed to validate payment type");
                    }

                    if (verifyPaymentType == PaymentType.Order)
                    {
                        string? markOrderAsComplete = await MarkOrderAsComplete(session);
                        if (string.IsNullOrEmpty(markOrderAsComplete))
                        {
                            return Ok();
                        }

                        return BadRequest(markOrderAsComplete);
                    }else if (verifyPaymentType == PaymentType.Wallet)
                    {
                        string? updateWalletPayment = await UpdateWalletPayment(session);
                        
                        if (string.IsNullOrEmpty(updateWalletPayment))
                        {
                            return Ok();
                        }

                        return BadRequest(updateWalletPayment);
                    }
                }
                    break;
                case Events.CheckoutSessionAsyncPaymentFailed:
                {
                    session = stripeEvent.Data.Object as Session;
                    if (session == null)
                        return BadRequest("The checkout session is unknown");

                    PaymentType paymentType = await VerifyPaymentType(session);

                    if (paymentType == PaymentType.Order)
                    {
                        if (!session.Metadata.ContainsKey("OrderId"))
                            return BadRequest("An unknown order id was provided");

                        string idRaw = session.Metadata["OrderId"];

                        bool tryParse = Guid.TryParse(idRaw, out Guid orderId);

                        if (!tryParse)
                        {
                            return BadRequest("A malformed order id was provided");
                        }
                    
                        Order? orderOfId = await OrderRepository.GetOrderOfId(orderId);
                        if (orderOfId == null)
                        {
                            return BadRequest("An unknown order was provided");
                        }

                        string? markOrderAsFailed = await OrderRepository.MarkOrderAsFailed(orderId,
                            "Failed payment gateway response has been received.");

                        if (string.IsNullOrEmpty(markOrderAsFailed))
                        {
                            return Ok();
                        }

                        return BadRequest(markOrderAsFailed);
                    }else if (paymentType == PaymentType.Wallet)
                    {
                        return Ok();
                    }
                    else
                    {
                        return BadRequest();
                    }
                    

                }
                    break;
            }
            
            return Ok();
        }
        catch (StripeException e)
        {
            Logger.LogError(e.ToString());
            return BadRequest();
        }
    }

    private async Task<string?> MarkOrderAsComplete(Session session)
    {
        if (!session.Metadata.ContainsKey("OrderId"))
        {
            return ("An unknown order id was provided");
        }

        string idRaw = session.Metadata["OrderId"];

        bool tryParse = Guid.TryParse(idRaw, out Guid orderId);

        if (!tryParse)
        {
            return ("A malformed order id was provided");
        }

        string? markOrderAsComplete = await OrderRepository.MarkOrderAsComplete(orderId);

        if (string.IsNullOrEmpty(markOrderAsComplete))
        {
            return null;
        }

        return (markOrderAsComplete);
    }
    
    private async Task<string?> UpdateWalletPayment(Session session)
    {
        try
        {
            if (!session.Metadata.ContainsKey("UserId"))
            {
                return ("An unknown user id was provided");
            }

            long? sessionAmountTotal = session.AmountTotal;
            if (!sessionAmountTotal.HasValue)
            {
                return ("Failed to get the recharge amount!");
            }

            string idRaw = session.Metadata["UserId"];

            int userId = Convert.ToInt32(idRaw);

            bool credit = await WalletRepository.Credit(userId, userId, sessionAmountTotal.Value/100, null, true);

            if (!credit)
            {
                return "Failed to charge the user";
            }

            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            return e.ToString();
        }
    }

    private async Task<PaymentType> VerifyPaymentType(Session session)
    {
        if (!session.Metadata.ContainsKey("Type"))
        {
            return PaymentType.Unknown;
        }
        

        string typeRaw = session.Metadata["Type"];
        Console.WriteLine(typeRaw);
        bool tryParse = Enum.TryParse(typeRaw, out PaymentType type);

        if (!tryParse)
            return PaymentType.Unknown;

        return type;
    }
}