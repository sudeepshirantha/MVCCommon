﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Routing;
using Newtonsoft.Json;

namespace System.Web.Mvc.Html
{
    /// <summary>
    /// MVC HtmlHelper extension methods - extensions that make use of jQuery Validates native unobtrusive data validation properties
    /// </summary>
    public static class Mapper
    {
        public static RouteValueDictionary GetUnobtrusiveValidationAttributes<TModel, TProperty>(
            HtmlHelper<TModel> helper,
            Expression<Func<TModel, TProperty>> expression,
            object htmlAttributes,
            ModelMetadata metadata)
        {
            RouteValueDictionary attributes = null;
            
            // prevent double conversion
            if (htmlAttributes is RouteValueDictionary)
                attributes = (RouteValueDictionary)htmlAttributes;
            else if (htmlAttributes is IDictionary<string, object>)
                attributes = new RouteValueDictionary((IDictionary<string, object>)htmlAttributes);
            else
                attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            // If client validation or unobtrusive javascript are disabled then there's really nothing to do
            if (!HtmlHelper.ClientValidationEnabled || !HtmlHelper.UnobtrusiveJavaScriptEnabled)
                return attributes;

            // get validators so we can get the contained ModelClientValidationRules
            var validators = ModelValidatorProviders.Providers.GetValidators(metadata, helper.ViewContext);
            foreach (var rule in validators.SelectMany(v => v.GetClientValidationRules()))
            {
                // if we have an error message for this rule then create a data-msg-/rulename/ attribute to store it
                if (!string.IsNullOrEmpty(rule.ErrorMessage) && rule.ValidationType != "length" && rule.ValidationType != "regex") // length & regex are exceptions to this mapping
                    attributes.AddIfNotPresent("data-msg-" + rule.ValidationType, rule.ErrorMessage);

                switch (rule.ValidationType)
                {
                    // Exceptions to the standard handling of rules

                    case "equalto":
                        // Convert equalto selector from "*.PasswordDemo" style to "#PasswordDemo" style
                        var equalToSelector = "#" + rule.ValidationParameters["other"].ToString().Substring(2);
                        attributes.AddIfNotPresent("data-rule-equalto", equalToSelector);
                        break;

                    case "minlength":
                    case "maxlength":
                    case "length":
                        // length is mapped to minlength and maxlength
                        if (rule.ValidationParameters.ContainsKey("min"))
                        {
                            attributes.AddIfNotPresent("data-rule-minlength", rule.ValidationParameters["min"]);
                            attributes.AddIfNotPresent("data-msg-minlength", rule.ErrorMessage);
                        }

                        if (rule.ValidationParameters.ContainsKey("max"))
                        {
                            attributes.AddIfNotPresent("data-rule-maxlength", rule.ValidationParameters["max"]);
                            attributes.AddIfNotPresent("data-msg-maxlength", rule.ErrorMessage);
                        }
                        break;

                    case "range":
                        attributes.AddIfNotPresent("data-rule-range", string.Format(CultureInfo.InvariantCulture, // Invariant so JavaScript can parse the numbers
                            "[{0},{1}]", rule.ValidationParameters["min"], rule.ValidationParameters["max"]));
                        break;

                    case "extension":
                        // Use the older "extensions" validator in Additional-Methods, and only spit out a string of extensions.
                        attributes.AddIfNotPresent("data-rule-extension", rule.ValidationParameters["extension"]);
                        break;

                    case "regex":
                        // Additional-Methods provides support for "pattern" with the regex passed in as a string.
                        attributes.AddIfNotPresent("data-rule-pattern", rule.ValidationParameters["pattern"]);
                        attributes.AddIfNotPresent("data-msg-pattern", rule.ErrorMessage);
                        break;

                    // Standard handling of rules

                    default:

                        // if we have *no* validation parameters then create a data-rule-/rulename/ attribute with the value "true"
                        // if we have validation parameters then create a data-rule-/rulename/ attribute with the JSON'd ValidationParameters
                        attributes.AddIfNotPresent("data-rule-" + rule.ValidationType,
                            rule.ValidationParameters.Count == 0
                                ? "true"
                                : JsonConvert.SerializeObject(rule.ValidationParameters));
                        break;
                }
            }

            return attributes;
        }

        private static void AddIfNotPresent(this IDictionary<string, object> attributes, string attrKey, object attrValue)
        {
            if (!attributes.ContainsKey(attrKey)) 
                attributes.Add(attrKey, attrValue);
        }

        public static IHtmlString GenerateHtmlWithoutMvcUnobtrusiveAttributes(Func<IHtmlString> generator)
        {
            var cachedClientValidationEnabled = HtmlHelper.ClientValidationEnabled;
            var cachedUnobtrusiveJavaScriptEnabled = HtmlHelper.UnobtrusiveJavaScriptEnabled;

            HtmlHelper.ClientValidationEnabled = false;
            HtmlHelper.UnobtrusiveJavaScriptEnabled = false;

            var html = generator();

            HtmlHelper.ClientValidationEnabled = cachedClientValidationEnabled;
            HtmlHelper.UnobtrusiveJavaScriptEnabled = cachedUnobtrusiveJavaScriptEnabled;

            return html;
        }
    }
}
