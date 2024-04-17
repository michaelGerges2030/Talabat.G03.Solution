
namespace Talabat.APIs.Errors
{
	public class ApiResponse
	{
		public int StatusCode { set; get; }
		public string? Message { set;  get; }

		public ApiResponse(int statusCode, string? message = null)
        {
			StatusCode = statusCode;
			Message = message?? GetDefaultMessageForStatusCode(statusCode);
		}

		private string? GetDefaultMessageForStatusCode(int statusCode)
		{
			return statusCode switch
			{
				400 => "A Bad Request, You Have Made!",
				401 => "Unauthorized!",
				404 => "Resource Was Not Found",
				500 => "Errors are the path to the dark side. Errors lead to Anger. Anger leads to Hate. Hate leads to career shifting",
				_ => null
			};

			#region Switch case old way
			//var message = string.Empty;
			//switch(statusCode)
			//{
			//	case 400:
			//		message = "A Bad Request, You Have Made!";
			//		break;
			//	case 401:
			//		message = "Unauthorized!";
			//		break;
			//	case 404:
			//		message = "Resource Was Not Found";
			//		break;
			//	case 500:
			//		message = "Errors are the path to the dark side. Errors lead to Anger. Anger leads to Hate. Hate leads to career shifting";
			//		break;
			//	default:
			//		message = null ;
			//		break;
			//}
			//return message;


			#endregion

		}
	}
}
