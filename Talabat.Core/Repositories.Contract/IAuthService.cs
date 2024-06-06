using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Core.Repositories.Contract
{
	public interface IAuthService
	{
		Task<string> CreateTokenAsync(ApplicationUser user, UserManager<ApplicationUser> userManager);
	}
}
