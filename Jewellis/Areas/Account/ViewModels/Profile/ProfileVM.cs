using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Areas.Account.ViewModels.Profile
{
    public class ProfileVM
    {

        public EditProfileSubVM EditProfileVM { get; set; }
        public EditPasswordSubVM EditPasswordVM { get; set; }
        public EditPreferencesSubVM EditPreferencesVM { get; set; }

        #region Inner View Models

        public class EditProfileSubVM
        {
            /// <summary>
            /// The first name of the user.
            /// </summary>
            [Display(Name = "First Name *")]
            [Required(ErrorMessage = "First name is required.")]
            [StringLength(50, ErrorMessage = "Maximum length allowed is 50 characters.")]
            public string FirstName { get; set; }

            /// <summary>
            /// The last name of the user.
            /// </summary>
            [Display(Name = "Last Name *")]
            [Required(ErrorMessage = "Last name is required.")]
            [StringLength(50, ErrorMessage = "Maximum length is 50 characters.")]
            public string LastName { get; set; }

            /// <summary>
            /// The email address of the user.
            /// </summary>
            /// <remarks>[Unique]</remarks>
            [Display(Name = "Email Address *")]
            [Required(ErrorMessage = "Email address is required.")]
            [StringLength(50, ErrorMessage = "Maximum length is 50 characters.")]
            [DataType(DataType.EmailAddress, ErrorMessage = "Invalid email address.")]
            [Remote("CheckEmailEditAvailability", "Profile", "Account", AdditionalFields = nameof(CurrentEmail), ErrorMessage = "Email is already registered.")]
            public string EmailAddress { get; set; }

            [HiddenInput]
            public string CurrentEmail { get; set; }

            /// <summary>
            /// The phone number of the user.
            /// </summary>
            [Display(Name = "Phone Number")]
            [StringLength(20, ErrorMessage = "Maximum length is 20 characters.")]
            [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid phone number.")]
            [DataType(DataType.PhoneNumber, ErrorMessage = "Invalid phone number.")]
            public string PhoneNumber { get; set; }
        }

        public class EditPasswordSubVM
        {
            [Display(Name = "Current Password")]
            [Required(ErrorMessage = "Current password is required.")]
            [StringLength(50, ErrorMessage = "Maximum length is 50 characters.")]
            [RegularExpression(@"^.{6,}$", ErrorMessage = "Minimum length is 6 characters.")]
            [DataType(DataType.Password)]
            public string CurrentPassword { get; set; }

            [Display(Name = "New Password")]
            [Required(ErrorMessage = "New password is required.")]
            [StringLength(50, ErrorMessage = "Maximum length is 50 characters.")]
            [RegularExpression(@"^.{6,}$", ErrorMessage = "Minimum length is 6 characters.")]
            [DataType(DataType.Password)]
            public string NewPassword { get; set; }

            [Display(Name = "Confirm Password")]
            [Required(ErrorMessage = "Confirm password is required.")]
            [StringLength(50, ErrorMessage = "Maximum length is 50 characters.")]
            [RegularExpression(@"^.{6,}$", ErrorMessage = "Minimum length is 6 characters.")]
            [DataType(DataType.Password)]
            [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
            public string NewPasswordConfirm { get; set; }
        }

        public class EditPreferencesSubVM
        {
            [Display(Name = "Currency")]
            [Required(ErrorMessage = "Currency is required.")]
            public string Currency { get; set; }

            [Display(Name = "Theme")]
            [Required(ErrorMessage = "Theme is required.")]
            public string Theme { get; set; }
        }

        #endregion

    }
}
