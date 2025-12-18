using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NDB.Audit.EF.Abstractions;
using NDB.Audit.EF.Models;

namespace NDB.Audit.EF.Internal;

internal static class AuditEntryBuilder
{
    public static List<AuditEntry> Build(ChangeTracker tracker, string? actor)
    {
        var result = new List<AuditEntry>();

        foreach (var entry in tracker.Entries<IAuditableEntity>())
        {
            if (entry.State is EntityState.Unchanged or EntityState.Detached)
                continue;

            var changes = new List<AuditChange>();

            if (entry.State == EntityState.Modified)
            {
                foreach (var prop in entry.Properties)
                {
                    if (!Equals(prop.OriginalValue, prop.CurrentValue))
                    {
                        changes.Add(new AuditChange
                        {
                            Property = prop.Metadata.Name,
                            OldValue = prop.OriginalValue?.ToString(),
                            NewValue = prop.CurrentValue?.ToString()
                        });
                    }
                }
            }

            result.Add(new AuditEntry
            {
                Entity = entry.Entity.GetType().Name,
                EntityId = GetPrimaryKey(entry),
                Action = entry.State.ToString(),
                Actor = actor,
                Timestamp = DateTime.UtcNow,
                Changes = changes
            });
        }

        return result;
    }

    private static string GetPrimaryKey(EntityEntry entry)
    {
        var key = entry.Metadata.FindPrimaryKey();
        if (key == null) return "-";

        return string.Join(",",
            key.Properties.Select(p =>
                entry.Property(p.Name).CurrentValue?.ToString() ?? "-"
            ));
    }
}
