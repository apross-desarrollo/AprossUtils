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
}