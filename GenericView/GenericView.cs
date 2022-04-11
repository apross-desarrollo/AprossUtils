using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace AprossUtils.GenericView
{

    public abstract class GenericView<TModel>
    {
        public const string PageString = "Page";
        public const string PageSizeString = "PageSize";
        public const string OrderString = "order";

        public GenericViewPagination Pagination;
        public Expression<Func<TModel, bool>> Filters;
        public List<string> OrderBy;
        public List<string> Searchs;

        public List<TModel> Objects;

        public virtual string[] ListFields { get => new string[] { }; }
        public virtual string[] FiterFields { get => new string[] { }; }
        public virtual string[] SearchFields { get => new string[] { }; }
        public virtual string[] OrderFields { get => new string[] { }; }
        public abstract Task LoadObjects(Expression<Func<TModel, bool>> expression);
        public abstract Task LoadObjects(string query);
        public NameValueCollection QueryString { get; set; }


        public void ProcessRequest(NameValueCollection querystring)
        {

            QueryString = querystring;
            if (Pagination is null) Pagination = new GenericViewPagination();
            Pagination.QueryString = QueryString;
            if (int.TryParse(QueryString.Get(PageSizeString), out int pageSize))
            {
                Pagination.PageSize = pageSize;
            }
            if (int.TryParse(QueryString.Get(PageString), out int actualPage))
            {
                Pagination.ActualPage = actualPage;
            }
            else
            {
                Pagination.ActualPage = 0;
            }
            string order = QueryString.Get(OrderString);
            OrderBy = new List<string>();
            if (!string.IsNullOrEmpty(order))
            {
                OrderBy.Add(order);
            }

            NameValueCollection filters = new NameValueCollection();
            foreach (var key in QueryString.AllKeys)
            {
                if (FiterFields.Any(x => x == key))
                {
                    filters.Add(key, QueryString[key]);
                }
            }
            Filters = ExpresssionBuilder<TModel>.FromNameValueCollection(filters);
        }
        public void ProcessRequest(HttpRequestBase request)
        {
            ProcessRequest(request.QueryString);
        }

        public void ProcessFilterForm<T>(T filterForm) where T : GenericViewFilterForm
        {

            Dictionary<string, object> filters = new Dictionary<string, object>();
            List<string> searchs = new List<string>();
            foreach (var prop in typeof(T).GetProperties())
            {
                var attr = (GenericViewFilterAttribute)prop.GetCustomAttributes(typeof(GenericViewFilterAttribute), false).FirstOrDefault();
                var value = prop.GetValue(filterForm);
                if (attr != null && value != null)
                {
                    filters.Add(attr.Pattern, prop.GetValue(filterForm));
                }

                var search_attrs = (GenericViewSearchAttribute[])prop.GetCustomAttributes(typeof(GenericViewSearchAttribute), false);
                foreach (var a in search_attrs)
                {

                    if (value != null)
                    {
                        var words = value.ToString().Split(' ');

                        foreach (var w in words)
                        {
                            if (!String.IsNullOrWhiteSpace(w)) searchs.Add(w.Trim());
                        }
                    }

                }
            }
            Filters = ExpresssionBuilder<TModel>.FromDict(filters);
            Searchs = searchs;
            filterForm.PostProcess();
        }
    }

    public class GenericViewPagination
    {
        public NameValueCollection QueryString;
        private int? _pageSize = 50;
        public int? PageSize
        {
            get => _pageSize;
            set { if (value > 0) _pageSize = value; }
        }
        private int _actualPage;
        public int ActualPage
        {
            get => _actualPage;
            set
            {
                if (value < 0) _actualPage = 0;
                else _actualPage = value;
            }
        }
        public long? RecordCount { get; set; }
        public long PageCount
        {
            get
            {
                return LastPage + 1;
            }
        }
        public int GetSkip()
        {
            if (PageSize is null || PageSize == 0) return 0;
            return ActualPage * (int)PageSize;
        }

        public long FirstRecord { get => GetSkip() + 1; }
        public long LastPage
        {
            get
            {
                if (PageSize is null || PageSize == 0 || RecordCount == null) return 0;
                var rest = RecordCount % PageSize;
                var d = Decimal.ToInt32((decimal)RecordCount / (int)PageSize);
                if (rest == 0) return d - 1;
                return d;
            }
        }

        public long? LastRecord
        {
            get
            {
                if (PageSize == null) return RecordCount;
                if (ActualPage == LastPage) return GetSkip() + (RecordCount % PageSize);
                return GetSkip() + PageSize;
            }
        }

        private NameValueCollection _qs;
        public string BuildQueryString(long page)
        {
            if (_qs == null) _qs = new NameValueCollection(QueryString);
            _qs[GenericView<object>.PageString] = page.ToString();
            string q = String.Join("&",
             _qs.AllKeys.Select(a => a + "=" + HttpUtility.UrlEncode(_qs[a])));
            return q;
        }
    }
}
