# Engineering Standards: Extreme Best Practices

These standards are non-negotiable for this platform. They exist to optimize for **long-term maintainability, predictable delivery, and safe operations at scale**.

## 1) Architecture & Design

### 1.1 Clean Architecture by default
- Domain contains business rules and no infrastructure concerns.
- Application coordinates use cases and orchestration.
- Infrastructure implements external dependencies.
- API/UI is a delivery mechanism, not a source of business logic.

**Why:** Enforces testability, modularity, and reduces coupling so change cost stays low over time.

### 1.2 SOLID + composition over inheritance
- Single responsibility per type.
- Explicit interfaces for boundaries.
- Dependency inversion across layers.

**Why:** Improves extensibility and lowers regression risk.

### 1.3 Contract-first external interfaces
- All external DTOs/models live in contracts.
- Breaking changes require versioning strategy and migration plan.

**Why:** Protects consumers and stabilizes platform evolution.

## 2) Reliability Engineering

### 2.1 No deployment without rollback
- Every release strategy documents rollback procedure.
- Backward-compatible migrations required before feature activation.

**Why:** Mean-time-to-recovery (MTTR) is as important as feature velocity.

### 2.2 Idempotency for critical writes
- Public write endpoints should support idempotency keys.

**Why:** Prevents duplicate side effects under retries/network partitions.

### 2.3 Timeouts, retries, and jitter
- All outbound/network calls use bounded timeout.
- Retry only transient failures with exponential backoff + jitter.

**Why:** Prevents cascading failures and retry storms.

### 2.4 Outbox/event delivery guarantees
- Domain events persisted atomically with state changes.
- Dispatcher processes asynchronously with retry and dead-letter strategy.

**Why:** Prevents lost events and inconsistent integration behavior.

## 3) Security by Design

### 3.1 OWASP mindset in all changes
- Validate all input; reject unknown/invalid payloads.
- No sensitive data in logs.
- Principle of least privilege for credentials and authorization.

**Why:** Security defects are cheapest to prevent at design time.

### 3.2 Secret management
- No secrets in source control.
- Use environment/config providers and secure vault integrations.

**Why:** Reduces blast radius of credential leakage.

### 3.3 Secure defaults
- ProblemDetails for errors.
- Security headers enabled.
- Rate limiting on public APIs.

**Why:** Baseline hardening protects against common abuse patterns.

## 4) Observability as a Feature

### 4.1 Logs, metrics, traces are required
- Structured logs with correlation IDs.
- SLI/SLO-aligned metrics.
- Distributed tracing for request paths.

**Why:** If we cannot observe it, we cannot reliably operate it.

### 4.2 Health and readiness
- Separate liveness and readiness checks.

**Why:** Prevents bad instances from receiving traffic and reduces incident impact.

## 5) Performance Discipline

### 5.1 Measurement-first
- Define baseline, target p95/p99, and throughput before optimization.

**Why:** Avoids premature optimization and focuses effort where it matters.

### 5.2 Database-aware development
- Index for query paths.
- Use projections and pagination.
- No unsafe/raw SQL unless reviewed and parameterized.

**Why:** Data layer is usually the primary latency and scaling bottleneck.

## 6) Testing & Quality Gates

### 6.1 No code without tests
- Unit tests for logic and invariants.
- Integration tests for boundary behavior and data consistency.

**Why:** Enables refactoring confidence and prevents regressions.

### 6.2 CI quality gates
- Build, test, static analysis, and contract checks in CI.

**Why:** Enforces consistent quality independent of developer environment.

## 7) Operability & Documentation

### 7.1 ADRs for meaningful architectural decisions
- Record context, decision, consequences, and alternatives.

**Why:** Preserves institutional memory for future teams.

### 7.2 Incident learning loop
- Every major incident produces corrective actions and ownership.

**Why:** Reliability improves only when learning is systematized.
