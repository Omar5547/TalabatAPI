using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{
    

    public class PaymentsController : APIBaseController
    {
        private readonly IPaymentServices _paymentServices;
        private readonly IMapper _mapper;
        const string endpointSecret = "whsec_92e26957a6eaea79a7dc233b09283cd8d27483c931664a421b06174071dda62f";

        public PaymentsController(IPaymentServices paymentServices, IMapper mapper)
        {
            _paymentServices = paymentServices;
            _mapper = mapper;
        }
        [ProducesResponseType(typeof(CustomerBasketDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]

        [HttpPost("{basketId}")]
        [Authorize]
        public async Task<ActionResult<CustomerBasketDto>> CreateOrUpatePaymentIntent(string basketId) 
        {
           var CustomerBasket = await _paymentServices.CreatOrUpdatePaymentIntent(basketId);
            if (CustomerBasket == null)  return BadRequest(new ApiResponse(400, "There is a Problem With your Basket"));
            var MappedBasket = _mapper.Map<CustomerBasket,CustomerBasketDto>(CustomerBasket);
           return Ok(MappedBasket);
        }
        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
          
            try
            {
                var signatureHeader = Request.Headers["Stripe-Signature"];
                var stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, endpointSecret);
                var PaymentIntent = stripeEvent.Data.Object as PaymentIntent;

                // ✅ الدفع تم بنجاح
                if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)

                {
                    await _paymentServices.UpdatePaymentIntentToSucceedOrFailed(PaymentIntent.Id, true);
                    // هنا تقدر تحدث الطلب في الداتا بيز مثلاً
                    // await _paymentService.MarkOrderAsPaid(paymentIntent.Id);
                }
                // ❌ الدفع فشل
                else if (stripeEvent.Type == EventTypes.PaymentIntentPaymentFailed)

                {
                   await _paymentServices.UpdatePaymentIntentToSucceedOrFailed(PaymentIntent.Id, false);

                    // تقدر تسجل فشل الدفع وتبعت إشعار للمستخدم مثلًا
                }
               

                return Ok();
            }
            catch (StripeException e)
            {
                Console.WriteLine($"Stripe error: {e.Message}");
                return BadRequest();
            }
            catch (Exception e)
            {
                Console.WriteLine($"General error: {e.Message}");
                return StatusCode(500);
            }

        }

    }
}
