using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace AprossUtils.HtmlHelpers
{

    public static class HtmlHelpers
    {
        private static readonly Regex _regex = new Regex(@"ALchangeMe([_\[])(.*?)_([0-9]*)_([_\]])");
        private static readonly JavaScriptEncoder _jsencoder = JavaScriptEncoder.Create(new TextEncoderSettings());

        public static IHtmlString RemoveLink(this HtmlHelper htmlHelper, string linkText, string container, string deleteElement, object htmlAttributes)
        {
            var js = string.Format("javascript:removeNestedForm(this,'{0}','{1}');return false;", container, deleteElement);


            TagBuilder tb = new TagBuilder("a");
            tb.Attributes.Add("href", "#");
            tb.Attributes.Add("onclick", js);
            tb.InnerHtml = linkText;
            var tag = tb.ToString(TagRenderMode.Normal);
            if (htmlAttributes != null)
                tb.MergeAttributes(
                    HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes),
                    replaceExisting: true);
            return MvcHtmlString.Create(tag);
        }


        public static IHtmlString AddLink<TModel>(this HtmlHelper<TModel> htmlHelper, string linkText, string containerElement, string counterElement, string collectionProperty, Type nestedType, string jsfunc = null)
        {
            var ticks = DateTime.UtcNow.Ticks;
            Dictionary<string, object> ALchangeMe = new Dictionary<string, object>();
            ALchangeMe[collectionProperty + "_" + ticks + "_"] = Activator.CreateInstance(nestedType);
            string partial = htmlHelper.EditorFor(x => ALchangeMe[collectionProperty + "_" + ticks + "_"]).ToHtmlString();
            string result = _regex.Replace(partial, "$2$1$3$4");
            string varjs = htmlHelper.IdForModel().ToHtmlString();

            var js = string.Format("javascript:addNestedForm('{0}','{1}','{2}','{3}',{4});return false;", containerElement, counterElement, ticks, "nf-clone-" + collectionProperty + "_" + ticks + "_", jsfunc);
            TagBuilder tb = new TagBuilder("a");
            tb.Attributes.Add("href", "#");
            tb.Attributes.Add("onclick", js);
            tb.InnerHtml = linkText;
            var atag = tb.ToString(TagRenderMode.Normal);


            tb = new TagBuilder("div");
            tb.Attributes.Add("id", "nf-clone-" + collectionProperty + "_" + ticks + "_");
            tb.Attributes.Add("class", "nf-clone");
            tb.Attributes.Add("style", "display: none");
            tb.InnerHtml = result;
            var divtag = tb.ToString(TagRenderMode.Normal);
            return MvcHtmlString.Create(atag + divtag);
        }

        public static string ReplaceLastOccurrence(string Source, string Find, string Replace)
        {
            int place = Source.LastIndexOf(Find);

            if (place == -1)
                return Source;

            string result = Source.Remove(place, Find.Length).Insert(place, Replace);
            return result;
        }
    }

}