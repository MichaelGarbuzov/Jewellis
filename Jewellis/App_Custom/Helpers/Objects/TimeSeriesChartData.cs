using Jewellis.App_Custom.Helpers.ViewModelHelpers;
using System;
using System.Collections.Generic;

namespace Jewellis.App_Custom.Helpers.Objects
{
    /// <summary>
    /// Represents a time series chart data.
    /// </summary>
    public class TimeSeriesChartData
    {
        #region Private Members

        private Periods _period;
        private DateTime _fromDateTime;
        private double[] _values;
        private int[] _counts;

        #endregion

        #region Properties

        public string[] Keys { get; private set; }

        public string[] Values { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSeriesChartData"/> class.
        /// Represents a time series chart data.
        /// </summary>
        /// <param name="period">The period this time series represents.</param>
        /// <param name="fromDateTime">The date and time the period starts.</param>
        public TimeSeriesChartData(Periods period, DateTime fromDateTime)
        {
            _period = period;
            _fromDateTime = fromDateTime;
            this.Initialize(period, fromDateTime);
        }

        #region Public API

        /// <summary>
        /// Adds the specified datetime and amount to the time series.
        /// </summary>
        /// <param name="dateTime">The date and time the amount occurred.</param>
        /// <param name="amount">The amount occurred in the date time.</param>
        public void Add(DateTime dateTime, double amount)
        {
            int index;
            if (_period == Periods.Today)
            {
                index = dateTime.Hour;
            }
            else if (_period == Periods.ThisMonth)
            {
                index = (dateTime.Day - 1);
            }
            else if (_period == Periods.ThisYear)
            {
                index = (dateTime.Month - 1);
            }
            else if (_period == Periods.Lifetime)
            {
                index = (dateTime.Year - _fromDateTime.Year);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(_period), $"{nameof(_period)} is not defined.");
            }

            _values[index] += amount;
            this.Values[index] = Math.Round(_values[index], 2).ToString();
            _counts[index]++;
        }

        /// <summary>
        /// Gets the average values in the <see cref="Values"/> array.
        /// </summary>
        /// <returns>Returns the average values in the <see cref="Values"/> array.</returns>
        public string[] GetAverageValues()
        {
            string[] result = new string[_values.Length];
            for (int i = 0; i < _values.Length; i++)
            {
                if (_counts[i] == 0)
                    result[i] = "0";
                else
                    result[i] = Math.Round(_values[i] / _counts[i], 2).ToString();
            }
            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes the time series with the proper keys according to the specified period.
        /// </summary>
        /// <param name="period">The period represents the time series.</param>
        /// <param name="fromDateTime">The date and time the period starts.</param>
        private void Initialize(Periods period, DateTime fromDateTime)
        {
            if (period == Periods.Today)
            {
                // Builds the labels (keys) by hours:
                List<string> keys = new List<string>();
                DateTime tmp = fromDateTime;
                for (int i = 0; i < 24; i++)
                {
                    keys.Add(tmp.ToString("t"));
                    tmp = tmp.AddHours(1);
                    if (tmp > DateTime.Now)
                        break;
                }
                this.Keys = keys.ToArray();
                this.Values = new string[keys.Count];
                _values = new double[keys.Count];

                for (int i = 0; i < keys.Count; i++)
                {
                    this.Values[i] = "0";
                }
            }
            else if (period == Periods.ThisMonth)
            {
                // Builds the labels (keys) by days:
                List<string> keys = new List<string>();
                DateTime tmp = fromDateTime;
                int daysInMonth = DateTime.DaysInMonth(fromDateTime.Year, fromDateTime.Month);
                for (int i = 0; i < daysInMonth; i++)
                {
                    keys.Add(tmp.ToString("MMM dd"));
                    tmp = tmp.AddDays(1);
                    if (tmp > DateTime.Now)
                        break;
                }
                this.Keys = keys.ToArray();
                this.Values = new string[keys.Count];
                _values = new double[keys.Count];

                for (int i = 0; i < keys.Count; i++)
                {
                    this.Values[i] = "0";
                }
            }
            else if (period == Periods.ThisYear)
            {
                // Builds the labels (keys) by months:
                List<string> keys = new List<string>();
                DateTime tmp = fromDateTime;
                for (int i = 0; i < 12; i++)
                {
                    keys.Add(tmp.ToString("MMM"));
                    tmp = tmp.AddMonths(1);
                    if (tmp > DateTime.Now)
                        break;
                }
                this.Keys = keys.ToArray();
                this.Values = new string[keys.Count];
                _values = new double[keys.Count];

                for (int i = 0; i < keys.Count; i++)
                {
                    this.Values[i] = "0";
                }
            }
            else if (period == Periods.Lifetime)
            {
                // Builds the labels (keys) by years:
                List<string> keys = new List<string>();
                DateTime tmp = fromDateTime;
                for (int i = tmp.Year; i <= DateTime.Now.Year; i++)
                {
                    keys.Add(tmp.ToString("yyyy"));
                    tmp = tmp.AddYears(1);
                }
                this.Keys = keys.ToArray();
                this.Values = new string[keys.Count];
                _values = new double[keys.Count];

                for (int i = 0; i < keys.Count; i++)
                {
                    this.Values[i] = "0";
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(period), $"{nameof(period)} is not defined.");
            }

            _counts = new int[this.Keys.Length];
        }

        #endregion

    }
}
