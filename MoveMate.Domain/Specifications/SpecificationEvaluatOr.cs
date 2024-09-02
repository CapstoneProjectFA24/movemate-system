using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Domain.Specifications
{
    public class SpecificationEvaluatOr<T> where T : class
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecifications<T> spec)
        {
            var Query = inputQuery.AsQueryable();
            if (spec.Criteria != null)
            {
                Query = Query.Where(spec.Criteria);
            }
            if (spec.OrderBy != null)
            {
                Query = Query.OrderBy(spec.OrderBy);
            }
            if (spec.OrderByDescending != null)
            {
                Query = Query.OrderByDescending(spec.OrderByDescending);
            }
            if (spec.isPagingEnabled != null)
            {
                Query = Query.Skip(spec.Skip).Take(spec.Take);
            }
            Query = spec.Includes.Aggregate(Query, (current, include) => current.Include(include));
            return Query;
        }
    }
}
