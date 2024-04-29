using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Repositories.Contract;

namespace Talabat.APIs.Controllers
{
	public class AccountController : BaseAPIController
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IAuthService _authService;

		public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
			IAuthService authService)
        {
			_userManager = userManager;
			_signInManager = signInManager;
			_authService = authService;
		}


		[HttpPost("login")]
		public async Task<ActionResult<UserDto>> Login(LoginDto model)
		{
			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user is null) return Unauthorized(new ApiResponse(401, "Invalid Login"));

			var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
			if(!result.Succeeded) return Unauthorized(new ApiResponse(401, "Invalid Login"));

			return Ok(new UserDto()
			{
				DiaplayName = user.DisplayName,
				Email = user.Email,
				Token = await _authService.CreateTokenAsync(user, _userManager)
			});
		}

		[HttpPost("register")]
		public async Task<ActionResult<UserDto>> Register(RegisterDto model)
		{
			var user = new ApplicationUser()
			{
				DisplayName = model.DisplayName,
				Email = model.Email,
				UserName = model.DisplayName,
				PhoneNumber = model.Phone
			};

			var result = await _userManager.CreateAsync(user);

			if (!result.Succeeded) return BadRequest(new ApiValidationErrorResponse() { Errors = result.Errors.Select(E => E.Description) });

			return Ok(new UserDto()
			{
				DiaplayName = user.DisplayName,
				Email = user.Email,
				Token = await _authService.CreateTokenAsync(user, _userManager)
			});
	}


		[Authorize]
		[HttpGet]
		public async Task<ActionResult<UserDto>> GetCurrentUser()
		{
			var email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

			var user = await _userManager.FindByEmailAsync(email);

			return Ok(new UserDto()
			{
				DiaplayName = user?.DisplayName ?? string.Empty,
				Email = user?.Email ?? string.Empty,
				Token = await _authService.CreateTokenAsync(user, _userManager)
			}) ;
		}


		[Authorize]
		[HttpGet("address")]
		public async Task<ActionResult<Address>> GetUserAddress()
		{
			var user = await _userManager.FindUserWithAddressAsync(User);

				return Ok(user.Address);	
		}

	}


}
