using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace AprossUtils.HtmlHelpers
{

    public class AutocompleteItem
    {
        public string Id { get; set; }
        public string Value { get; set; }
    }

    public static class AutocompleteHelper
    {


        public static MvcHtmlString AutocompleteFor<TModel>(this HtmlHelper<TModel> html, Expression<Func<TModel, AutocompleteItem>> expression, object additionalViewData)
        {

            Func<TModel, AutocompleteItem> deleg = expression.Compile();
            AutocompleteItem result = deleg(html.ViewData.Model);
            if (result == null)
            {
                result = new AutocompleteItem();
            }
            string original_name = GetHtmlIdNameFor(expression);
            string original_id = html.IdFor(expression).ToHtmlString();

            MvcHtmlString hidden = html.Hidden(original_name + ".Id", result.Id, new { @class = "autocomplete", datasearch = original_id + "_Value" });

            IDictionary<string, object> attrs = new RouteValueDictionary(additionalViewData);
            var htmlAttributes = attrs["htmlAttributes"];
            attrs = new RouteValueDictionary(htmlAttributes);
            MvcHtmlString text = html.TextBox(original_name + ".Value", result.Value, attrs);


            return new MvcHtmlString(hidden.ToString() + text.ToString());
        }

        private static string GetHtmlIdNameFor<TModel, TValue>(Expression<Func<TModel, TValue>> expression)
        {
            if (expression.Body.NodeType == ExpressionType.Call)
            {
                var methodCallExpression = (MethodCallExpression)expression.Body;
                string name = GetHtmlIdNameFor(methodCallExpression);
                return name.Substring(expression.Parameters[0].Name.Length + 1).Replace('.', '_');

            }
            return expression.Body.ToString().Substring(expression.Parameters[0].Name.Length + 1).Replace('.', '_');
        }

        private static string GetHtmlIdNameFor(MethodCallExpression expression)
        {
            var methodCallExpression = expression.Object as MethodCallExpression;
            if (methodCallExpression != null)
            {
                return GetHtmlIdNameFor(methodCallExpression);
            }
            return expression.Object.ToString();
        }
    }
}