namespace NDB.Audit.EF.Abstractions;

public interface IAuditContext
{
    string? Actor { get; }
}
