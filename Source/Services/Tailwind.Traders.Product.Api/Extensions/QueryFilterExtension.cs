using System;
using System.Linq;
using System.Linq.Expressions;

namespace Tailwind.Traders.Product.Api.Extensions
{
    public static class QueryFilterExtension
    {
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            var criteria = Expression.OrElse(left.Body, Expression.Invoke(right, left.Parameters.Single()));
            return Expression.Lambda<Func<T, bool>>(criteria, left.Parameters.Single());
        }
    }
}
