using System.Text.Json;
using Cafet_Backend.Abstracts;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;
using Order = Cafet_Backend.Models.Order;
using StripeConfiguration = Cafet_Backend.Configuration.StripeConfiguration;

namespace Cafet_Backend.Controllers;

public class PaymentController : AbstractController
{

    private readonly ILogger<PaymentController> Logger;
    private readonly IOrderRepository OrderRepository;
    private readonly Configuration.StripeConfiguration StripeConfiguration;

    public PaymentController(ILogger<PaymentController> logger, IOrderRepository orderRepository, IOptions<StripeConfiguration> options)
    {
        Logger = logger;
        OrderRepository = orderRepository;
        StripeConfiguration = options.Value;
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
                    
                    if (!session.Metadata.ContainsKey("OrderId"))
                    {
                        return BadRequest("An unknown order id was provided");
                    }

                    string idRaw = session.Metadata["OrderId"];

                    bool tryParse = Guid.TryParse(idRaw, out Guid orderId);

                    if (!tryParse)
                    {
                        return BadRequest("A malformed order id was provided");
                    }

                    string? markOrderAsComplete = await OrderRepository.MarkOrderAsComplete(orderId);

                    if (string.IsNullOrEmpty(markOrderAsComplete))
                    {
                        return Ok();
                    }

                    return BadRequest(markOrderAsComplete);
                }
                    break;
                case Events.CheckoutSessionAsyncPaymentFailed:
                {
                    session = stripeEvent.Data.Object as Session;
                    if (session == null)
                        return BadRequest("The checkout session is unknown");

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
}