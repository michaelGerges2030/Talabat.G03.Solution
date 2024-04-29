using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Infrastructure._Identity
{
	public static class ApplicationIdentityContextSeed
	{
		public static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
		{
			if(!userManager.Users.Any()) 
			{
				var user = new ApplicationUser()
				{
					DisplayName = "Michael Sameh",
					Email = "michaelsameh2030@gmail.com",
					UserName = "michael.sameh",
					PhoneNumber = "+201276091313"
				};

				await userManager.CreateAsync(user, "P@ssw0rd");
			}
		}
	}
}
