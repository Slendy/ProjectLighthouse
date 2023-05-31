using System;
using System.Collections.Generic;
using System.Linq;
using LBPUnion.ProjectLighthouse.Logging;
using LBPUnion.ProjectLighthouse.Types.Filter;
using LBPUnion.ProjectLighthouse.Types.Filter.Sorts;
using LBPUnion.ProjectLighthouse.Types.Logging;
using Microsoft.EntityFrameworkCore;
using LBPUnion.ProjectLighthouse.Types.Entities.Profile;
using LBPUnion.ProjectLighthouse.Types.Roles;

namespace LBPUnion.ProjectLighthouse.Extensions;

public static class QueryExtensions
{
    public static List<T2> ToSerializableList<T, T2>(this IEnumerable<T> enumerable, Func<T, T2> selector)
        => enumerable.Select(selector).ToList();

    public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> queryable, PaginationData pagination)
    {
        if (pagination.MaxElements <= 0)
        {
            Logger.Warn($"ApplyPagination() called with MaxElements of {pagination.MaxElements}\n{queryable.ToQueryString()}", LogArea.Database);
            pagination.MaxElements = pagination.PageSize;
        }
        queryable = queryable.Skip(Math.Max(0, pagination.PageStart - 1));
        return queryable.Take(Math.Min(pagination.PageSize, Math.Min(1000, pagination.MaxElements)));
    }

    public static IOrderedQueryable<T> ApplyOrdering<T>
        (this IQueryable<T> queryable, ISortBuilder<T> sortBuilder) =>
        sortBuilder.Build(queryable);

    public static IQueryable<UserEntity> InverseHasPermission
        (this IQueryable<UserEntity> queryable, Entitlements entitlements) =>
        queryable.Where(u => (u.Permissions & entitlements) == 0);

    public static IQueryable<UserEntity> HasPermission
        (this IQueryable<UserEntity> queryable, Entitlements entitlements) =>
        queryable.Where(u => (u.Permissions & entitlements) == entitlements);
}