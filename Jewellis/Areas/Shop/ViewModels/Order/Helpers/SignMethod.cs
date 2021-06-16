using System.ComponentModel.DataAnnotations;

namespace Jewellis.Areas.Shop.ViewModels.Order.Helpers
{
    /// <summary>
    /// Represents an enumeration of sign methods.
    /// </summary>
    public enum SignMethod
    {
        [Display(Name = "Login")]
        Login,

        [Display(Name = "Register")]
        Register
    }
}
