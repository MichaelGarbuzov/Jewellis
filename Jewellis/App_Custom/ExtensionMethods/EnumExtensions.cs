using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Jewellis.App_Custom.ExtensionMethods
{
    /// <summary>
    /// Represents extension methods for enums.
    /// </summary>
    public static class EnumExtensions
    {

        /// <summary>
        /// Gets the display name (<see cref="DisplayAttribute"/>) for the enum value.
        /// </summary>
        /// <param name="enumValue">The enum value to get the display name.</param>
        /// <returns>Returns the display name (<see cref="DisplayAttribute"/>) for the enum value.</returns>
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()
                            .GetName();
        }

    }
}
