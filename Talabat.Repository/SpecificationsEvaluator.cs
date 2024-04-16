using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Infrastructure
{
	internal class SpecificationsEvaluator<TEntity> where TEntity : BaseEntity
	{
		public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecifications<TEntity> spec)
		{
			var query = inputQuery;

			if (spec.Criteria is not null)
				query = query.Where(spec.Criteria);

			query = spec.Includes.Aggregate(query, (currentQuery, includeExpression) => currentQuery.Include(includeExpression));

			return query;
		}
	}
}
