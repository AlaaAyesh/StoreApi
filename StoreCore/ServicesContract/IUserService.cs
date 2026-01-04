using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreCore.Dtos.Auth;

namespace StoreCore.ServicesContract
{
    public interface IUserService
    {
        Task<UserDto> LoginAsync(LoginDto loginDto);    
        Task<UserDto> RegisterAsync(RegisterDto registerDto);   
        
        Task<bool> UserExists(string email);
    }
}
