namespace Jewellis.Models.Helpers
{
    /// <summary>
    /// Represents the status of a contact (<see cref="Contact"/>).
    /// </summary>
    public enum ContactStatus
    {
        /// <summary>
        /// Marks a contact as pending (not handled by anyone).
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Marks a contact as being handled (someone is taking care of the contact).
        /// </summary>
        Handling = 1,

        /// <summary>
        /// Marks a contact as closed (contact is handled and finished).
        /// </summary>
        Closed = 2
    }
}
