namespace NDB.Audit.EF.Models;

public sealed class AuditChange
{
    public string Property { get; init; } = default!;
    public string? OldValue { get; init; }
    public string? NewValue { get; init; }
}
