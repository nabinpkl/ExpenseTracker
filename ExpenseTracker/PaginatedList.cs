using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseTracker
{
    public class PaginatedList<T>:List<T>
    {
        public int PageIndex { get; private set; } // Currently displayed page number in view
        public int TotalPages { get; private set; } //total no of pages available

        public PaginatedList(List<T> items,int count,int pageIndex,int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);//total no of pages calculated by dividing total count by no of elements in each page
            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1); //for disabling previous link in first page
            }
        }
        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages); //for disabling next link at end page
            }
        }

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source,int pageIndex,int pageSize)
        {
            var count = await source.CountAsync();
            /*
             * Skip upto current page no. * no of elements and take required no of elements
             * pageIndex -1 beacuse of 0 indexing in list
             */
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
