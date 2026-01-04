using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreApis.Errors;

namespace StoreApis.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)] // This will hide this controller from Swagger documentation
    public class ErrorsController : BaseController
    {

        // this endpoint to handle errors globally
        public IActionResult Error (int code)
        {
            return NotFound(new ApiErrorResponse(StatusCodes.Status404NotFound,"The EndPoint Not Exist"));
        }
    }
}
