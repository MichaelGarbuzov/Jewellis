using System;
using System.Collections.Generic;

namespace Jewellis.App_Custom.Helpers.ViewModelHelpers
{
    /// <summary>
    /// Represents a pagination helper.
    /// </summary>
    public class Pagination
    {
        #region Properties

        /// <summary>
        /// Gets the total records in the pagination.
        /// </summary>
        public int TotalRecords { get; private set; }

        /// <summary>
        /// Gets the number of records to show in a page.
        /// </summary>
        public int? PageSize { get; private set; }

        /// <summary>
        /// Gets the current page displaying in the pagination.
        /// </summary>
        /// <remarks>Count starts from 1.</remarks>
        public int CurrentPage { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Pagination"/> class.
        /// Represents a pagination helper.
        /// </summary>
        /// <param name="totalRecords">The total records in the pagination.</param>
        /// <param name="pageSize">The number of records to show in a page.</param>
        /// <param name="currentPage">The current page displaying in the pagination.</param>
        public Pagination(int totalRecords, int? pageSize, int currentPage)
        {
            this.TotalRecords = totalRecords;
            this.PageSize = pageSize;
            this.CurrentPage = this.ValidatePage(currentPage, pageSize, totalRecords);
        }

        #region Public API

        /// <summary>
        /// Checks whether this pagination contains records and can be displayed.
        /// </summary>
        /// <returns>Returns true if the pagination contains records and can be displayed, otherwise false.</returns>
        public bool HasPagination()
        {
            if (this.PageSize.HasValue && this.TotalRecords > this.PageSize.Value)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Gets the first record index in the current page.
        /// </summary>
        /// <remarks>Calculated by the <see cref="TotalRecords"/>, <see cref="CurrentPage"/>, and <see cref="PageSize"/> properties.</remarks>
        /// <returns>Returns the first record index in the current page.</returns>
        public int GetFirstRecordIndexInPage()
        {
            if (this.TotalRecords < 1)
                return 0;
            else if (this.PageSize == null)
                return 1;
            else
                return (((this.PageSize.Value * this.CurrentPage) - this.PageSize.Value) + 1);
        }

        /// <summary>
        /// Gets the last record index in the current page.
        /// </summary>
        /// <remarks>Calculated by the <see cref="TotalRecords"/>, <see cref="CurrentPage"/>, and <see cref="PageSize"/> properties.</remarks>
        /// <returns>Returns the last record index in the current page.</returns>
        public int GetLastRecordIndexInPage()
        {
            if (this.PageSize == null)
            {
                return this.TotalRecords;
            }
            else
            {
                int expectedLast = (this.PageSize.Value * this.CurrentPage);

                // Checks if the expected result is bigger than the actual total number of records:
                if (expectedLast > this.TotalRecords)
                    return this.TotalRecords;
                else
                    return expectedLast;
            }
        }

        /// <summary>
        /// Gets the index of the first page in the pagination.
        /// </summary>
        /// <returns>Returns the index of the first page in the pagination.</returns>
        public int GetFirstPage()
        {
            if (this.TotalRecords < 1)
                return 0;
            else
                return 1;
        }

        /// <summary>
        /// Gets the index of the last page in the pagination.
        /// </summary>
        /// <returns>Returns the index of the last page in the pagination.</returns>
        public int GetLastPage()
        {
            if (this.PageSize == null)
                return this.GetFirstPage();
            else
                return (int)Math.Ceiling((double)this.TotalRecords / this.PageSize.Value);
        }

        /// <summary>
        /// Checks whether the current page has a previous page or not.
        /// </summary>
        /// <returns>Returns true if the current page has a previous page, otherwise false.</returns>
        public bool HasPrevious()
        {
            if (this.PageSize == null || this.CurrentPage < 2)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Checks whether the current page has a next page or not.
        /// </summary>
        /// <returns>Returns true if the current page has a next page, otherwise false.</returns>
        public bool HasNext()
        {
            if (this.TotalRecords < 1 || this.PageSize == null || this.CurrentPage == this.GetLastPage())
                return false;
            else
                return true;
        }

        /// <summary>
        /// Gets the index of the previous page in the pagination. If there's no previous page then returns "-1".
        /// </summary>
        /// <returns>Returns the index of the previous page in the pagination. If there's no previous page then returns "-1".</returns>
        public int GetPreviousPage()
        {
            if (this.HasPrevious())
                return (this.CurrentPage - 1);
            else
                return -1;
        }

        /// <summary>
        /// Gets the index of the next page in the pagination. If there's no next page then returns "-1".
        /// </summary>
        /// <returns>Returns the number of the next page in the pagination, or if there's no next page then returns "-1".</returns>
        public int GetNextPage()
        {
            if (this.HasNext())
                return (this.CurrentPage + 1);
            else
                return -1;
        }

        /// <summary>
        /// Gets the number of records skipped to this current page (records that are shown in previous pages).
        /// </summary>
        /// <returns>Gets the number of records skipped to this current page.</returns>
        public int GetRecordsSkipped()
        {
            if (this.PageSize == null)
                return 0;
            else
                return this.PageSize.Value * (this.CurrentPage - 1);
        }

        /// <summary>
        /// Gets the list of pages to display, while keeping a maximum of 5 pages in the list, in order to make an easy, short, and efficient display.
        /// </summary>
        /// <returns>Returns an array of page numbers to display.</returns>
        public int[] GetPageList()
        {
            List<int> list = new List<int>();

            int maxPage = this.GetLastPage();

            // Checks if currently at the first 3 pages:
            if (this.CurrentPage < 3)
            {
                for (int i = 1; i <= 5; i++)
                {
                    // Makes sure we don't pass the last page:
                    if (i > maxPage)
                        return list.ToArray();

                    list.Add(i);
                }
            }
            // Checks if currently at the last 3 pages:
            else if ((maxPage - this.CurrentPage) < 3)
            {
                for (int i = -4; i < 1; i++)
                {
                    // Makes sure we have a minimum 1 page:
                    if ((maxPage + i) < 1)
                        continue;

                    list.Add(maxPage + i);
                }
            }
            // Here, makes sure the current page is in the middle (because we're not at the first/last 3 pages):
            else
            {
                for (int i = -2; i < 3; i++)
                {
                    list.Add(this.CurrentPage + i);
                }
            }

            return list.ToArray();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets a valid page number for a current page, by the specified page, size of each page, and total number of records.
        /// </summary>
        /// <param name="page">The current page in the pagination to validate.</param>
        /// <param name="pageSize">The size of each page in the pagination.</param>
        /// <param name="totalRecords">The total number of records in all pages.</param>
        /// <returns>Returns the valid page number for the current page.</returns>
        private int ValidatePage(int page, int? pageSize, int totalRecords)
        {
            if (pageSize == null)
                return 1;

            // Gets the number of the last page:
            int lastPageNumber = (int)Math.Ceiling((double)totalRecords / pageSize.Value);

            if (page < 1)
                return 1;
            else if ((lastPageNumber > 0) && (page > lastPageNumber))
                return lastPageNumber;
            else
                return page;
        }

        #endregion

    }
}
