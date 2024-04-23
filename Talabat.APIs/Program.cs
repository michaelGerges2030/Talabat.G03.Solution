using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Net;
using System.Text.Json;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
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

			builder.Services.AddSwaggerServices();

			builder.Services.AddDbContext<StoreContext>(options =>
			{
				options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
			});

			builder.Services.AddSingleton<IConnectionMultiplexer>((serviceProvider) =>
			{
				var connection = builder.Configuration.GetConnectionString("Redis");
				return ConnectionMultiplexer.Connect(connection);
			});

			//builder.Services.AddScoped<IGenericRepository<Product>, GenericRepository<Product>>();
			//builder.Services.AddScoped<IGenericRepository<ProductBrand>, GenericRepository<ProductBrand>>();
			//builder.Services.AddScoped<IGenericRepository<ProductCategory>, GenericRepository<ProductCategory>>();

		
			builder.Services.AddAplicationServices();

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
				app.UseSwaggerMiddlewares();
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
