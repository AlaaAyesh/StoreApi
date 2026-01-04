using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreApis.Errors;
using StoreCore.Dtos.Basket;
using StoreCore.Entities;
using StoreCore.RepositoriesContract;
using StoreCore.ServicesContract;

namespace StoreApis.Controllers
{
 
    public class BasketController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IBasketService _basketService;

        public BasketController( IMapper mapper,IBasketService basketService)
        {
           
            _mapper= mapper;
           _basketService = basketService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBasket(string id)
        {
            var basket = await _basketService.GetBasketAsync(id);
            if (basket == null)
                return NotFound(new ApiErrorResponse(404));

            return Ok(basket);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrCreateBasket([FromBody] CustomerBasketDto basketDto)
        {
            if (basketDto == null)
                return BadRequest(new ApiErrorResponse(400));

            var basket = await _basketService.SaveOrUpdateBasketAsync(_mapper.Map<CustomerBasket>(basketDto));
            return Ok(basket);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBasket(string id)
        {
            await _basketService.DeleteBasketAsync(id);
            return NoContent();
        }
    }
}
