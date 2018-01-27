using System.Linq.Expressions;

namespace System.Web.Mvc.Html
{
    /// <summary>
    /// MVC HtmlHelper extension methods - extensions that make use of jQuery Validates native unobtrusive data validation properties
    /// </summary>
    public static class ImageInputExtensions
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
        public static IHtmlString BizImageInputFor<TModel, TProperty>(
          this HtmlHelper<TModel> htmlHelper,
          Expression<Func<TModel, TProperty>> expression,
          bool showLable = true,
          int imageWidth = 200,
          int imageHeight = 200,
          string accept = "image/gif, image/jpeg, image/png, application/pdf",
          string format = null,
          string title = null,
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

            //Script
            UrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            result += htmlHelper.Partial("_ImageUploadScript", new ViewDataDictionary { 
                        { "Field", htmlHelper.IdFor(expression).ToString() }, 
                        { "Title", title }});


            //Input
            result += "<div style=\"height:" + imageHeight + "px;width:" + imageWidth + "px; margin-bottom:50px\">";
            result += "    <img id=\"" + htmlHelper.IdFor(expression).ToString() + "Preview\" src=\"" + metadata.Model + "\" style=\"height:" + imageHeight + "px;width:" + imageWidth + "px;border: 1px solid #e3e3e3;\"/>";
            result += "    <button type=\"button\" style=\"margin:1px\" class=\"btn btn-primary btn-sm btn-block actionUpload\">";
            result += "        Upload";
            result += "    </button>";
            result += "</div>";
            result += "<input type=\"hidden\" name=\"" + htmlHelper.NameFor(expression).ToString() + "\" id=\"" + htmlHelper.IdFor(expression).ToString() + "\" value=\"" + metadata.Model + "\" />";

            //Validation
            result += htmlHelper.ValidationMessage(metadata.PropertyName);

            return MvcHtmlString.Create(result); 
        }
    }
}
