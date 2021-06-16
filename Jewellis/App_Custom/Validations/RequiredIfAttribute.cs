using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Jewellis.App_Custom.Validations
{
    /// <summary>
    /// Validates that the property has a value in it only if the specified property has the specified value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public sealed class RequiredIfAttribute : ValidationAttribute, IClientModelValidator
    {
        public string PropertyName { get; private set; }
        public object RequiredOnValue { get; private set; }

        /// <summary>
        /// Validates that the property has a value in it only if the specified property has the specified value.
        /// </summary>
        /// <param name="propertyName">A property name that when its value equals to the specified value - this property required.</param>
        /// <param name="value">The value to make this property required on.</param>
        public RequiredIfAttribute(string propertyName, object value)
            : base("{0} is required.")
        {
            PropertyName = propertyName;
            RequiredOnValue = value;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Gets information about the property specified:
            PropertyInfo otherProperty = validationContext.ObjectType.GetProperty(PropertyName);
            if (otherProperty == null)
                throw new ArgumentNullException($"The property ({PropertyName}) was not found.");

            // Gets the value of the other property:
            object otherPropertyValue = otherProperty.GetValue(validationContext.ObjectInstance, null);

            // Checks if the check property equals the check state requested (in order to apply the required logic on this property):
            if (object.Equals(otherPropertyValue, RequiredOnValue))
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
            context.Attributes.Add("data-val-reqif-prop", PropertyName);
            context.Attributes.Add("data-val-reqif-val", RequiredOnValue.ToString());
        }

    }
}
