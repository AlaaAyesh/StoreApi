using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StoreCore.Entities.Identity;

namespace StoreApis.Extenstions
{
    public static class UserMangerExtensions
    {
        public static async Task<AppUser> GetUserEmailWithAddress( this UserManager<AppUser>userManager,ClaimsPrincipal User)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return null;
            var user =  await userManager.Users.Include(u=>u.Address).FirstOrDefaultAsync(u=>u.Email==email);
            if (user == null) return null;
            return user;
        }
    }
}
