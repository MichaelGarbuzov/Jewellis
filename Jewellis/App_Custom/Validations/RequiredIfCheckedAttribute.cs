using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Jewellis.App_Custom.Validations
{
    /// <summary>
    /// Validates that the property has a value in it only if the specified property is checked.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public sealed class RequiredIfCheckedAttribute : ValidationAttribute, IClientModelValidator
    {
        public string CheckPropertyName { get; private set; }

        /// <summary>
        /// Validates that the has a value in it only if the specified property is checked.
        /// </summary>
        /// <param name="checkPropertyName">A boolean property name that when checked (true) makes this property required.</param>
        public RequiredIfCheckedAttribute(string checkPropertyName)
            : base("{0} is required.")
        {
            CheckPropertyName = checkPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Gets information about the property specified:
            PropertyInfo checkProperty = validationContext.ObjectType.GetProperty(CheckPropertyName);
            if (checkProperty == null)
                throw new ArgumentNullException($"The property ({CheckPropertyName}) was not found.");

            // Gets the value of the property:
            object checkPropertyValue = checkProperty.GetValue(validationContext.ObjectInstance, null);
            if (!(checkPropertyValue is bool))
                throw new ArgumentException($"The property ({CheckPropertyName}) must be a boolean.");

            // Checks if the check property is checked:
            if ((bool)checkPropertyValue)
            {
                // Enables the required validation - checks this property has a value:
                if (value is null || (value is string && string.IsNullOrEmpty(value as string)))
                {
                    string errorMessage = base.FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(errorMessage);
                }
            }
            return ValidationResult.Success;
        }

        // For client side validation:
        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-reqif", ErrorMessage);
            context.Attributes.Add("data-val-reqif-prop", CheckPropertyName);
        }

    }
}
