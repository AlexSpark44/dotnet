# Branch Platform (.NET 8 LTS)

![CI](../../actions/workflows/ci.yml/badge.svg)
![Pages](../../actions/workflows/pages.yml/badge.svg)

Production-grade Clean Architecture platform with an Orders bounded context, Azure-first infrastructure, and deployment tooling where possible in C#.

## Repository Structure

```text
/src
  /Branch.Platform.Api
  /Branch.Platform.Application
  /Branch.Platform.Domain
  /Branch.Platform.Infrastructure
  /Branch.Platform.Contracts
  /Branch.Platform.Worker
/tests
  /Branch.Platform.UnitTests
  /Branch.Platform.IntegrationTests
/infra
  Pulumi C# Azure provisioning
/deploy
  C# deployment helper CLI
/docs
  engineering and operations standards/templates
```

## Architecture Decisions

- **Clean Architecture**: strict boundary ownership by layer.
- **Contract-first API**: transport DTOs live in Contracts; no EF entities leak into API.
- **Reliability**: idempotent create, outbox scaffold, retries with jitter + timeout, cancellation token propagation.
- **Observability**: OpenTelemetry traces/metrics/logging, request correlation IDs, health endpoints.
- **Security**: JWT/OIDC-ready auth, policy-based authorization (`orders.write`, `orders.read`), ProblemDetails, rate limiting.
- **Performance**: EF projections + `AsNoTracking`, indexes, Redis cache-aside with cache metrics.

## Orders API

- `POST /api/v1/orders` (requires `Idempotency-Key`, `orders.write` policy)
- `GET /api/v1/orders/{id}` (`orders.read` policy, cache-aside)
- `GET /api/v1/orders` paged list (`orders.read` policy)

## Local Run

```bash
docker compose up --build
# API: http://localhost:8080
# Jaeger UI: http://localhost:16686
```

## Tests

- Unit tests in `tests/Branch.Platform.UnitTests`
- Integration tests in `tests/Branch.Platform.IntegrationTests` using Testcontainers (Postgres + Redis)

## Azure Provisioning (Pulumi C#)

```bash
cd infra
pulumi stack init dev # once
pulumi up -s dev
```

Outputs include Container App URL, ACR login server, key vault name, and major connection endpoints.

## Deployment (C# helper)

```bash
dotnet run --project deploy/src/Branch.Platform.Deploy -- all
```

Environment variables:
- `BRANCH_ACR_SERVER`
- `BRANCH_IMAGE_NAME`
- `BRANCH_IMAGE_TAG`
- `BRANCH_PULUMI_STACK`

## Rollback Strategy

Use Container Apps revision-based rollback:
1. Deploy as new revision.
2. Shift traffic gradually.
3. On error budget breach, route 100% traffic back to prior healthy revision.
4. Keep schema backward-compatible during rollout to allow instant app rollback.

## Caching Notes

- Cache-aside for `GetOrderById` with measured TTL strategy.
- Current invalidation: TTL-first for scaffold simplicity.
- Recommended hardening: outbox-driven cache invalidation events.

## Trade-offs

- Outbox dispatcher is scaffolded (logging-only dispatch) and must be wired to real messaging.
- Pulumi networking for Postgres is currently public-access with restrictive firewall starter rule; move to private networking for higher security environments.


## Background Worker

- `Branch.Platform.Worker` runs infrastructure background processes (e.g., outbox dispatcher) outside API request pipeline.
- In local compose, `worker` runs as a separate container to mirror production separation-of-concerns.
