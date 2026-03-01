# NDB.Audit.EF

> Lightweight and extensible audit logging for Entity Framework Core.
> Automatically captures entity changes and writes structured audit entries via pluggable writers.

---

## Overview

`NDB.Audit.EF` provides a simple, extensible audit mechanism for EF Core applications.

It:

* Tracks changes on entities marked as auditable
* Captures modified properties (old vs new values)
* Includes contextual metadata (actor, correlation, etc.)
* Supports multiple audit writers
* Integrates cleanly with `DbContext`

This library focuses only on **capturing and dispatching audit events**.
It does not enforce storage strategy.

---

# Core Concepts

## 1. Auditable Entities

To enable auditing, your entity must implement:

```csharp
public interface IAuditableEntity
{
}
```

Only entities implementing this interface will be tracked.

---

## 2. Audit Context

Audit metadata is provided via `IAuditContext`:

```csharp
public interface IAuditContext
{
    string? Actor { get; set; }
    string? ActorId { get; set; }
    string? RoleId { get; set; }
    string? RoleName { get; set; }
    string? CorrelationId { get; set; }
}
```

This allows you to attach:

* Current user
* Role
* Correlation ID (for distributed tracing)
* Custom metadata

---

## 3. Audit Entry Model

### AuditEntry

```csharp
public sealed class AuditEntry
{
    public string Entity { get; init; }
    public string EntityId { get; init; }
    public string Action { get; init; }
    public string? Actor { get; init; }
    public DateTime Timestamp { get; init; }
    public IReadOnlyList<AuditChange> Changes { get; init; }
}
```

### AuditChange

```csharp
public sealed class AuditChange
{
    public string Property { get; init; }
    public string? OldValue { get; init; }
    public string? NewValue { get; init; }
}
```

---

# How It Works

When `SaveChangesWithAuditAsync()` is called:

1. EF Core tracks entity changes
2. The library scans `ChangeTracker`
3. It builds `AuditEntry` objects
4. It dispatches them to all registered `IAuditWriter` implementations

---

# Quick Start

## 1️⃣ Register Services

```csharp
services.AddNdbAudit();
```

This registers:

* `IAuditContext`
* `IAuditService`

---

## 2️⃣ Mark Entities

```csharp
public class User : IAuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}
```

---

## 3️⃣ Use SaveChangesWithAuditAsync

```csharp
await context.SaveChangesWithAuditAsync();
```

Extension method:

```csharp
public static async Task<int> SaveChangesWithAuditAsync(
    this DbContext context,
    CancellationToken ct = default)
```

---

# Writing Audit Logs

The library does not dictate where audits are stored.
You must implement `IAuditWriter`.

```csharp
public interface IAuditWriter
{
    Task WriteAsync(
        IEnumerable<AuditEntry> entries,
        CancellationToken ct);
}
```

Example: Save to database

```csharp
public class DbAuditWriter : IAuditWriter
{
    private readonly AuditDbContext _db;

    public async Task WriteAsync(
        IEnumerable<AuditEntry> entries,
        CancellationToken ct)
    {
        _db.AuditLogs.AddRange(entries);
        await _db.SaveChangesAsync(ct);
    }
}
```

Register your writer:

```csharp
services.AddScoped<IAuditWriter, DbAuditWriter>();
```

Multiple writers are supported.

---

# Save With Result

If you need audit entries returned:

```csharp
var entries = await auditService
    .WriteWithResultAsync(context, ct);
```

Or via extension:

```csharp
await context.SaveChangesWithAuditAsync();
```

---

# Change Detection Behavior

For `Modified` entities:

* Compares original vs current values
* Records only changed properties

For `Added` or `Deleted` entities:

* Captures entity + action
* No property diff for added entities

Primary key is extracted automatically.

---

# Utility Extensions

## Check if auditable changes exist

```csharp
context.HasAuditableChanges();
```

## Query modes

```csharp
query.ReadOnly();  // AsNoTracking
query.ForAudit();  // AsTracking
```

---

# Intended Architecture

```text
Application Layer
    ↓
DbContext
    ↓
NDB.Audit.EF
    ↓
IAuditWriter(s)
    ↓
Database / File / External System
```

---

# Design Decisions

* Only entities implementing `IAuditableEntity` are tracked
* Writers are pluggable
* No opinionated storage format
* Minimal reflection usage
* No dependency on ASP.NET
* Scoped lifetime for context-bound metadata

---

# Non-Goals

This library does not:

* Provide a database schema
* Replace EF interceptors
* Provide UI for audit logs
* Implement soft-delete
* Track read access
* Handle encryption

It strictly captures write operations.

---

# Versioning Policy

* MAJOR → Breaking change in audit contract
* MINOR → New features / extensibility
* PATCH → Fixes and improvements

---

# Dependencies

* Microsoft.EntityFrameworkCore
* Microsoft.Extensions.DependencyInjection

Optional:

* NDB.Kit (for extended EF integration)

---

# License

Choose your preferred open-source license (MIT recommended).

---

# Maintained By

Navigate Digital Boundaries (NDB)
