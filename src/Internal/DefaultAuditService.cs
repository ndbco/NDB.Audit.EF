using Microsoft.EntityFrameworkCore;
using NDB.Audit.EF.Abstractions;
using NDB.Audit.EF.Internal;
using NDB.Audit.EF.Models;

namespace NDB.Audit.EF.Internal;

internal sealed class DefaultAuditService : IAuditService
{
    private readonly IAuditContext _context;
    private readonly IEnumerable<IAuditWriter> _writers;

    public DefaultAuditService(
        IAuditContext context,
        IEnumerable<IAuditWriter> writers)
    {
        _context = context;
        _writers = writers;
    }

    public async Task WriteAsync(DbContext context, CancellationToken ct)
    {
        var entries = AuditEntryBuilder.Build(
            context.ChangeTracker,
            _context.Actor);

        if (entries.Count == 0)
            return;

        foreach (var writer in _writers)
        {
            await writer.WriteAsync(entries, ct);
        }
    }
    public async Task<IReadOnlyList<AuditEntry>> WriteWithResultAsync(DbContext context,CancellationToken ct)
    {
        var entries = AuditEntryBuilder.Build(
            context.ChangeTracker,
            _context.Actor);

        if (entries.Count == 0)
            return entries;

        foreach (var writer in _writers)
        {
            await writer.WriteAsync(entries, ct);
        }

        return entries;
    }

}
