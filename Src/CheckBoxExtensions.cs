using System.Linq.Expressions;

namespace System.Web.Mvc.Html
{
    /// <summary>
    /// MVC HtmlHelper extension methods - extensions that make use of jQuery Validates native unobtrusive data validation properties
    /// </summary>
    public static class CheckBoxExtensions
    {
        /// <summary>
        /// Render a CheckBox for the supplied model using native jQuery Validate Unobtrusive extensions (only if true passed).
        /// It's arguable whether this is necessary since a checkbox implicitly has a true / false value
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="useNativeUnobtrusiveAttributes">Pass true if you want to use native extensions</param>
        /// <param name="htmlAttributes">OPTIONAL</param>
        /// <returns></returns>
        public static IHtmlString BizCheckBoxFor<TModel>(this HtmlHelper<TModel> htmlHelper,
          Expression<Func<TModel, bool>> expression,
          bool showLable = true,
          object htmlAttributes = null)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var attributes = Mapper.GetUnobtrusiveValidationAttributes(htmlHelper, expression, htmlAttributes, metadata);

            var checkBox = Mapper.GenerateHtmlWithoutMvcUnobtrusiveAttributes(() =>
                htmlHelper.CheckBoxFor(expression, attributes));

            //Input
            string result = "<label class='checkbox state-success'>";
            result += checkBox;
            if (showLable)
            {
                //Lable
                string lable = metadata.DisplayName ?? metadata.PropertyName;
                if (String.IsNullOrEmpty(lable))
                {
                    lable = String.Empty;
                }
                result += "<i data-swchon-text='ON' data-swchoff-text='OFF'></i>" + lable + "</label>";

            }
            else
            {
                result += "<i data-swchon-text='ON' data-swchoff-text='OFF'></i></label>";
            }
            //Validation
            result += htmlHelper.ValidationMessage(metadata.PropertyName);

            return MvcHtmlString.Create(result); 
        }


        public static IHtmlString BizToggleBoxFor<TModel>(this HtmlHelper<TModel> htmlHelper,
          Expression<Func<TModel, bool>> expression,
          bool showLable = true,
          object htmlAttributes = null)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var attributes = Mapper.GetUnobtrusiveValidationAttributes(htmlHelper, expression, htmlAttributes, metadata);

            var checkBox = Mapper.GenerateHtmlWithoutMvcUnobtrusiveAttributes(() =>
                htmlHelper.CheckBoxFor(expression, attributes));

            //Input
            string result = "<label class='toggle'>";
            result += checkBox;
            if (showLable)
            {
                //Lable
                string lable = metadata.DisplayName ?? metadata.PropertyName;
                if (String.IsNullOrEmpty(lable))
                {
                    lable = String.Empty;
                }
                result += "<i data-swchon-text='ON' data-swchoff-text='OFF'></i>" + lable + "</label>";

            }
            else
            {
                result += "<i data-swchon-text='ON' data-swchoff-text='OFF'></i></label>";
            }
            //Validation
            result += htmlHelper.ValidationMessage(metadata.PropertyName);

            return MvcHtmlString.Create(result);
        }

    }
}
