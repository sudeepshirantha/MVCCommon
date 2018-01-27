﻿using System.Linq.Expressions;

namespace System.Web.Mvc.Html
{
    /// <summary>
    /// MVC HtmlHelper extension methods - extensions that make use of jQuery Validates native unobtrusive data validation properties
    /// </summary>
    public static class EnumDropDownListExtensions
    {
        /// <summary>
        /// Render a DropDownList for the supplied model using native jQuery Validate Unobtrusive extensions (only if true passed)
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="useNativeUnobtrusiveAttributes"></param>
        /// <param name="optionLabel">OPTIONAL</param>
        /// <param name="htmlAttributes">OPTIONAL</param>
        /// <returns></returns>
        public static IHtmlString BizEnumDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            bool showLable = true,
            string icon = null,
            string optionLabel = null,
            object htmlAttributes = null)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var attributes = Mapper.GetUnobtrusiveValidationAttributes(htmlHelper, expression, htmlAttributes, metadata);

            var dropDown = Mapper.GenerateHtmlWithoutMvcUnobtrusiveAttributes(() =>
                htmlHelper.EnumDropDownListFor(expression, optionLabel, attributes));

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
            result += "<label class='select'>";
            if (icon != null)
            {
                result += "<i class='icon-append fa " + icon + "'></i>";
            }

            result += dropDown;
            result += "<i></i></label>";
            //Validation
            result += htmlHelper.ValidationMessage(metadata.PropertyName);

            return MvcHtmlString.Create(result); 
        }
    }
}
