using System.ComponentModel.DataAnnotations;

namespace Talabat.APIs.Dtos
{
	public class RegisterDto
	{
		[Required]
		public string DisplayName { get; set; } = null!;

		[Required]
		[EmailAddress]
		public string Email { get; set; } = null!;


		[Required]
		public string Phone { get; set; } = null!;

		[Required]
		[RegularExpression("(?=.*[a-z])(?=.*[A-Z])(?=.*\\d|\\W).{6,}"
			, ErrorMessage = "Password must have one uppercase, 1 lowercase, 1 non alphanumeric and at least 6 chars")]
		public string Password { get; set; } = null!;
	}
}
