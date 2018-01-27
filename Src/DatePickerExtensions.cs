using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

namespace System.Web.Mvc.Html
{
    /// <summary>
    /// MVC HtmlHelper extension methods - extensions that make use of jQuery Validates native unobtrusive data validation properties
    /// </summary>
    public static class DatePickerExtensions
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
        public static IHtmlString BizDatePickerFor<TModel, TProperty>(
          this HtmlHelper<TModel> htmlHelper,
          Expression<Func<TModel, TProperty>> expression,
          bool showLable = true,
          object htmlAttributes = null,
          string format = null)
        {
            Dictionary<string, object> attrs = new Dictionary<string, object>();

            if (htmlAttributes != null)
            {
                foreach (var prop in htmlAttributes.GetType().GetProperties())
                {
                    attrs[prop.Name] = prop.GetValue(htmlAttributes, null);
                }
            }
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            attrs["class"] = "date-picker";
            attrs["Value"] = String.Format("{0:yyyy-MM-dd}", (DateTime)metadata.Model);
            string icon = "fa-calendar";
            
            var attributes = Mapper.GetUnobtrusiveValidationAttributes(htmlHelper, expression, attrs, metadata);
                                    
            var textBox = Mapper.GenerateHtmlWithoutMvcUnobtrusiveAttributes(() => 
                htmlHelper.TextBoxFor(expression, format, attributes));

            string result = "";
            if (showLable)
            {
                //Lable
                result += metadata.DisplayName ?? metadata.PropertyName;
                if (String.IsNullOrEmpty(result))
                {
                    result += String.Empty;
                }
            }

            var builder = new TagBuilder("script");
            builder.MergeAttribute("type", "text/javascript");
            builder.InnerHtml = 
                "$(function () { " +
                "    $('#" + htmlHelper.IdFor(expression).ToString() + "').datepicker({ dateFormat: 'yy-mm-dd' });" +
                "});";
            result += MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));

            //Input
            result += "<label class='input'>";
            if (icon != null)
            {
                result += "<i class='icon-append fa " + icon + "'></i>";
            }

            result += textBox;
            result += "</label>";
            //Validation
            result += htmlHelper.ValidationMessage(metadata.PropertyName);

            return MvcHtmlString.Create(result); 
        }

    }
}
