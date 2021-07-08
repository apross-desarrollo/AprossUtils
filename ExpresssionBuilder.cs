using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

namespace AprossUtils
{
    public class ExpresssionBuilder<T>
    {
        public static Expression<Func<T, bool>> FromQueryString(string query)
        {



            var nvc = HttpUtility.ParseQueryString(query);
            return FromNameValueCollection(nvc);
        }


        public static Expression<Func<T, bool>> FromNameValueCollection(NameValueCollection nvc)
        {

            var dic = nvc.AllKeys.ToDictionary(k => k, k => (object)nvc[k]);
            return FromDict(dic);
        }

        public static Expression<Func<T, bool>> FromDict(Dictionary<string, object> properties)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("es-AR");

            if (properties == null || properties.Count <= 0) return default;
            List<string> exps = new List<string>();
            List<object> objs = new List<object>();
            int index = 0;
            foreach (string key in properties.Keys)
            {

                object val = properties[key];
                if (val != null)
                {
                    exps.Add(key.Replace("@value", "@" + index));
                    objs.Add(val);
                    index++;
                }
            }


            LambdaExpression e = DynamicExpressionParser.ParseLambda(
                typeof(T),
                typeof(bool),
                string.Join(" AND ", exps.ToArray()),
                objs.ToArray()
                );
            return (Expression<Func<T, bool>>)e;
        }
    }
    public enum StringOperator
    {
        StartsWith = 1,
        Contains = 2,
        EndsWith = 3,
    }


    public static class LinqQueries
    {
        private static MethodInfo miTL = typeof(String).GetMethod("ToLower", Type.EmptyTypes);
        private static MethodInfo miS = typeof(String).GetMethod("StartsWith", new Type[] { typeof(String) });
        private static MethodInfo miC = typeof(String).GetMethod("Contains", new Type[] { typeof(String) });
        private static MethodInfo miE = typeof(String).GetMethod("EndsWith", new Type[] { typeof(String) });

        public static IQueryable<T> FilterByString<T>(this IQueryable<T> query,
                                                      Expression<Func<T, string>> propertySelector,
                                                      StringOperator operand,
                                                      string value)
        {
            ParameterExpression parameterExpression = null;
            var memberExpression = GetMemberExpression(propertySelector.Body, out parameterExpression);
            var dynamicExpression = Expression.Call(memberExpression, miTL);
            Expression constExp = Expression.Constant(value.ToLower());
            switch (operand)
            {
                case StringOperator.StartsWith:
                    dynamicExpression = Expression.Call(dynamicExpression, miS, constExp);
                    break;
                case StringOperator.Contains:
                    dynamicExpression = Expression.Call(dynamicExpression, miC, constExp);
                    break;
                case StringOperator.EndsWith:
                    dynamicExpression = Expression.Call(dynamicExpression, miE, constExp);
                    break;
            }

            var pred = Expression.Lambda<Func<T, bool>>(dynamicExpression, new[] { parameterExpression });
            return query.Where(pred);

        }


        public static Expression GetMemberExpression(Expression expression, out ParameterExpression parameterExpression)
        {
            parameterExpression = null;
            if (expression is MemberExpression)
            {
                var memberExpression = expression as MemberExpression;
                while (!(memberExpression.Expression is ParameterExpression))
                    memberExpression = memberExpression.Expression as MemberExpression;
                parameterExpression = memberExpression.Expression as ParameterExpression;
                return expression as MemberExpression;
            }
            if (expression is MethodCallExpression)
            {
                var methodCallExpression = expression as MethodCallExpression;
                parameterExpression = methodCallExpression.Object as ParameterExpression;
                return methodCallExpression;
            }
            return null;
        }
    }
}