using backend.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace backend.Extensions;
public static class IQueryableExtensio
{
    extension<T>(IQueryable<T> queryable)
    {
        public async Task<PaginationItem<T>> PaginationItemAsync(PaginationRequest paginationRequest)
        {
            var pageNumber = paginationRequest.PageNumber <= 0 ? 1 : paginationRequest.PageNumber;
            var pageSize = paginationRequest.PageSize <= 0 ? 10 : paginationRequest.PageSize;

            var countItem = await queryable.CountAsync();
            var data = await queryable.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var pagination = new PaginationItem<T>(pageNumber: pageNumber, pageSize: pageSize, totalItem: countItem, data: data);
            return pagination;
            
        }
    }
}
