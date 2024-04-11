﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;

namespace Talabat.APIs.Controllers
{
	public class ProductController : BaseAPIController
	{
		private readonly IGenericRepository<Product> _productsRepo;

		public ProductController(IGenericRepository<Product> productsRepo)
        {
			_productsRepo = productsRepo;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
		{
			var products = await _productsRepo.GetAllAsync();

			return Ok(products);	
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Product>> GetProduct(int id)
		{
			var product = await _productsRepo.GetAsync(id);

			if (product is null)
				return NotFound(new { Message = "Not Found", StatusCode = 404});

			return Ok(product);
		}
    }
}
