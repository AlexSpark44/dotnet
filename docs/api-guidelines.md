# API Guidelines

## 1) Versioning
- Use explicit URL versioning: `/api/v{version}/...`.
- Breaking changes require a new major API version.
- Non-breaking additive changes can be introduced within the same version.

## 2) Backwards Compatibility
- Do not remove or repurpose existing response fields in active versions.
- Additive fields must be optional and safe for old clients.
- Avoid tightening validation rules without deprecation notice.

## 3) Deprecation Policy
- Mark endpoints/fields as deprecated with timeline.
- Minimum deprecation window: 90 days (or contractual agreement).
- Publish migration guidance and examples.
- Track usage telemetry before final removal.

## 4) Error Model (ProblemDetails)
- Use RFC 7807 `application/problem+json`.
- Include: `type`, `title`, `status`, `detail`, `instance`.
- Add extension members when needed (e.g. `traceId`, validation errors).
- Never leak stack traces, secrets, SQL, or internal infrastructure details.

### Validation Errors
- Return `400` with structured field-level error details.
- Use stable error codes for client handling where possible.

### Authorization/AuthN Errors
- `401` for unauthenticated requests.
- `403` for authenticated but unauthorized requests.

### Not Found & Conflicts
- `404` when resource does not exist.
- `409` for state conflicts/idempotency collisions.

## 5) Paging, Filtering, Sorting
- Require explicit paging on collection endpoints.
- Default and max page sizes documented.
- Ensure deterministic sorting with stable fields.

## 6) Idempotency for Writes
- Support `Idempotency-Key` header for critical create/command endpoints.
- Return same outcome for duplicate keys within retention window.

## 7) Security Expectations
- Validate all inputs and enforce allow-list semantics.
- Apply rate limiting for abuse protection.
- Include correlation IDs for diagnostic continuity.
