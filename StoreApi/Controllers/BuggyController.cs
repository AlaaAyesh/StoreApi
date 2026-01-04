using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreApis.Errors;
using StoreRepository.Data.Contexts;

namespace StoreApis.Controllers
{
    
    public class BuggyController : BaseController
    {
        private readonly StoreDbContext _context;
        public BuggyController(StoreDbContext context)
        {
            _context = context;
        }

        [HttpGet("notfound")] // api/buggy/notfound
        public async Task<IActionResult> GetNotFoundRequestError()
        {
            var brand = await _context.Brands.FindAsync(100);
            if (brand == null) return NotFound(new ApiErrorResponse(404));
            return Ok(brand);
        }

        [HttpGet("servererror")] // api/buggy/servererror
        public async Task<IActionResult> GetServerRequestError()
        {
            var brand = await _context.Brands.FindAsync(100);
            var brandToReturn = brand.ToString(); // This will throw a null reference exception
            return Ok(brandToReturn);

        }

        [HttpGet("badrequest")] // api/buggy/badrequest
        public async Task<IActionResult> GetBadRequestError()
        {

            return BadRequest(new ApiErrorResponse(400));
     
        }

        [HttpGet("badrequest/{id}")] // api/buggy/badrequest/5
        public async Task<IActionResult> GetNotFoundRequestError(int id)
        {
            return Ok();
        }

        [HttpGet("unauthorized")] // api/buggy/unauthorized
        public async Task<IActionResult> GetUnauthorizedRequestError()
        {
            return Unauthorized(new ApiErrorResponse(401));
        }


         
    }

}
