using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Services.Contract;

namespace Talabat.APIs.Controllers
{ 
	public class PaymentController : BaseAPIController
	{
		private readonly IPaymentService _paymentService;
		private readonly ILogger<PaymentController> _logger;

		// This is your Stripe CLI webhook secret for testing your endpoint locally.
		private const string whSecret = "whsec_e4b88a05634065aefd4f0909b07efddfd79f6c98d2a80da5c4b9b69cd2331eea";

		public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
			_paymentService = paymentService;
		     _logger = logger;
		}

		[ProducesResponseType(typeof(CustomerBasket), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
		[HttpGet]
		public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
		{
			var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);
			if (basket is null) return BadRequest(new ApiResponse(400, "An Error With Your Backet Has Occured"));

			return Ok(basket);
		}


		[HttpPost("webhook")]
		public async Task<IActionResult> WebHook()
		{
			var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
				var stripeEvent = EventUtility.ConstructEvent(json,
					Request.Headers["Stripe-Signature"], whSecret);

		    var paymentIntent = (PaymentIntent) stripeEvent.Data.Object;

			Order? order;			         

                switch(stripeEvent.Type)
				{
					case Events.PaymentIntentSucceeded:
					order = await _paymentService.UpdateOrderStatus(paymentIntent.Id, true);
					_logger.LogInformation("Order Is Succeeded: {0}", order?.PaymentIntentId);
					_logger.LogInformation("Unhandeld Event Type: {0}", stripeEvent.Type);
						break;
					case Events.PaymentIntentPaymentFailed:
						order = await _paymentService.UpdateOrderStatus(paymentIntent.Id, false);
					_logger.LogInformation("Order Is Failed: {0}", order?.PaymentIntentId);
					_logger.LogInformation("Unhandeld Event Type: {0}", stripeEvent.Type);
					break;
				}

				return Ok();
		}
    }
}
