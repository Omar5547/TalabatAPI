using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;

namespace Talabat.APIs.Controllers
{

    public class BasketController : APIBaseController
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketController(IBasketRepository basketRepository , IMapper mapper)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }
        // Get or ReCreate Basket
        [HttpGet]
        public async Task<ActionResult<CustomerBasket>> GetCustomerBasket(string BasketId) 
        {
            var Basket = await _basketRepository.GetBasketAsync(BasketId);
            if (Basket is null) 
            {
                return new CustomerBasket(BasketId);
            }
            else 
            {
                return Ok(Basket);
            }
        }
        // Update Or Create New Basket
        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDto basket) 
        {
            var MappedBasket = _mapper.Map<CustomerBasketDto, CustomerBasket>(basket);
            var CreatedOrUpatedBasket = await _basketRepository.UpdateBasketAsync(MappedBasket);
            if (CreatedOrUpatedBasket is null) 
            {
                return BadRequest(new ApiResponse(400));

            }
            return Ok(CreatedOrUpatedBasket);
        }
        // Delete
        [HttpDelete]
        public async Task<ActionResult<bool>> DeleteBasket(string BasketId) 
        {
          return  await _basketRepository.DeleteBasketAsync(BasketId);
        }
    }
}
