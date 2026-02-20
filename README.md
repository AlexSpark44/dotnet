# Branch Platform

Consultancy-grade .NET 8 platform scaffold implementing **Clean Architecture**, **observability**, **security by design**, and **reliability patterns**.

## Repository Structure

```text
/src
  /Branch.Platform.Api
  /Branch.Platform.Application
  /Branch.Platform.Domain
  /Branch.Platform.Infrastructure
  /Branch.Platform.Contracts
/tests
  /Branch.Platform.UnitTests
  /Branch.Platform.IntegrationTests
```

## Architecture Decisions

- **Clean Architecture boundaries**: Domain owns business invariants, Application owns use-cases/validation, Infrastructure owns EF/Redis/outbox, API owns transport and cross-cutting concerns.
- **Contract-first API**: explicit request/response contracts in `Contracts` for `/api/v1/orders`.
- **Reliability**:
  - Idempotency key for create order.
  - Outbox table + background dispatcher scaffold.
  - Retry + timeout resilience pipeline with jitter.
- **Observability**:
  - OpenTelemetry traces + metrics + logs.
  - Correlation ID middleware.
  - Health endpoints (`/health/live`, `/health/ready`).
  - Cache hit/miss metrics via `Branch.Platform.Cache` meter.
- **Security by design**:
  - JWT bearer/OIDC skeleton.
  - ProblemDetails + FluentValidation.
  - Rate limiting and secure headers middleware.
  - Configuration placeholders for Azure Key Vault.

## Orders Bounded Context

- Domain entities: `Order`, `OrderItem`, `Money`.
- Use cases:
  - `CreateOrder`
  - `GetOrderById` (cache-aside with TTL strategy)
  - `ListOrders` (paged)
- Persistence: EF Core/Postgres with indexes and no raw SQL.

## Local Run

```bash
docker compose up --build
```

## Testing

- Unit: domain invariants and idempotency behavior in handlers.
- Integration (Testcontainers):
  - POST create order then GET returns it.
  - Cache path scenario (double get).
  - Idempotency returns same order id.

## Deployment and Rollback Strategy

Recommended progressive delivery model:

- **Canary** for low-risk validation under production traffic.
- **Blue/Green** for deterministic cutover and fast rollback.
- Keep schema changes backward-compatible; activate write-path changes after read compatibility is proven.
- Maintain feature flags to disable risky behavior without redeploying.

## Trade-offs

- Outbox dispatcher is scaffolded (logs dispatch) and should be wired to broker/topic of choice.
- Integration tests rely on migrations; add concrete migration files before production cutover.

## Caching Notes (Strategy + Invalidation)

- `GetOrderById` uses cache-aside with Redis and TTL derived from order size.
- Invalidation strategy is **TTL-first** for this scaffold; writes currently rely on natural expiry.
- For production hardening, prefer event-driven invalidation via outbox-dispatched `OrderUpdated`/`OrderCancelled` events.
- Cache behavior is measurable through hit/miss metrics (`Branch.Platform.Cache`) and spans around get/set operations.

## Pattern Justification (Avoid Overengineering)

- Read repository projection was added only for hot read paths to reduce aggregate materialization overhead.
- Outbox is scaffolded because external integration reliability requires it; dispatcher implementation remains intentionally minimal.
- Policies and telemetry are kept centralized to avoid scattering cross-cutting concerns through controllers.
