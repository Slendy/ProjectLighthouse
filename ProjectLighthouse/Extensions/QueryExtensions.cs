using System;
using System.Collections.Generic;
using System.Linq;
using LBPUnion.ProjectLighthouse.Types.Entities.Profile;
using LBPUnion.ProjectLighthouse.Types.Roles;

namespace LBPUnion.ProjectLighthouse.Extensions;

public static class QueryExtensions
{
    public static List<T2> ToSerializableList<T, T2>(this IEnumerable<T> enumerable, Func<T, T2> selector)
        => enumerable.Select(selector).ToList();

    public static IQueryable<UserEntity> InverseHasPermission
        (this IQueryable<UserEntity> queryable, Entitlements entitlements) =>
        queryable.Where(u => (u.Permissions & entitlements) == 0);

    public static IQueryable<UserEntity> HasPermission
        (this IQueryable<UserEntity> queryable, Entitlements entitlements) =>
        queryable.Where(u => (u.Permissions & entitlements) == entitlements);
}