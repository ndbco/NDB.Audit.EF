using Microsoft.EntityFrameworkCore;

namespace NDB.Audit.EF.Extensions;

public static class ChangeTrackerExtensions
{
    public static bool HasAuditableChanges(this DbContext context)
        => context.ChangeTracker.Entries()
            .Any(e => e.Entity is Abstractions.IAuditableEntity &&
                      e.State != EntityState.Unchanged);
}
