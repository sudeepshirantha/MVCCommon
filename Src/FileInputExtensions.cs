using System.Linq.Expressions;
using ThirdParty.lib;

namespace System.Web.Mvc.Html
{
    /// <summary>
    /// MVC HtmlHelper extension methods - extensions that make use of jQuery Validates native unobtrusive data validation properties
    /// </summary>
    public static class FileInputExtensions
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
        public static IHtmlString BizFileInputFor<TModel, TProperty>(
          this HtmlHelper<TModel> htmlHelper,
          Expression<Func<TModel, TProperty>> expression,
          bool showLable = true,
          string accept = "image/gif, image/jpeg, image/png, application/pdf, application/word, application/excel",
          string format = null,
          object htmlAttributes = null)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var attributes = Mapper.GetUnobtrusiveValidationAttributes(htmlHelper, expression, htmlAttributes, metadata);

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

            //Input
            result += "<div class='input input-file'>";
            result += "   <span class=\"button\">";
            result += "      <input id=\"" + metadata.PropertyName + "_File\" type=\"file\" name=\"" + metadata.PropertyName + "_File\" value=\"" + metadata.Model + "\" onchange=\"document.getElementById('" + metadata.PropertyName + "').value = this.value\">Browse";
            result += "   </span> ";
            result += "   <input type=\"text\" id=\"" + metadata.PropertyName + "\" name=\"" + metadata.PropertyName + "\"  style=\"cursor:pointer;color:blue;text-decoration:underline;\"  value=\"" + metadata.Model + "\" onclick=\"openReport('" + Utility.GetBaseUrl() + "' + this.value)\">";
            result += "</div>";

            //Validation
            result += htmlHelper.ValidationMessage(metadata.PropertyName);

            return MvcHtmlString.Create(result); 
        }
    }
}
