# Performance Playbook

## Objective
Engineer performance intentionally using measurement-first practices.

## 1) Measurement-First Workflow
1. Define SLO-aligned targets (e.g., p95 latency, throughput, error rate).
2. Capture baseline before code changes.
3. Profile and identify the dominant bottleneck.
4. Implement one optimization at a time.
5. Re-measure and compare against baseline.

## 2) Baseline Standards
- Record p50/p95/p99 latency.
- Record CPU, memory, DB connection usage.
- Track cache hit ratio and downstream dependency latency.
- Test with representative payload sizes and concurrency.

## 3) p95-Focused Tuning
- Optimize tail latency first (p95/p99), not average latency.
- Investigate contention, lock duration, and queueing effects.
- Avoid unbounded fan-out and synchronous waterfalls.

## 4) Query Analysis
- Use query plans (`EXPLAIN ANALYZE`) on slow paths.
- Ensure indexes align with filtering and sorting predicates.
- Prefer projections over loading full aggregates for reads.
- Enforce pagination for all list endpoints.

## 5) Caching Policy
- Use cache-aside for read-heavy workloads.
- Define TTL strategy per data volatility and SLA.
- Measure hit/miss rates and stale-read risk.
- Plan invalidation approach explicitly (event-driven or TTL-based).

## 6) Load/Stress Testing
- Include steady-state and burst scenarios.
- Include degraded dependency scenarios and timeout behavior.
- Validate autoscaling and saturation thresholds.

## 7) Guardrails in CI/CD
- Add repeatable performance smoke tests for critical paths.
- Flag regressions beyond agreed thresholds.
- Keep benchmark environment/config as code.
