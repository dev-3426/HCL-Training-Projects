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

    public static IQueryable<T> ApplySort<T>(
        IQueryable<T> query, string? sortBy, string? sortOrder,
        IReadOnlyDictionary<string, Func<IQueryable<T>, bool, IQueryable<T>>> sorters,
        string defaultSort)
    {
        var descending = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);
        var key = string.IsNullOrWhiteSpace(sortBy) ? defaultSort : sortBy.Trim().ToLowerInvariant();

        return sorters.TryGetValue(key, out var sorter)
            ? sorter(query, descending)
            : sorters[defaultSort](query, descending);
    }
}
