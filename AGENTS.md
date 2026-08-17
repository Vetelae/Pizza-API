# Pizza API Guidance

## Project
- Single ASP.NET Core Web API targeting .NET 10.
- EF Core 10 with PostgreSQL and snake_case database naming.
- ASP.NET Core Identity/JWT, SignalR, built-in rate limiting, Resend, Scalar, and health checks.
- Root namespace: `Pizza_API`.

## Structure and architecture
- Controllers are grouped under `Controllers/Admin`, `Controllers/Public`, and `Controllers/User`.
- Feature interfaces and implementations live together under `Services/<Feature>` and generally use the `Pizza_API.Services` namespace.
- DTOs live under `Entities/Dtos/<Feature>`; EF entities live directly under `Entities`.
- Use `ApplicationDbContext` directly through the existing service pattern. Do not add repositories, MediatR, AutoMapper, or a new architectural layer unless explicitly requested.
- Register new services and options in `Program.cs`.

## API and business logic
- Keep controllers limited to routing, authorization, identity/context extraction, service calls, and HTTP results.
- Keep business rules, persistence, and entity-to-DTO mapping in services/helpers.
- Do not expose EF entities through API or SignalR contracts.
- Follow adjacent naming and formatting: PascalCase members, `_camelCase` fields, constructor injection, block-scoped namespaces, and `Async` suffixes for asynchronous I/O.

## Validation and errors
- Put request-shape validation on DTOs with DataAnnotations and reuse limits from `Constants`.
- Cross-field DTO validation must implement `IValidatableObject`.
- Enforce database-dependent and business validation in services.
- Throw `ValidationException`, `NotFoundException`, or `ConflictException` for expected service failures; let `GlobalExceptionHandler` produce `ProblemDetails`.
- Preserve auth-specific response behavior, including non-enumerating forgot-password and lockout responses.

## Data and configuration
- Put EF relationships, delete behavior, indexes, lengths, and decimal precision in `Configurations`; configurations are discovered automatically.
- Use DTO projections and `AsNoTracking()` for read-only queries when practical.
- Store operational timestamps in UTC. Use `TimeProvider` and the configured business timezone for date-based reporting.
- Add a new EF migration for schema changes; do not rewrite existing migrations.
- Do not commit credentials, tokens, API keys, or production connection strings.

## Security and realtime
- Admin routes and the admin order hub require the `Admin` role.
- Scope user data queries by the authenticated user ID.
- Preserve lookup-token checks for guest order access and customer order subscriptions.
- Preserve authenticated-user/`X-Session-Id` cart ownership.
- Persist order changes before publishing SignalR notifications.

## Workflow
- Inspect the directly affected files, one analogous feature, and relevant startup/configuration code before editing; avoid repository-wide reads unless the change is cross-cutting.
- Preserve existing public routes and response contracts unless a breaking change is requested.
- Keep changes focused and do not modify unrelated files or add dependencies without a concrete need.
- Verify with `dotnet build Pizza-API.slnx --no-restore` when dependencies are already restored. No automated test project currently exists.
- Do not create commits or branches unless asked.
- In the handoff, list modified files and mention migrations, manual steps, or assumptions only when applicable.