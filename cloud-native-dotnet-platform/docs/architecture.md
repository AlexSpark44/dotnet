# Architecture

## Goals
- Maintainable in 5 years
- Cloud-native behaviors: resilience + observability + automation
- Clean separation: Domain / Application / Infrastructure / API

## High-level diagram
![Architecture](assets/architecture.png)

## Projects
- Platform.Domain: entities, value objects, domain events
- Platform.Application: use cases, validation, DTOs
- Platform.Infrastructure: EF Core, Redis, external adapters
- Platform.Api: HTTP surface, versioning, auth hooks, telemetry
- Platform.Worker: outbox dispatch, retries, metrics

## Flows
### Create Order
1) API validates + authorizes
2) Application use case executes
3) EF Core transaction writes Order + Outbox message
4) Worker dispatches outbox asynchronously
