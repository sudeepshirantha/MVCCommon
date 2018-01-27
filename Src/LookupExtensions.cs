using System.Linq.Expressions;
using System.Web.Routing;

namespace System.Web.Mvc.Html
{
    /// <summary>
    /// MVC HtmlHelper extension methods - extensions that make use of jQuery Validates native unobtrusive data validation properties
    /// </summary>
    public static class LookupExtensions
    {
        /// <summary>
        /// Render a TextBox for the supplied model using native jQuery Validate Unobtrusive extensions (only if true passed)
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="useNativeUnobtrusiveAttributes">Pass true if you want to use native extensions</param>
        /// <param name="format">OPTIONAL</param>
        /// <param name="htmlAttributes">OPTIONAL</param>
        /// <returns></returns>
        public static IHtmlString BizLookupFor<TModel, TProperty, TDisplay>(
          this HtmlHelper<TModel> htmlHelper,
          Expression<Func<TModel, TProperty>> expression,
          Expression<Func<TModel, TDisplay>> expressionDisplay,
          string dependentField = null,
          bool showLable = true,
          string icon = null,
          string format = null,
          string title = null,
          string url = null,
          object htmlAttributes = null)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var metadataDisplay = ModelMetadata.FromLambdaExpression(expressionDisplay, htmlHelper.ViewData);
            var attributes = Mapper.GetUnobtrusiveValidationAttributes(htmlHelper, expressionDisplay, htmlAttributes, metadataDisplay);

            var textBox = Mapper.GenerateHtmlWithoutMvcUnobtrusiveAttributes(() =>
                htmlHelper.TextBoxFor(expressionDisplay, format, attributes));

            string result = "";

            //Script
            UrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            result += htmlHelper.Partial("_LookupScript", new ViewDataDictionary { 
                        { "Id", metadata.PropertyName }, 
                        { "Field", metadata.PropertyName }, 
                        { "DisplayField", htmlHelper.IdFor(expressionDisplay).ToString() }, 
                        { "DependentField", dependentField }, 
                        { "Title", title }, 
                        { "Url", urlHelper.Content(url) } });

            if (showLable)
            {
                //Lable
                result += metadata.DisplayName ?? metadata.PropertyName;
                if (String.IsNullOrEmpty(result))
                {
                    result += String.Empty;
                }
            }

            //Input
            result += "<div class='input input-file'> ";
            result += "    <a id='btn" + metadata.PropertyName + "' data-id='@Model." + metadata.PropertyName + "' data-toggle='modal' data-target='#bizLookup'>";
            result += "        <span class='button'>List</span>";
            result += "    </a>";
            if (icon != null)
            {
                result += "<i class='icon-append fa " + icon + "'></i>";
            }
            result += htmlHelper.HiddenFor(expression).ToHtmlString();
            result += textBox;
            result += "</div>";
            //Validation
            result += htmlHelper.ValidationMessage(metadata.PropertyName);

            return MvcHtmlString.Create(result); 
        }



        public static IHtmlString BizLookupLink(
          this HtmlHelper htmlHelper,
          string id,
          string targetField = null,
          string displayField = null,
          string dependentField = null,
          string format = null,
          string linkTitle = "Link", 
          string lookupTitle = null,
          string cssClass = null,
          string url = null,
          object htmlAttributes = null)
        {
            string result = "";

            //Script
            UrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            result += htmlHelper.Partial("_LookupScript", new ViewDataDictionary { 
                        { "Id", id }, 
                        { "Field", targetField }, 
                        { "DisplayField", displayField }, 
                        { "DependentField", dependentField }, 
                        { "Title", lookupTitle }, 
                        { "Url", urlHelper.Content(url) } });

            // Create link
            var linkTagBuilder = new TagBuilder("a");
            linkTagBuilder.MergeAttribute("href", "#");
            linkTagBuilder.Attributes.Add("id", "btn" + id);
            linkTagBuilder.MergeAttribute("data-id", id);
            linkTagBuilder.MergeAttribute("data-toggle", "modal");
            linkTagBuilder.MergeAttribute("data-target", "#bizLookup");
            linkTagBuilder.MergeAttribute("class", cssClass);
            linkTagBuilder.InnerHtml = linkTitle;
            linkTagBuilder.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            result += linkTagBuilder.ToString();
            
            return MvcHtmlString.Create(result);
        }
    }
}
