using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using NDB.Audit.EF.Abstractions;

namespace NDB.Audit.EF.Extensions;

public static class DbContextAuditExtensions
{
    public static async Task<int> SaveChangesWithAuditAsync(
        this DbContext context,
        CancellationToken ct = default)
    {
        var auditService = context.GetService<IAuditService>();

        var result = await context.SaveChangesAsync(ct);

        if (auditService != null)
        {
            await auditService.WriteAsync(context, ct);
        }

        return result;
    }
}
