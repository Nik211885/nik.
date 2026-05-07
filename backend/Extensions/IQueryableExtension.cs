using backend.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace backend.Extensions;

/// <summary>
/// Extension members for <see cref="IQueryable{T}"/> providing pagination utilities.
/// </summary>
public static class IQueryableExtensio
{
    extension<T>(IQueryable<T> queryable)
    {
        /// <summary>
        /// Executes the query with server-side pagination and returns a
        /// <see cref="PaginationItem{T}"/> containing the current page of results.
        /// Defaults to page 1 and page size 10 when the request values are invalid.
        /// </summary>
        /// <param name="paginationRequest">Requested page number and page size.</param>
        /// <returns>A paginated result wrapper with metadata.</returns>
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
