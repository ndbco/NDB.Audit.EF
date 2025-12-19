using Microsoft.EntityFrameworkCore;
using NDB.Audit.EF.Models;

namespace NDB.Audit.EF.Abstractions;

public interface IAuditService
{
    Task WriteAsync(DbContext context, CancellationToken ct);
    Task<IReadOnlyList<AuditEntry>> WriteWithResultAsync(DbContext context,CancellationToken ct);

}
