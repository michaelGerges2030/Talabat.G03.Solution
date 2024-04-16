using Microsoft.EntityFrameworkCore;
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

			#endregion

			var app = builder.Build();

			using var scope = app.Services.CreateScope();

		    var services = scope.ServiceProvider;

		    var _dbContext = services.GetRequiredService<StoreContext>();

			var loggerFactory = services.GetRequiredService<ILoggerFactory>();

			try
			{
				await _dbContext.Database.MigrateAsync();
				await StoreContextSeed.SeedAsync(_dbContext);
			}
			catch (Exception ex)
			{
				var logger = loggerFactory.CreateLogger<Program>();
				logger.LogError(ex.StackTrace.ToString());
			}
		

			#region Configure Kestrel Middlewares
			
			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			//app.UseAuthorization();
			app.MapControllers();

			#endregion

			app.Run();
		}
	}
}
