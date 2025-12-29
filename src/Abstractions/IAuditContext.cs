namespace NDB.Audit.EF.Abstractions;

public interface IAuditContext
{
    string? Actor { get; set; }
    string? ActorId { get; set; }
    string? CorrelationId { get; set; }
}
