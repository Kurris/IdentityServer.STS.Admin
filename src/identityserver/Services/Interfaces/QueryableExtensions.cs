using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using IdentityServer.STS.Admin.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.STS.Admin.Services.Interfaces;

public static class QueryableExtensions
{
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
    {
        return condition
            ? query.Where(predicate)
            : query;
    }


    public static bool isEmpty(this List<int> list)
    {
        return list != null || !list.Any();
    }


    public static async Task<Pagination<T>> ToPaginationBy<T, TKey>(this IQueryable<T> query, Expression<Func<T, TKey>> orderBy, PageIn pageInput, bool orderByDescending = true) where T : class, new()
    {
        query = orderByDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

        var total = await query.CountAsync();
        var items = await query.Skip((pageInput.PageIndex - 1) * pageInput.PageSize).Take(pageInput.PageSize).ToListAsync();

        return new Pagination<T>
        {
            PageIndex = pageInput.PageIndex,
            PageSize = pageInput.PageSize,
            TotalCount = total,
            Items = items,
        };
    }

    public static async Task<Pagination<T>> ToPagination<T>(this IQueryable<T> query, PageIn pageInput) where T : class, new()
    {
        var total = await query.AsNoTracking().CountAsync();
        var items = await query.Skip((pageInput.PageIndex - 1) * pageInput.PageSize)
            .Take(pageInput.PageSize)
            .AsNoTracking()
            .ToListAsync();

        return new Pagination<T>
        {
            PageIndex = pageInput.PageIndex,
            PageSize = pageInput.PageSize,
            TotalCount = total,
            Items = items,
        };
    }
}