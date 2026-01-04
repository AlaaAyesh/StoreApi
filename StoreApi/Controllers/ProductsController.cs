using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreApis.Attributes;
using StoreApis.Errors;
using StoreCore.Dtos.Products;
using StoreCore.Helper;
using StoreCore.ServicesContract;
using StoreCore.Specificatons.product;

namespace StoreApis.Controllers
{

    public class ProductsController : BaseController
    {
        private readonly IProductService _productService;

        public ProductsController (IProductService productService)
        {
            _productService = productService;
        }

        [ProducesResponseType(typeof(PaginationResponse<ProductDto>), StatusCodes.Status200OK)]
        [HttpGet] // Get all products
        [Cached(100)]
        public async Task<ActionResult<PaginationResponse<ProductDto>>> GetAllProducts([FromQuery] ProductSpecParams productSpecParams)
        {
            var result = await _productService.GetAllProductsAsync(productSpecParams);
            return Ok(new PaginationResponse<ProductDto>(productSpecParams.PageSize, productSpecParams.PageIndex,result.TotalCount , result.Data));
        }


        [ProducesResponseType(typeof(PaginationResponse<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{id}")] // Get product by id
        public async Task<IActionResult> GetProductById(int id)
        {
            var result = await _productService.GetProductByIdAsync(id);
            if (result == null)
            {
                return NotFound(new ApiErrorResponse(404,$"The Product Id {id} Not Exist"));
            }
            return Ok(result);
        }

        [ProducesResponseType(typeof(IEnumerable<TypeBrandDto>), StatusCodes.Status200OK)]
        [HttpGet("brands")]// Get all brands
        public async Task<ActionResult<IEnumerable<TypeBrandDto>>> GetAllBrand()
        {
            var result = await _productService.GetAllBrandsAsync();
            return Ok(result);
        }

        [ProducesResponseType(typeof(IEnumerable<TypeBrandDto>), StatusCodes.Status200OK)]
        [HttpGet("types")]// Get all types
        public async Task<IActionResult> GetAllTypes()
        {  
            var result = await _productService.GetAllTypesAsync();
            return Ok(result);
        }  

    }
}
