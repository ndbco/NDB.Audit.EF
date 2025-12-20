using NDB.Audit.EF.Abstractions;

namespace NDB.Audit.EF.Internal;

internal sealed class AuditContext : IAuditContext
{
    public string? Actor { get; set; }
}
