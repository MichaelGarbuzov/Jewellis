using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Jewellis.App_Custom.Validations
{
    /// <summary>
    /// Validates that the datetime is greater than the datetime of the specified property name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public sealed class DateTimeGreaterThanAttribute : ValidationAttribute, IClientModelValidator
    {
        public string LowerDatePropertyName { get; private set; }
        public bool AllowEqual { get; private set; }

        /// <summary>
        /// Validates that the datetime is greater than the datetime of the specified property name.
        /// </summary>
        /// <param name="lowerDate">The property name of the datetime that needs to be lower.</param>
        /// <param name="allowEqual">An indicator whether or not to allow equal datetimes.</param>
        public DateTimeGreaterThanAttribute(string lowerDate, bool allowEqual = false)
            : base($"{{0}} must be greater date then {lowerDate}.")
        {
            LowerDatePropertyName = lowerDate;
            AllowEqual = allowEqual;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            // Gets the date value of this property:
            DateTime thisDate = (DateTime)value;

            // Gets information about the lower date property specified:
            PropertyInfo lowerDatePropertyInfo = validationContext.ObjectType.GetProperty(LowerDatePropertyName);
            // Checks if the lower date property exists:
            if (lowerDatePropertyInfo == null)
                throw new ArgumentNullException($"The lower date property ({LowerDatePropertyName}) was not found.");

            // Gets the value of the lower date property:
            object lowerDateObj = lowerDatePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            if (lowerDateObj != null)
            {
                if (!(lowerDateObj is DateTime))
                    throw new ArgumentException($"The lower date property ({LowerDatePropertyName}) must be a DateTime.");

                // Gets the datetime value of the lower property:
                DateTime lowerDate = (DateTime)lowerDateObj;

                // Checks if this date is lower than the lower date:
                if ((thisDate < lowerDate) || (thisDate == lowerDate && !AllowEqual))
                {
                    // If so, the date is invalid:
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
            context.Attributes.Add("data-val-dtgt", ErrorMessage);
            context.Attributes.Add("data-val-dtgt-lower", LowerDatePropertyName);
            context.Attributes.Add("data-val-dtgt-equal", AllowEqual.ToString());
        }

    }
}
