using System.Drawing;

namespace Jewellis
{
    /// <summary>
    /// Represents extension methods for the <see cref="Color"/> class.
    /// </summary>
    public static class ColorExtensions
    {

        #region Public Static API

        /// <summary>
        /// Returns a HEX string representation of the color.
        /// </summary>
        /// <param name="c">The <see cref="Color"/> to extend.</param>
        /// <returns>Returns a HEX string representation of the color.</returns>
        public static string ToHexString(this Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        /// <summary>
        /// Returns an RGB string representation of the color.
        /// </summary>
        /// <param name="c">The <see cref="Color"/> to extend.</param>
        /// <returns>Returns an RGB string representation of the color.</returns>
        public static string ToRgbString(this Color c)
        {
            return "rgb(" + c.R.ToString() + "," + c.G.ToString() + "," + c.B.ToString() + ")";
        }

        /// <summary>
        /// Lerps the color to the specified color and amount.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="toColor">The color to lerp with.</param>
        /// <param name="amount">The amount to lerp.</param>
        /// <returns>Returns the lerped color.</returns>
        public static Color Lerp(this Color color, Color toColor, float amount)
        {
            // Start colors as lerp-able floats:
            float sr = color.R, sg = color.G, sb = color.B;

            // End colors as lerp-able floats:
            float er = toColor.R, eg = toColor.G, eb = toColor.B;

            // Lerps the colours to get the difference
            byte r = (byte)sr.Lerp(er, amount), g = (byte)sg.Lerp(eg, amount), b = (byte)sb.Lerp(eb, amount);

            // Returns the new color:
            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Returns the color by the specified color hex value.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="colorHex">The color in hex.</param>
        /// <returns>Returns the color by the specified color hex value.</returns>
        public static Color FromHex(this Color color, string colorHex)
        {
            return ColorTranslator.FromHtml(colorHex);
        }

        #endregion

        #region Private Static Methods

        private static float Lerp(this float start, float end, float amount)
        {
            float difference = end - start;
            float adjusted = difference * amount;
            return start + adjusted;
        }

        #endregion

    }
}