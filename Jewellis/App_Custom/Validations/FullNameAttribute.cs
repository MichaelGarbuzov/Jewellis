using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.App_Custom.Validations
{
    /// <summary>
    /// Validates that the property value has a full name in it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class FullNameAttribute : ValidationAttribute, IClientModelValidator
    {

        /// <summary>
        /// Validates that the property value has a full name in it.
        /// </summary>
        public FullNameAttribute()
            : base("{0} must be a full name.")
        { }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string strValue = (value as string);
            if (strValue != null)
            {
                // Checks if the full name not contains a space between the words (after trimmed):
                if (!strValue.Trim().Contains(" "))
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
            context.Attributes.Add("data-val-fullname", ErrorMessage);
        }

    }
}
