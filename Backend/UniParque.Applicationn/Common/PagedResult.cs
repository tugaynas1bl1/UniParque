using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniParque.Application.Common;

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int Page { get; set; }
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int TotalPages
        => Convert.ToInt32(Math.Ceiling(TotalCount / (double)PageSize));

    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;

    public static PagedResult<T> Create(
        IEnumerable<T> items,
        int totalCount,
        int pageSize,
        int page)
    {
        return new PagedResult<T>
        {
            Page = page,
            Items = items,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
