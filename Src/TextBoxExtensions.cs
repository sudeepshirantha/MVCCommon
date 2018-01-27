﻿using System.Linq.Expressions;

namespace System.Web.Mvc.Html
{
    /// <summary>
    /// MVC HtmlHelper extension methods - extensions that make use of jQuery Validates native unobtrusive data validation properties
    /// </summary>
    public static class TextBoxExtensions
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
        public static IHtmlString BizTextBoxFor<TModel, TProperty>(
          this HtmlHelper<TModel> htmlHelper,
          Expression<Func<TModel, TProperty>> expression,
          bool showLable = true,
          string icon = null,
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
