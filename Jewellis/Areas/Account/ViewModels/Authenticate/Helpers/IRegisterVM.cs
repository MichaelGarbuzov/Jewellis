namespace Jewellis.Areas.Account.ViewModels.Authenticate.Helpers
{
    public interface IRegisterVM
    {
        /// <summary>
        /// The first name of the user.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the user.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// The email address of the user.
        /// </summary>
        /// <remarks>[Unique]</remarks>
        public string EmailAddress { get; set; }

        /// <summary>
        /// The password of the user.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The confirm password of the user.
        /// </summary>
        public string ConfirmPassword { get; set; }

        public bool SubscribeNewsletter { get; set; }
    }
}
