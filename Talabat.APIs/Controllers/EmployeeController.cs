using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Specifications.Employee_Specs;
using Talabat.Core.Specifications.Product_Specs;

namespace Talabat.APIs.Controllers
{
	//Draft example to see what specifications design pattern do and how it's important

	[Route("api/[controller]")]
	[ApiController]
	public class EmployeeController : ControllerBase
	{
		private readonly IGenericRepository<Employee> _employeeRepo;

		public EmployeeController(IGenericRepository<Employee> employeeRepo)
        {
			_employeeRepo = employeeRepo;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
		{
			var spec = new EmployeeWithDepartmentSpecifications();	
			var employees = await _employeeRepo.GetAllWithSpecAsync(spec);

			return Ok(employees);
		}


		[HttpGet("{id}")]
		public async Task<ActionResult<IEnumerable<Employee>>> GetEmployee(int id)
		{
			var spec = new EmployeeWithDepartmentSpecifications(id);
			var employees = await _employeeRepo.GetAllWithSpecAsync(spec);

			if(employees is null) 
				return NotFound(new ApiResponse(404));	

			return Ok(employees);
		}
	}
}
