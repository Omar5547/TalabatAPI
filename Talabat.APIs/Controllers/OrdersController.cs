using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Services;
using Talabat.Services;

namespace Talabat.APIs.Controllers
{

    public class OrdersController : APIBaseController
    {
        private readonly IOrderServices _orderServices;
        private readonly IMapper _mapper;
        

        public OrdersController(IOrderServices orderServices, IMapper mapper)
        {
            _orderServices = orderServices;
            _mapper = mapper;
          
        }
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]

        [HttpPost] // Basurl / api/orders
        [Authorize]
        public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
        {
            var BuyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var MappedAddress = _mapper.Map<AddressDto, Address>(orderDto.shipToAddress);



            var Order = await _orderServices.CreateOrderAsync(BuyerEmail, orderDto.DeliveryMethodId, orderDto.BasketId, MappedAddress);

            if (Order is null)
            {
                return BadRequest(new ApiResponse(400, "There is a Problem With Your Orders"));
            }

            return Ok(Order);
        }
        [ProducesResponseType(typeof(OrderToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpGet] // Get => BaseUrl/api/Orders
        [Authorize]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser()
        {
            var BuyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var Orders = await _orderServices.GetOrdersForSpecificUserAsync(BuyerEmail);
            if (Orders is null)
            {
                return NotFound(new ApiResponse(404, "This is no Orders For This User"));
            }
            var MappedOrders = _mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(Orders);
            return Ok(Orders);

        }
        [ProducesResponseType(typeof(OrderToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpGet("{id}")]
        [Authorize]

        public async Task<ActionResult<OrderToReturnDto>> GetOrderByIdForUser(int id)
        {
            var BuyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var Order = await _orderServices.GetOrderByIdForSpecificuserAsync(id, BuyerEmail);
            if (Order is null)
            {

                return NotFound(new ApiResponse(404, $"There is no Order With Id = {id}For This User"));


            }
            var MappedOrder = _mapper.Map<Order ,OrderToReturnDto>(Order);
            return Ok(Order);


        }
        [HttpGet("DeliveryMethods")] // Get => baseUrl/api/Orders/Delivery/Methods
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods() 
        {
            var DeliveryMethods  =  await _orderServices.GetDeliveryMethodsAsync();
            return Ok(DeliveryMethods);
        }
    }
}
