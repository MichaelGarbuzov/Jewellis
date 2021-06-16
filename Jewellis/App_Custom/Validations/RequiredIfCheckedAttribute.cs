using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Jewellis.App_Custom.Validations
{
    /// <summary>
    /// Validates that the property has a value in it only if the specified property is checked (or not checked).
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public sealed class RequiredIfCheckedAttribute : ValidationAttribute, IClientModelValidator
    {
        public string CheckPropertyName { get; private set; }
        public bool CheckState { get; private set; }

        /// <summary>
        /// Validates that the property has a value in it only if the specified property is checked (or not checked).
        /// </summary>
        /// <param name="checkPropertyName">A boolean property name that when checked (or not) makes this property required.</param>
        /// <param name="checkState">The check state to make this property required on, whether on checked or not checked.</param>
        public RequiredIfCheckedAttribute(string checkPropertyName, bool checkState = true)
            : base("{0} is required.")
        {
            CheckPropertyName = checkPropertyName;
            CheckState = checkState;
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

            // Checks if the check property equals the check state requested (in order to apply the required logic on this property):
            if (((bool)checkPropertyValue && CheckState) || (!(bool)checkPropertyValue && !CheckState))
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
            context.Attributes.Add("data-val-reqifchk", ErrorMessage);
            context.Attributes.Add("data-val-reqifchk-prop", CheckPropertyName);
            context.Attributes.Add("data-val-reqifchk-chk", CheckState.ToString().ToLower());
        }

    }
}
