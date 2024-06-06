using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Talabat.APIs.CacheService;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Application.OrderService;
using Talabat.Application.PaymentService;
using Talabat.Application.ProductService;
using Talabat.Core;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Infrastructure;
using Talabat.Infrastructure._Identity.Config;
using Talabat.Repository;

namespace Talabat.APIs.Extensions
{
	public static class ApplicationServicesExtension
	{
		public static IServiceCollection AddAplicationServices(this IServiceCollection services) 
		{

			services.AddSingleton(typeof(IResponseCacheService), typeof(ResponseCacheService));

			services.AddScoped(typeof(IPaymentService), typeof(PaymentService));

			services.AddScoped(typeof(IProductService), typeof(ProductService));

			services.AddScoped(typeof(IOrderService), typeof(OrderService));

			services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));

			//services.AddScoped<IBasketRepository, BasketRepository>();
			services.AddScoped(typeof(IBasketRepository), typeof(BasketRepository));

			//services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

			//builder.Services.AddAutoMapper(M => M.AddProfile(new MappingProfiles()));
			services.AddAutoMapper(typeof(MappingProfiles));

			services.Configure<ApiBehaviorOptions>(options =>
			{
				options.InvalidModelStateResponseFactory = (actionContext) =>
				{
					var errors = actionContext.ModelState.Where(P => P.Value.Errors.Count() > 0)
														 .SelectMany(P => P.Value.Errors)
														 .Select(E => E.ErrorMessage)
														 .ToList();

					var response = new ApiValidationErrorResponse()
					{
						Errors = errors
					};

					return new BadRequestObjectResult(response);
				};
			});

			return services;
		}
	
		public static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration configuration) 
		{
			services.AddIdentity<ApplicationUser, IdentityRole>()
				.AddEntityFrameworkStores<ApplicationIdentityDbContext>();

			services.AddAuthServices(configuration);

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
					.AddJwtBearer(options =>
					{
						options.TokenValidationParameters = new TokenValidationParameters()
						{
							ValidateIssuer = true,
							ValidIssuer = configuration["JWT:ValidIssuer"],
							ValidateAudience = true,
							ValidAudience = configuration["JWT:ValidAudience"],
							ValidateIssuerSigningKey = true,
							IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:AuthKey"] ?? string.Empty)),
							ValidateLifetime = true,
							ClockSkew = TimeSpan.Zero,
						};
	
					});

			return services;
		}
	
	}
}
