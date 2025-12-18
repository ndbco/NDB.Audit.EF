namespace NDB.Audit.EF.Models;

public sealed class AuditEntry
{
    public string Entity { get; init; } = default!;
    public string EntityId { get; init; } = default!;
    public string Action { get; init; } = default!;
    public string? Actor { get; init; }
    public DateTime Timestamp { get; init; }
    public IReadOnlyList<AuditChange> Changes { get; init; } = [];
}
