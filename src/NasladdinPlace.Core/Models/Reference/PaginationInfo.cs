using System;
using PagedList.Interfaces;

namespace NasladdinPlace.Core.Models.Reference
{
    /// <summary>
    /// Модель используемая для постраничного формирования данных
    /// </summary>
    public class PaginationInfo
    {
        /// <summary>
        /// Текущая страница
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Размер страницы - кол-во элементов на странице
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Общее количество элементов на всех страницах (Вычисляется)
        /// </summary>
        public int TotalItems { get; set; }


        public PaginationInfo()
        {
        }

        public PaginationInfo(int page, int pageSize, int totalItems = 0)
        {
            Page = page;
            PageSize = pageSize;
            TotalItems = totalItems;
        }

        public PaginationInfo(IPaginationInfo info)
        {
            Page = info.PageNumber;
            PageSize = info.PageSize;
            TotalItems = info.TotalItemCount;
        }

        public void ResetPage()
        {
            Page = 0;
        }

        public bool TryIncrementPage()
        {
            if (HasNextPage)
            {
                Page++;
                return true;
            }
            return false;
        }

        private int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
        private bool HasNextPage => Page < TotalPages;
    }
}
