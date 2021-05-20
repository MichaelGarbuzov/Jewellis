namespace Jewellis.App_Custom.Services.ClientCurrency
{
    /// <summary>
    /// Represents the options to configure the <see cref="ClientCurrencyService"/>.
    /// </summary>
    public class ClientCurrencyOptions
    {

        /// <summary>
        /// Gets or sets the default currency code (<see cref="Currency.Code"/>).
        /// </summary>
        public string DefaultCurrency { get; set; }

        /// <summary>
        /// Gets or sets the list of supported client currencies in the application.
        /// </summary>
        public Currency[] SupportedCurrencies { get; set; }

    }
}
