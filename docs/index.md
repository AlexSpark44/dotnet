# Project docs

This repository keeps architecture and engineering guidance under `docs/`.

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

## Evidence folders

- `evidence/coverage/` — generated HTML coverage reports.
- `evidence/benchmarks/` — generated benchmark reports.
- `evidence/openapi/` — rendered OpenAPI documentation.
- `evidence/traces/` — trace screenshots and short captions.
