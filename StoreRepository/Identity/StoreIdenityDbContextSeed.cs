using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using StoreCore.Entities.Identity;
using StoreRepository.Identity.Contexts;

namespace StoreRepository.Identity
{
    public static class StoreIdenityDbContextSeed
    {
        public static async Task SeedAppUserAsync(UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var user = new AppUser
                {
                    DisplayName = "Alaa Ayesh",
                    Email = "alaaamen686@gmail.com",
                    UserName = "alaa.ayesh",
                    PhoneNumber = "0599999999",
                    Address = new Address
                    {
                        FirstName = "Alaa",
                        LastName = "Ayesh",
                        Street = "123 Main St",
                        City = "Ramallah",
                        State = "State",
                        Country = "Palestine",
                    }
                };

                await userManager.CreateAsync(user, "Lolo1372002");
            }

        }

    }
}
