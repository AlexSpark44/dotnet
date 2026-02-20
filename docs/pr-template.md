# Pull Request Template

## Summary
- What changed?
- Why now?
- Scope boundaries and non-goals.

## Architecture Impact
- Layers/components touched:
- Any ADR needed/updated:
- Backward compatibility impact:

## Testing
- [ ] Unit tests added/updated
- [ ] Integration tests added/updated
- [ ] Non-flaky assertions and deterministic setup
- [ ] Local verification commands listed

Commands executed:
```bash
# example
dotnet restore Branch.Platform.sln
dotnet build Branch.Platform.sln -c Release
dotnet test Branch.Platform.sln -c Release
```

## Monitoring / Observability
- [ ] Logs updated (structured, no sensitive data)
- [ ] Metrics added/updated (name + intent)
- [ ] Traces span coverage considered
- [ ] Dashboards/alerts updated or follow-up ticket created

## Security Considerations
- [ ] Input validation reviewed
- [ ] AuthN/AuthZ implications reviewed
- [ ] Secret/config handling reviewed
- [ ] OWASP-style abuse/failure paths considered

## Rollout & Rollback
- Rollout plan:
- Rollback plan:
- Migration strategy (if any):
- Feature flags (if any):

## Risks
- Primary risks:
- Mitigations:
- Contingency triggers:

## Checklist
- [ ] Documentation updated
- [ ] API contract changes versioned/deprecated correctly
- [ ] Performance implications measured or profiled
- [ ] Team reviewers from affected domains requested
