# Project docs

This repository keeps architecture and engineering guidance under `docs/`.

## Cloud-Native Proof (No Deployment)

This site publishes **evidence produced by CI**.

## Evidence

- [OpenAPI](evidence/openapi/)
- [Coverage](evidence/coverage/)
- [Benchmarks](evidence/benchmarks/)
- [Traces (screenshots)](evidence/traces/)

## Contents

- [Architecture](architecture.md)
- [Reliability](reliability.md)
- [Observability](observability.md)
- [Security](security.md)
- [Delivery](delivery.md)

## Expected structure

```text
docs/
  index.md
  architecture.md
  reliability.md
  observability.md
  security.md
  delivery.md          # CI + rollback strategy, no runtime deploy
  evidence/
    coverage/          # generated HTML
    benchmarks/        # generated reports
    openapi/           # rendered API docs
    traces/            # screenshots + captions
```
