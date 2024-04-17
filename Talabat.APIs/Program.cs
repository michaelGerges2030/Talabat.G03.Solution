using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.APIs.Middlewares;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Repository;
using Talabat.Repository.Data;

namespace Talabat.APIs
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			#region Configure Services
			
			// Add services to the container.

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			builder.Services.AddDbContext<StoreContext>(options =>
			{
				options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
			});

			//builder.Services.AddScoped<IGenericRepository<Product>, GenericRepository<Product>>();
			//builder.Services.AddScoped<IGenericRepository<ProductBrand>, GenericRepository<ProductBrand>>();
			//builder.Services.AddScoped<IGenericRepository<ProductCategory>, GenericRepository<ProductCategory>>();

			builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

			//builder.Services.AddAutoMapper(M => M.AddProfile(new MappingProfiles()));
			builder.Services.AddAutoMapper(typeof(MappingProfiles));

			builder.Services.Configure<ApiBehaviorOptions>(options =>
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

			#endregion

			var app = builder.Build();

			using var scope = app.Services.CreateScope();

		    var services = scope.ServiceProvider;

		    var _dbContext = services.GetRequiredService<StoreContext>();

			var loggerFactory = services.GetRequiredService<ILoggerFactory>();
			var _logger = loggerFactory.CreateLogger<Program>();

			try
			{
				await _dbContext.Database.MigrateAsync();
				await StoreContextSeed.SeedAsync(_dbContext);
			}
			catch (Exception ex)
			{	
				_logger.LogError(ex.StackTrace.ToString());
			}


			#region Configure Kestrel Middlewares

			app.UseMiddleware<ExceptionMiddleware>();

			#region MiddleWare another way
	//		app.Use(async (httpContext, _next) =>
	//{
	//	try
	//	{
	//		//writing code that takes action with the Request

	//		await _next.Invoke(httpContext); //Go to the next Middleware

	//		//writing code that takes action with the Response
	//	}
	//	catch (Exception ex)
	//	{
	//		_logger.LogError(ex.Message);

	//		httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
	//		httpContext.Response.ContentType = "application/json";

	//		var response = app.Environment.IsDevelopment() ?
	//			new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString())
	//			:
	//			new ApiExceptionResponse((int)HttpStatusCode.InternalServerError);

	//		var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

	//		var json = JsonSerializer.Serialize(response, options);

	//		await httpContext.Response.WriteAsync(json);

	//	}
	//}); 
			#endregion


			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseStatusCodePagesWithReExecute("/Errors");

			app.UseHttpsRedirection();

			app.UseStaticFiles();

			//app.UseAuthorization();
			app.MapControllers();

			#endregion

			app.Run();
		}
	}
}
