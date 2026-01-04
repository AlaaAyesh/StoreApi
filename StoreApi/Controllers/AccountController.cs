using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StoreApis.Errors;
using StoreApis.Extenstions;
using StoreCore.Dtos.Auth;
using StoreCore.Entities.Identity;
using StoreCore.ServicesContract;

namespace StoreApis.Controllers
{

    public class AccountController : BaseController
    {
        private readonly IUserService _userService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;


        public AccountController(
            IUserService userService,
            UserManager<AppUser> userManager ,
            IMapper mapper,
            ITokenService  tokenService
            )
        {
            _userService = userService;
            _userManager = userManager;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userService.LoginAsync(loginDto);
            if (user == null) return Unauthorized(new ApiErrorResponse(StatusCodes.Status401Unauthorized));
            return Ok(user);
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var user = await _userService.RegisterAsync(registerDto);
            if (user == null) return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest, "\"User already exists\""));
            return Ok(user);
        }

        [HttpGet("GetCurrentUser")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest));
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest));
            return Ok( new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _tokenService.CreateTokenAsync(user,_userManager),

            });

        }


        [HttpGet("GetCurrentUserAddress")]
        [Authorize]
        public async Task<ActionResult<AddressDto>> GetCurrentUserAddress()
        {
            var user = await _userManager.GetUserEmailWithAddress(User);
            if (user == null) return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest));
            return Ok(_mapper.Map<AddressDto>(user.Address));
        }




        [HttpPut("UpdateUserAddress")]
        [Authorize]
        public async Task<ActionResult<AddressDto>> UpdateUserAddress([FromBody] AddressDto addressDto)
        {
            var user = await _userManager.GetUserEmailWithAddress(User);
            if (user == null) return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest));
            user.Address = _mapper.Map<Address>(addressDto);
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded) return Ok(_mapper.Map<AddressDto>(user.Address));
            return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest, "Problem updating the user"));
        }


    }
}
