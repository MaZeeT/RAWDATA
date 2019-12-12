using System;

namespace DatabaseService
{
    public class PagingAttributes
    {
        public const int MaxPageSize = 100;
        public const int DefPageSize = 10;
        private int _pageSize = DefPageSize;
        public const int FirstPage = 1; // first page is page one, not page zero
        private int _page = FirstPage;
        public int Page
        {
            get => _page;
            set => _page = Math.Max(value, FirstPage);
        }
        public int PageSize {
            get => _pageSize;
            set => _pageSize = Math.Abs(Math.Min(value, MaxPageSize));
        }
    }
}
