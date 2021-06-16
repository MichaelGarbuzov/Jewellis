namespace Jewellis.Areas.Account.ViewModels.Authenticate.Helpers
{
    public interface ILoginVM
    {

        /// <summary>
        /// The email address of the user.
        /// </summary>
        /// <remarks>[Unique]</remarks>
        public string EmailAddress { get; set; }

        /// <summary>
        /// The password of the user.
        /// </summary>
        public string Password { get; set; }

        public bool RememberMe { get; set; }

    }
}
