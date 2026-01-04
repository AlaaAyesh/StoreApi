using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreApis.Errors;
using StoreCore;
using StoreCore.Dtos.Orders;
using StoreCore.Dtos.Payments;
using StoreCore.Entities.Identity;
using StoreCore.Entities.Order;
using StoreCore.ServicesContract;

namespace StoreApis.Controllers
{
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IOrderService orderService, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _orderService = orderService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderDto model)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
                return Unauthorized(new ApiErrorResponse(StatusCodes.Status401Unauthorized));

            var address = _mapper.Map<OrderAddress>(model.ShipToAddress);

            (Order order, PaymentResultDto paymentResult) = await _orderService.CreateOrderAsync(
                userEmail, model.DeliveryMethodId, model.BasketId, address, model.PaymentMethod, model.WalletNumber);

            if (order == null)
                return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest, paymentResult.Message));

            var orderDto = _mapper.Map<OrderToReturnDto>(order);

            return Ok(new
            {
                Order = orderDto,
                PaymentUrl = paymentResult.PaymentUrl
            });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetOrderForSpecificUser()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
                return Unauthorized(new ApiErrorResponse(StatusCodes.Status401Unauthorized));

            var orders = await _orderService.GetOrdersForUserAsync(userEmail!);

            if (orders == null)
                return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest));

            return Ok(_mapper.Map<IEnumerable<OrderToReturnDto>>(orders));
        }

        [Authorize]
        [HttpGet("{OrderId}")]
        public async Task<IActionResult> GetOrderForSpecificUser(int OrderId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
                return Unauthorized(new ApiErrorResponse(StatusCodes.Status401Unauthorized));

            var order = await _orderService.GetOrderByIdAsync(OrderId, userEmail!);

            if (order == null)
                return NotFound(new ApiErrorResponse(StatusCodes.Status404NotFound));

            return Ok(_mapper.Map<OrderToReturnDto>(order));
        }

        [HttpGet("DeliveryMethod")]
        public async Task<IActionResult> GetDeliveryMethod()
        {
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod, int>().GetAllAsync();
            if (deliveryMethod == null)
                return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest));

            return Ok(deliveryMethod);
        }
    }
}
