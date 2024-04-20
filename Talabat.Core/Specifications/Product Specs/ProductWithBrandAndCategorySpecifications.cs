using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications.Product_Specs
{
	public class ProductWithBrandAndCategorySpecifications: BaseSpecifications<Product>
	{
        public ProductWithBrandAndCategorySpecifications(ProductSpecParams SpecParams)
			:base( P =>
			                   (!SpecParams.BrandId.HasValue || P.BrandId == SpecParams.BrandId.Value) &&
			                   (!SpecParams.CategoryId.HasValue || P.CategoryId == SpecParams.CategoryId.Value)
			)
        {
			AddIncludes();

			if (!string.IsNullOrEmpty(SpecParams.Sort))
			{
				switch (SpecParams.Sort) 
				{
					case "priceAsc":
						AddOrderBy(P => P.Price);
						break;
					case "priceDesc":
						AddOrderByDesc(P => P.Price);
						break;
					default:
						AddOrderBy(P => P.Name);
						break;
				}
			}
			else
				AddOrderBy(P => P.Name);


			ApplyPagination((SpecParams.PageIndex - 1) * SpecParams.PageSize, SpecParams.PageSize);
        }

		public ProductWithBrandAndCategorySpecifications(int id) : base(P => P.Id == id)
		{
			AddIncludes();
		}

		private void AddIncludes()
		{
			Includes.Add(P => P.Brand);
			Includes.Add(P => P.Category);
		}
	}
}
