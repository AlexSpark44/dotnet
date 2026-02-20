# Observability proof

This document records the "proof" artifacts requested for the Orders vertical slice.

## Status

- Orders vertical slice implementation: **not present in this repository yet**.
- Jaeger trace screenshot: **pending** (no runnable API/worker stack in current repo snapshot).

## Target trace (when stack exists)

Capture a single trace that demonstrates:

1. `POST /orders` request in `Platform.Api`.
2. Idempotency decision (new request vs duplicate key).
3. Outbox write in the same transaction as the order write.
4. Cache-aside behavior around order read.
5. Worker polling and dispatching an outbox record.

## Screenshot placeholder

Store the screenshot at:

- `docs/evidence/traces/jaeger-orders-trace.png`

Then embed it here:

```md
![Jaeger trace for Orders vertical slice](evidence/traces/jaeger-orders-trace.png)
```

## Minimal implementation file list (recommended)

If you proceed with the vertical slice, keep boundaries clean with a small set of files:

- `src/Platform.Api/Features/Orders/CreateOrder.cs`
- `src/Platform.Api/Features/Orders/GetOrder.cs`
- `src/Platform.Api/Infrastructure/Idempotency/IdempotencyStore.cs`
- `src/Platform.Api/Infrastructure/Persistence/OutboxMessage.cs`
- `src/Platform.Api/Infrastructure/Caching/OrderCache.cs`
- `src/Platform.Worker/Outbox/OutboxDispatcher.cs`
- `src/Platform.Shared/Observability/Telemetry.cs`

(Adjust paths to match your actual solution layout.)
