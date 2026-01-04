using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreCore.Dtos.Payments;
using StoreCore.ServicesContract;

namespace StoreApis.Controllers
{
    [Authorize]
    public class PaymentController : BaseController
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // إنشاء دفعة عامة (الكارت في Paymob)
        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentDto dto)
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(buyerEmail))
                return Unauthorized();

            var result = await _paymentService.CreateOrUpdatePaymobPaymentAsync(
                dto.BasketId,
                buyerEmail,
                dto.BillingAddress
            );

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // Webhook من Paymob (علّني)
        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] PaymobWebhookDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.PaymentId))
                return BadRequest("Invalid payload");

            var success = await _paymentService.HandlePaymobWebhookAsync(dto.PaymentId, dto.Status);
            return success ? Ok("✅ Webhook processed") : BadRequest("❌ Failed to process webhook");
        }

        // إنشاء دفعة محفظة (wallet)
        [HttpPost("wallet")]
        public async Task<IActionResult> CreateWalletPayment([FromBody] WalletPaymentDto dto)
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(buyerEmail))
                return Unauthorized();

            var result = await _paymentService.CreateWalletPaymentAsync(
                dto.BasketId,
                buyerEmail,
                dto.BillingAddress,
                dto.WalletNumber
            );

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // 💵 إنشاء دفعة كاش (Cash On Delivery)
        [HttpPost("cash")]
        public async Task<IActionResult> CreateCashPayment([FromBody] PaymentDto dto)
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(buyerEmail))
                return Unauthorized();

            var result = await _paymentService.CreateCashPaymentAsync(
                dto.BasketId,
                buyerEmail,
                dto.BillingAddress
            );

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // 💳 إنشاء دفعة كارت (Card Payment)
        [HttpPost("card")]
        public async Task<IActionResult> CreateCardPayment([FromBody] PaymentDto dto)
        {
            //var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            //if (string.IsNullOrEmpty(buyerEmail))
            //    return Unauthorized();

            //var result = await _paymentService.CreateCardPaymentAsync(
            //    dto.BasketId,
            //    buyerEmail,
            //    dto.BillingAddress
            //);

            //if (!result.IsSuccess)
            //    return BadRequest(result);

            return Ok();
        }
    }
}
