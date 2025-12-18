using NDB.Audit.EF.Models;

namespace NDB.Audit.EF.Abstractions;

public interface IAuditWriter
{
    Task WriteAsync(IEnumerable<AuditEntry> entries, CancellationToken ct);
}
