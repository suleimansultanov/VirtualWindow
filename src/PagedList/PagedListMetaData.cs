using System;
using PagedList.Interfaces;

namespace PagedList
{
	///<summary>
	/// Non-enumerable version of the PagedList class.
	///</summary>
	[Serializable]
	public class PagedListMetaData : IPaginationInfo
	{
		/// <summary>
		/// Protected constructor that allows for instantiation without passing in a separate list.
		/// </summary>
		protected PagedListMetaData()
		{
		}

		///<summary>
		/// Non-enumerable version of the PagedList class.
		///</summary>
		///<param name="pagedList">A PagedList (likely enumerable) to copy metadata from.</param>
		public PagedListMetaData(IPaginationInfo pagedList)
		{
			PageCount = pagedList.PageCount;
			TotalItemCount = pagedList.TotalItemCount;
			PageNumber = pagedList.PageNumber;
			PageSize = pagedList.PageSize;
			HasPreviousPage = pagedList.HasPreviousPage;
			HasNextPage = pagedList.HasNextPage;
			IsFirstPage = pagedList.IsFirstPage;
			IsLastPage = pagedList.IsLastPage;
			FirstItemOnPage = pagedList.FirstItemOnPage;
			LastItemOnPage = pagedList.LastItemOnPage;
		}

        /// <summary>
        /// 	Initializes a new instance of a type deriving from <see cref = "BasePagedList{T}" /> and sets properties needed to calculate position and size data on the subset and superset.
        /// </summary>
        /// <param name = "pageNumber">The one-based index of the subset of objects contained by this instance.</param>
        /// <param name = "pageSize">The maximum size of any individual subset.</param>
        protected internal PagedListMetaData(int pageNumber, int pageSize)
        {
            InitFields(pageNumber, pageSize);
        }

        /// <summary>
        /// 	Initializes a new instance of a type deriving from <see cref = "BasePagedList{T}" /> and sets properties needed to calculate position and size data on the subset and superset.
        /// </summary>
        /// <param name = "pageNumber">The one-based index of the subset of objects contained by this instance.</param>
        /// <param name = "pageSize">The maximum size of any individual subset.</param>
        /// <param name = "totalItemCount">The size of the superset.</param>
        protected internal PagedListMetaData(int pageNumber, int pageSize, int totalItemCount): this(pageNumber, pageSize)
		{
            InitFields(totalItemCount);
        }

        #region InitFields

        /// <summary>
        /// 
        /// </summary>
        /// <param name = "pageNumber">The one-based index of the subset of objects contained by this instance.</param>
        /// <param name = "pageSize">The maximum size of any individual subset.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void InitFields(int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
                throw new ArgumentOutOfRangeException(nameof(pageNumber), pageNumber, "PageNumber cannot be below 1.");
            if (pageSize < 1)
                throw new ArgumentOutOfRangeException(nameof(pageSize), pageSize, "PageSize cannot be less than 1.");

            PageSize = pageSize;
            PageNumber = pageNumber;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name = "totalItemCount">The size of the superset.</param>
        protected void InitFields(int totalItemCount)
        {
            // set source to blank list if superset is null to prevent exceptions
            TotalItemCount = totalItemCount;

            PageCount = TotalItemCount > 0 ? (int)Math.Ceiling(TotalItemCount / (double)PageSize) : 0;
            if (PageNumber > PageCount) PageNumber = Math.Max(PageCount, 1);
            HasPreviousPage = PageNumber > 1;
            HasNextPage = PageNumber < PageCount;
            IsFirstPage = PageNumber == 1;
            IsLastPage = PageNumber >= PageCount;
            FirstItemOnPage = (PageNumber - 1) * PageSize + 1;
            var numberOfLastItemOnPage = FirstItemOnPage + PageSize - 1;
            LastItemOnPage = numberOfLastItemOnPage > TotalItemCount
                                 ? TotalItemCount
                                 : numberOfLastItemOnPage;
        }

        #endregion

        #region IPagedList Members

        /// <summary>
        /// 	Total number of subsets within the superset.
        /// </summary>
        /// <value>
        /// 	Total number of subsets within the superset.
        /// </value>
        public int PageCount { get; protected set; }

		/// <summary>
		/// 	Total number of objects contained within the superset.
		/// </summary>
		/// <value>
		/// 	Total number of objects contained within the superset.
		/// </value>
		public int TotalItemCount { get; protected set; }
		
		/// <summary>
		/// 	One-based index of this subset within the superset.
		/// </summary>
		/// <value>
		/// 	One-based index of this subset within the superset.
		/// </value>
		public int PageNumber { get; protected set; }

		/// <summary>
		/// 	Maximum size any individual subset.
		/// </summary>
		/// <value>
		/// 	Maximum size any individual subset.
		/// </value>
		public int PageSize { get; protected set; }

		/// <summary>
		/// 	Returns true if this is NOT the first subset within the superset.
		/// </summary>
		/// <value>
		/// 	Returns true if this is NOT the first subset within the superset.
		/// </value>
		public bool HasPreviousPage { get; protected set; }

		/// <summary>
		/// 	Returns true if this is NOT the last subset within the superset.
		/// </summary>
		/// <value>
		/// 	Returns true if this is NOT the last subset within the superset.
		/// </value>
		public bool HasNextPage { get; protected set; }

		/// <summary>
		/// 	Returns true if this is the first subset within the superset.
		/// </summary>
		/// <value>
		/// 	Returns true if this is the first subset within the superset.
		/// </value>
		public bool IsFirstPage { get; protected set; }

		/// <summary>
		/// 	Returns true if this is the last subset within the superset.
		/// </summary>
		/// <value>
		/// 	Returns true if this is the last subset within the superset.
		/// </value>
		public bool IsLastPage { get; protected set; }

		/// <summary>
		/// 	One-based index of the first item in the paged subset.
		/// </summary>
		/// <value>
		/// 	One-based index of the first item in the paged subset.
		/// </value>
		public int FirstItemOnPage { get; protected set; }

		/// <summary>
		/// 	One-based index of the last item in the paged subset.
		/// </summary>
		/// <value>
		/// 	One-based index of the last item in the paged subset.
		/// </value>
		public int LastItemOnPage { get; protected set; }

		#endregion
	}
}
