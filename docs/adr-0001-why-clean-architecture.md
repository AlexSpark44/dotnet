# ADR-0001: Why Clean Architecture

- Status: Accepted
- Date: 2026-02-20
- Owners: Platform Engineering

## Context
The platform must remain maintainable under frequent business change, team growth, and technology churn. Previous tightly coupled architectures increased regression risk, slowed onboarding, and made testing expensive.

## Decision
Adopt Clean Architecture with strict directional dependencies:
- Domain: core business rules and invariants.
- Application: use-case orchestration and validation.
- Infrastructure: external systems and persistence details.
- API: transport concerns and request lifecycle.

## Alternatives Considered
1. Layered monolith without strict dependency rules.
2. Vertical slices with direct infrastructure dependencies in handlers.
3. Early microservices split.

## Consequences
### Positive
- Higher testability and modularity.
- Improved replacement flexibility for infrastructure technologies.
- Clear ownership boundaries and easier onboarding.

### Negative / Trade-offs
- Additional abstraction overhead in smaller features.
- Requires discipline to avoid leaking infrastructure into domain/application.

## Implementation Notes
- Enforce project references from outer to inner layers only.
- Keep contracts explicit and versioned.
- Use architecture tests/linting as the codebase matures.

## Follow-up Actions
- [ ] Add automated architecture rule tests.
- [ ] Add dependency graph validation in CI.
