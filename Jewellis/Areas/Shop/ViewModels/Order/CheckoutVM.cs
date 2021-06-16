using Jewellis.App_Custom.Validations;
using Jewellis.Areas.Account.ViewModels.Authenticate.Helpers;
using Jewellis.Areas.Shop.ViewModels.Order.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Jewellis.Areas.Shop.ViewModels.Order
{
    public class CheckoutVM
    {
        #region Account Step

        public SignMethod? SignMethod { get; set; }

        public ConditionalLoginVM LoginVM { get; set; }

        public ConditionalRegisterVM RegisterVM { get; set; }

        #endregion

        #region Address Step

        public BillingDetailsModel BillingDetails { get; set; }

        public ShippingDetailsModel ShippingDetails { get; set; }

        #endregion

        #region Delivery Step

        [HiddenInput]
        [Required(ErrorMessage = "Delivery method is required.")]
        public int? DeliveryMethodId { get; set; }

        #endregion

        #region Payment Step

        public CreditCardInfo CreditCard { get; set; }

        #endregion

        #region Review Step

        [Display(Prompt = "Note (Optional)")]
        [StringLength(100, ErrorMessage = "Maximum length is 100 characters.")]
        public string Note { get; set; }

        #endregion

        #region Inner Classes

        public class ConditionalLoginVM : ILoginVM
        {
            public SignMethod SignMethod { get; set; }

            /// <summary>
            /// The email address of the user.
            /// </summary>
            /// <remarks>[Unique]</remarks>
            [Display(Name = "Email Address", Prompt = "Email Address")]
            [RequiredIf(nameof(SignMethod), SignMethod.Login, ErrorMessage = "Email address is required.")]
            [StringLength(50, ErrorMessage = "Maximum length is 50 characters.")]
            [DataType(DataType.EmailAddress, ErrorMessage = "Invalid email address.")]
            public string EmailAddress { get; set; }

            /// <summary>
            /// The password of the user.
            /// </summary>
            [Display(Name = "Password", Prompt = "Password")]
            [RequiredIf(nameof(SignMethod), SignMethod.Login, ErrorMessage = "Password is required.")]
            [StringLength(50, ErrorMessage = "Maximum length is 50 characters.")]
            [RegularExpression(@"^.{6,}$", ErrorMessage = "Minimum length is 6 characters.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember Me")]
            public bool RememberMe { get; set; }
        }

        public class ConditionalRegisterVM : IRegisterVM
        {
            public SignMethod SignMethod { get; set; }

            /// <summary>
            /// The first name of the user.
            /// </summary>
            [Display(Name = "First Name", Prompt = "First Name")]
            [RequiredIf(nameof(SignMethod), SignMethod.Register, ErrorMessage = "First name is required.")]
            [StringLength(50, ErrorMessage = "Maximum length allowed is 50 characters.")]
            public string FirstName { get; set; }

            /// <summary>
            /// The last name of the user.
            /// </summary>
            [Display(Name = "Last Name", Prompt = "Last Name")]
            [RequiredIf(nameof(SignMethod), SignMethod.Register, ErrorMessage = "Last name is required.")]
            [StringLength(50, ErrorMessage = "Maximum length is 50 characters.")]
            public string LastName { get; set; }

            /// <summary>
            /// The email address of the user.
            /// </summary>
            /// <remarks>[Unique]</remarks>
            [Display(Name = "Email Address", Prompt = "Email Address")]
            [RequiredIf(nameof(SignMethod), SignMethod.Register, ErrorMessage = "Email address is required.")]
            [StringLength(50, ErrorMessage = "Maximum length is 50 characters.")]
            [DataType(DataType.EmailAddress, ErrorMessage = "Invalid email address.")]
            [Remote("CheckEmailAvailability", "Order", "Shop", ErrorMessage = "Email is already registered.")]
            public string EmailAddress { get; set; }

            /// <summary>
            /// The password of the user.
            /// </summary>
            [Display(Name = "Password", Prompt = "Password")]
            [RequiredIf(nameof(SignMethod), SignMethod.Register, ErrorMessage = "Password is required.")]
            [StringLength(50, ErrorMessage = "Maximum length is 50 characters.")]
            [RegularExpression(@"^.{6,}$", ErrorMessage = "Minimum length is 6 characters.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            /// The confirm password of the user.
            /// </summary>
            [Display(Name = "Confirm Password", Prompt = "Confirm Password")]
            [RequiredIf(nameof(SignMethod), SignMethod.Register, ErrorMessage = "Confirm password is required.")]
            [StringLength(50, ErrorMessage = "Maximum length is 50 characters.")]
            [RegularExpression(@"^.{6,}$", ErrorMessage = "Minimum length is 6 characters.")]
            [DataType(DataType.Password)]
            [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; }

            [Display(Name = "Subscribe our newsletter")]
            public bool SubscribeNewsletter { get; set; }
        }

        public class BillingDetailsModel
        {
            [Display(Prompt = "Name *")]
            [Required(ErrorMessage = "Name is required.")]
            [StringLength(50, ErrorMessage = "Maximum length is 50 characters.")]
            public string Name { get; set; }

            [Display(Prompt = "Phone *")]
            [Required(ErrorMessage = "Phone is required.")]
            [StringLength(20, ErrorMessage = "Maximum length is 20 characters.")]
            [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid phone number.")]
            [DataType(DataType.PhoneNumber, ErrorMessage = "Invalid phone number.")]
            public string Phone { get; set; }

            /// <summary>
            /// The street of the address.
            /// </summary>
            [Display(Prompt = "Street *")]
            [Required(ErrorMessage = "Street is required.")]
            [StringLength(50, ErrorMessage = "Maximum length is 50 characters.")]
            public string Street { get; set; }

            /// <summary>
            /// The postal code of the address.
            /// </summary>
            [Display(Prompt = "Postal Code *")]
            [Required(ErrorMessage = "Postal code is required.")]
            [StringLength(30, ErrorMessage = "Maximum length is 30 characters.")]
            [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Invalid postal code.")]
            [DataType(DataType.PostalCode, ErrorMessage = "Invalid postal code.")]
            public string PostalCode { get; set; }

            /// <summary>
            /// The city of the address.
            /// </summary>
            [Display(Prompt = "City *")]
            [Required(ErrorMessage = "City is required.")]
            [StringLength(30, ErrorMessage = "Maximum length is 30 characters.")]
            public string City { get; set; }

            /// <summary>
            /// The country of the address.
            /// </summary>
            [Display(Prompt = "Country *")]
            [Required(ErrorMessage = "Country is required.")]
            [StringLength(30, ErrorMessage = "Maximum length is 30 characters.")]
            public string Country { get; set; }
        }

        public class ShippingDetailsModel
        {
            [Display(Name = "Same as Billing Details")]
            public bool ShippingSameAsBilling { get; set; }

            [Display(Prompt = "Name *")]
            [RequiredIfChecked(nameof(ShippingSameAsBilling), false, ErrorMessage = "Name is required.")]
            [StringLength(50, ErrorMessage = "Maximum length is 50 characters.")]
            public string Name { get; set; }

            [Display(Prompt = "Phone *")]
            [RequiredIfChecked(nameof(ShippingSameAsBilling), false, ErrorMessage = "Phone is required.")]
            [StringLength(20, ErrorMessage = "Maximum length is 20 characters.")]
            [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid phone number.")]
            [DataType(DataType.PhoneNumber, ErrorMessage = "Invalid phone number.")]
            public string Phone { get; set; }

            /// <summary>
            /// The street of the address.
            /// </summary>
            [Display(Prompt = "Street *")]
            [RequiredIfChecked(nameof(ShippingSameAsBilling), false, ErrorMessage = "Street is required.")]
            [StringLength(50, ErrorMessage = "Maximum length is 50 characters.")]
            public string Street { get; set; }

            /// <summary>
            /// The postal code of the address.
            /// </summary>
            [Display(Prompt = "Postal Code *")]
            [RequiredIfChecked(nameof(ShippingSameAsBilling), false, ErrorMessage = "Postal code is required.")]
            [StringLength(30, ErrorMessage = "Maximum length is 30 characters.")]
            [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Invalid postal code.")]
            [DataType(DataType.PostalCode, ErrorMessage = "Invalid postal code.")]
            public string PostalCode { get; set; }

            /// <summary>
            /// The city of the address.
            /// </summary>
            [Display(Prompt = "City *")]
            [RequiredIfChecked(nameof(ShippingSameAsBilling), false, ErrorMessage = "City is required.")]
            [StringLength(30, ErrorMessage = "Maximum length is 30 characters.")]
            public string City { get; set; }

            /// <summary>
            /// The country of the address.
            /// </summary>
            [Display(Prompt = "Country *")]
            [RequiredIfChecked(nameof(ShippingSameAsBilling), false, ErrorMessage = "Country is required.")]
            [StringLength(30, ErrorMessage = "Maximum length is 30 characters.")]
            public string Country { get; set; }
        }

        public class CreditCardInfo
        {
            [Display(Prompt = "Card Number *")]
            [Required(ErrorMessage = "Card number is required.")]
            public string CardNumber { get; set; }

            [Display(Prompt = "Expiry Date *")]
            [Required(ErrorMessage = "Expiry date is required.")]
            [RegularExpression(@"^(0[1-9]|1[0-2])\/?([0-9]{4}|[0-9]{2})$", ErrorMessage = "Accepted format: month/year (MM/yy)")]
            public string CardExpiryDate { get; set; }

            [Display(Prompt = "Security Code *")]
            [Required(ErrorMessage = "Security code is required.")]
            [RegularExpression(@"^([0-9]{3,4})$", ErrorMessage = "Invalid security code.")]
            public string CardSecurityCode { get; set; }

            [Display(Prompt = "Card Holder Name *")]
            [Required(ErrorMessage = "Card holder name is required.")]
            [FullName(ErrorMessage = "Must be a full name.")]
            public string CardHolderName { get; set; }
        }

        #endregion

    }
}
