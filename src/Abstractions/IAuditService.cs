using Microsoft.EntityFrameworkCore;

namespace NDB.Audit.EF.Abstractions;

public interface IAuditService
{
    Task WriteAsync(DbContext context, CancellationToken ct);
}
