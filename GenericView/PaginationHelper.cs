using System;
using System.Collections.Specialized;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace AprossUtils.GenericView
{
    public static class PaginationHelper
    {
        public static MvcHtmlString PaginationFor<TModel>(this HtmlHelper<GenericView<TModel>> html, GenericViewPagination pagination) where TModel: new()
        {
            var model = html.ViewData.Model;
            MvcHtmlString html_string = html.Partial("GenericViewPagination", pagination);
            return html_string;
        }
    }
}
