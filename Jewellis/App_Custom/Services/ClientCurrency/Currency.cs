using System;

namespace Jewellis.App_Custom.Services.ClientCurrency
{
    /// <summary>
    /// Represents a supported currency in the application.
    /// </summary>
    public class Currency
    {
        #region Properties

        /// <summary>
        /// Gets the code (3-digit ID) of the currency.
        /// </summary>
        public string Code { get; private set; }

        /// <summary>
        /// Gets the symbol of the currency.
        /// </summary>
        public char Symbol { get; private set; }

        #endregion

        /// <summary>
        /// Represents a supported currency in the application.
        /// </summary>
        /// <param name="code">The code (3-digit ID) of the currency.</param>
        /// <param name="symbol">The symbol of the currency.</param>
        public Currency(string code, char symbol)
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentNullException(nameof(code), $"{nameof(code)} cannot be null or empty.");

            this.Code = code;
            this.Symbol = symbol;
        }

        #region Public API

        public override string ToString()
        {
            return $"{this.Code} ({this.Symbol})";
        }

        #endregion

    }
}
