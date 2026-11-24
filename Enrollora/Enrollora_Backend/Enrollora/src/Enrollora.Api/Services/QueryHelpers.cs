using Enrollora.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Enrollora.Api.Services;

internal static class QueryHelpers
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        IQueryable<T> query, PaginationQuery paging, CancellationToken ct)
    {
        paging.Normalize();

        var total = await query.CountAsync(ct);
        var items = await query
            .Skip((paging.PageNumber - 1) * paging.PageSize)
            .Take(paging.PageSize)
            .ToListAsync(ct);

        return new PagedResult<T>(
            items,
            paging.PageNumber,
            paging.PageSize,
            total,
            (int)Math.Ceiling(total / (double)paging.PageSize));
    }
}
