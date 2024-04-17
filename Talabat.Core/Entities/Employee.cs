using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities
{
	//Draft example to see what specifications design pattern do and how it's important
	public class Employee: BaseEntity
	{
        public string Name { get; set; }
        public decimal Salary { get; set; }

        public int? Age{ get; set; }

        public Department Department { get; set; }
    }
}
