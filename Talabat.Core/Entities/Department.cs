using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities
{
	//Draft example to see what specifications design pattern do and how it's important
	public class Department: BaseEntity
	{
        public string Name { get; set; }
        public DateOnly DateOfCreation { get; set; }
    }
}
