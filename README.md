# NDB.Audit.EF

Minimal, extensible audit trail for **Entity Framework Core**.

`NDB.Audit.EF` provides a **clean and explicit audit mechanism** for EF Core
without introducing repositories, Unit of Work patterns, or framework-level magic.

This library is designed for **enterprise systems** that need
**reliable audit trails** while keeping EF Core usage **pure and transparent**.

---

## Key Features

- Audit **Entity / Action / Old & New Values / Actor / Timestamp**
- Works with **pure EF Core**
- No repository, no DAL, no framework lock-in
- Explicit, opt-in audit (no hidden hooks)
- Minimal and readable extension methods
- Supports **.NET 8** and **.NET 10**

---

## What This Library Is NOT

- Not a repository pattern
- Not a Unit of Work
- Not a logging framework
- Not a replacement for EF Core
- Not coupled to ASP.NET, HTTP, or infrastructure

---

## Installation

```bash
dotnet add package NDB.Audit.EF
```

## Core Concepts
### 1. Auditable Entity
Only entities that explicitly implement IAuditableEntity
will be included in audit logs.
```code
using NDB.Audit.EF.Abstractions;

public class ProductionVehicle : IAuditableEntity
{
    public Guid Id { get; set; }
    public string EngineNumber { get; set; } = default!;
    public int Status { get; set; }
}
```
### 2. Audit Context (Actor)
IAuditContext provides information about who performs the action.
This is implemented in the application layer, not in the library.
```code
using NDB.Audit.EF.Abstractions;

public class HttpAuditContext : IAuditContext
{
    private readonly IHttpContextAccessor _http;

    public HttpAuditContext(IHttpContextAccessor http)
    {
        _http = http;
    }

    public string? Actor =>
        _http.HttpContext?.User?.Identity?.Name ?? "SYSTEM";
}
```

### 3. Audit Writer (Output)
Audit output is fully pluggable.
You decide where audit data goes:
- Logger
- Database
- Message Queue
- External service

Example: Logger writer
```code
using Microsoft.Extensions.Logging;
using NDB.Audit.EF.Abstractions;
using NDB.Audit.EF.Models;

public class LoggerAuditWriter : IAuditWriter
{
    private readonly ILogger<LoggerAuditWriter> _logger;

    public LoggerAuditWriter(ILogger<LoggerAuditWriter> logger)
    {
        _logger = logger;
    }

    public Task WriteAsync(IEnumerable<AuditEntry> entries, CancellationToken ct)
    {
        foreach (var entry in entries)
        {
            _logger.LogInformation(
                "AUDIT {Entity} {Action} by {Actor} | {Changes}",
                entry.Entity,
                entry.Action,
                entry.Actor,
                entry.Changes);
        }

        return Task.CompletedTask;
    }
}
```

## Dependency Injection Setup
```code
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IAuditContext, HttpAuditContext>();
builder.Services.AddScoped<IAuditWriter, LoggerAuditWriter>();

builder.Services.AddNdbAudit();
```
If no IAuditWriter is registered, audit runs silently.

## Magic Extension (Explicit & Safe)
### SaveChanges with Audit
```code
await dbContext.SaveChangesWithAuditAsync(cancellationToken);
```

This method:
- Calls SaveChangesAsync
- Builds audit entries
- Dispatches them to registered writers
- Does nothing if audit is not configured

## Example: Update Handler
```code
var vehicle = await _db.ProductionVehicles
    .ForAudit()
    .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

if (vehicle == null)
    return Result.NotFound("Vehicle not found");

vehicle.EngineNumber = request.EngineNumber;
vehicle.Status = request.Status;

await _db.SaveChangesWithAuditAsync(ct);

return Result.Ok();
```

## Additional Extensions
### Read-only Queries
```code
var vehicles = await _db.ProductionVehicles
    .ReadOnly()
    .Where(x => x.Status == Active)
    .ToListAsync();
```
### Check Auditable Changes
```code
if (_db.HasAuditableChanges())
{
    await _db.SaveChangesWithAuditAsync(ct);
}
```

## Design Principles
- Explicit over implicit
- Opt-in audit only
- Zero infrastructure coupling
- Minimal surface area
- EF Core remains the source of truth

## Versioning Strategy
- 1.x → supports .NET 8 and .NET 10
- Future major versions may drop older frameworks

EF Core version is resolved by the application, not locked by the library.

## Related Libraries
- NDB.Abstraction — request & result contracts
- NDB.Kit — productivity helpers (AutoMapping, guards, utilities)

Each library is optional and can be used independently.

## Final Notes

If you want a clean audit trail
without sacrificing EF Core clarity,
NDB.Audit.EF gives you exactly that —
no more, no less.