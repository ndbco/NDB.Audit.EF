using Microsoft.EntityFrameworkCore;

namespace NDB.Audit.EF.Extensions;

public static class QueryAuditExtensions
{
    public static IQueryable<T> ReadOnly<T>(this IQueryable<T> query)
        where T : class
        => query.AsNoTracking();

    public static IQueryable<T> ForAudit<T>(this IQueryable<T> query)
        where T : class
        => query.AsTracking();
}
