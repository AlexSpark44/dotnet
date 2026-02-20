# ADR-0002: Why OpenTelemetry + Structured Logging

- Status: Accepted
- Date: 2026-02-20
- Owners: Platform Engineering + SRE

## Context
We need rapid detection and diagnosis of production issues across API, data, and integration boundaries. Unstructured logs and ad-hoc telemetry create fragmented observability and longer MTTR.

## Decision
Standardize on:
- OpenTelemetry for distributed tracing and metrics.
- Structured logging with correlation IDs for all request paths.
- Health endpoints for liveness/readiness.

## Alternatives Considered
1. Vendor-specific APM SDK only.
2. Logs-only observability model.
3. Metrics-only model with sampled logs.

## Consequences
### Positive
- Vendor-neutral telemetry pipeline and portability.
- Faster incident triage via trace-log correlation.
- Better SLO-based alerting through consistent metrics.

### Negative / Trade-offs
- Additional instrumentation effort and runtime overhead.
- Requires taxonomy governance for metric and span naming.

## Implementation Notes
- Maintain a telemetry naming convention catalog.
- Exclude sensitive payloads/PII from all logs and spans.
- Define alert thresholds aligned with p95 latency and error budgets.

## Follow-up Actions
- [ ] Publish initial dashboard and alert pack.
- [ ] Add span coverage checklist to PR reviews.
