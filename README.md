# Pizza Shop API

Pizza Shop API is the ASP.NET Core backend for the Pizza Shop full-stack restaurant portfolio project. It supports the customer ordering flow, registered accounts, guest checkout, order tracking, and an admin-facing workflow for managing the menu and processing orders.

This repository contains the backend only. The companion [React frontend](https://github.com/Vetelae/pizza-react) owns the browser UI, routing, client state, and API consumption. This API owns business rules, persistence, authentication, transactional email, real-time hubs, and server-side validation.

The application models cash and card payment choices, but it does not integrate with a real payment provider.

## What the application does

Customers can browse menu categories, menu items, and news; maintain a cart; and place pickup or delivery orders. A cart can belong either to a registered customer or to an unauthenticated guest session. Registered customers can view their own order history, while guests receive a lookup token for accessing and subscribing to updates for a specific order.

Administrators can manage categories, menu items, images, and news, move orders through a controlled status workflow, search completed and cancelled orders, and view daily operational KPIs.

## Main features

- Public menu, category, and news endpoints
- Guest and registered-customer carts with snapshotted item prices
- Pickup and delivery checkout
- Order status workflow: pending, confirmed, preparing, ready, completed, or cancelled
- User profiles and user-scoped order history
- Token-protected guest order lookup
- Admin CRUD operations and image uploads for menu content
- Paginated and searchable admin order history
- Daily order, revenue, average-order-value, and preparation-time KPIs
- Real-time admin and customer order updates with SignalR
- Registration, email confirmation, login, password reset, logout, and refresh-token rotation
- Liveness and PostgreSQL readiness checks
- Development-only OpenAPI documentation and Scalar API UI

## Backend tech stack

- .NET 10 and ASP.NET Core Web API
- Entity Framework Core 10
- PostgreSQL with Npgsql
- ASP.NET Core Identity
- JWT bearer authentication and persisted refresh tokens
- SignalR
- ASP.NET Core rate limiting and output caching
- Resend for transactional email
- OpenAPI and Scalar
- DataAnnotations and `ProblemDetails` error responses

## Architecture overview

The API follows a straightforward controller-service structure:

```text
HTTP request
    -> Controller (routing, authorization, request context)
    -> Feature service (business rules, persistence, DTO mapping)
    -> ApplicationDbContext
    -> PostgreSQL
```

Controllers are separated into public, authenticated-user, and administrator areas. Business logic stays in feature services, while API and SignalR contracts use DTOs rather than exposing EF Core entities. Expected validation, conflict, and not-found failures are translated into consistent `ProblemDetails` responses by a global exception handler.

## Authentication and authorization

ASP.NET Core Identity manages users, password hashing, email-confirmation tokens, and password-reset tokens. Successful authentication returns a JWT access token and a cryptographically generated refresh token. Refresh tokens are stored in PostgreSQL and rotated or revoked during refresh and logout operations.

- Administrator controllers and the admin order hub require the `Admin` role.
- Registered-customer profile and order endpoints require authentication and scope queries to the JWT user ID.
- Guest order access requires both the order ID and its lookup token.
- The customer SignalR hub accepts authenticated registered customers and token-authorized guest subscriptions.
- Login attempts use IP throttling plus escalating account lockouts.
- Forgot-password responses avoid revealing whether an account exists and apply an account-level email cooldown.

Password rules require uppercase and lowercase letters, a digit, a non-alphanumeric character, and at least eight characters. Registered users must confirm their email before signing in.

## Database and Entity Framework Core

`ApplicationDbContext` extends Identity's EF Core context and contains the application entities for users, categories, menu items, carts, orders, order items, news, and refresh tokens.

Entity configuration is kept in `Configurations/` and applied automatically. The model includes explicit relationships and delete behavior, decimal precision for monetary values, field-length constraints, unique refresh-token indexing, cart and order indexes, and PostgreSQL snake_case naming. Read-only queries use projections and `AsNoTracking()` where appropriate.

Operational timestamps are stored in UTC. Date-based order history and dashboard reporting convert configured business-local dates into UTC query ranges through `TimeProvider` and `Business:TimeZoneId`.

## Rate limiting

The API uses configurable sliding-window limiters with no request queue:

- A global per-IP limit
- Separate policies for public reads, cart reads, cart mutations, and checkout
- Per-IP policies for registration, login, email confirmation, password reset, token refresh, and logout
- User-based cart partitions for registered customers and IP-based partitions for guests

Rejected requests return HTTP `429`, include a `Retry-After` header, and use an authentication response or `ProblemDetails` body depending on the endpoint. Forwarded client addresses are trusted only when explicitly configured through the reverse-proxy settings.

## Output caching

Public category, menu-item, and news reads use a five-minute output-cache policy. Entries are tagged by feature and evicted after successful admin create, update, image, or delete operations so cached data does not remain stale after a mutation.

## Other technical decisions

- SignalR notifications are published only after database changes have been persisted.
- Order item prices are copied into cart and order rows instead of being recalculated from later menu prices.
- Order status transitions are validated in the service layer and record milestone timestamps.
- Category and menu images are validated by content type and size, stored under `wwwroot/uploads`, and excluded from Git.
- Request-shape validation uses DataAnnotations, with `IValidatableObject` for rules such as requiring an address for delivery.
- Enums are serialized as readable strings.
- CORS origins are configuration-driven for the separate React client.
- `/health` is a liveness endpoint and `/health/ready` verifies database readiness.
- Seed categories and default images are created when the application starts. Roles are seeded in all environments, while the fixed demo administrator is development-only.

## Project structure

```text
Configurations/       EF Core entity configurations
Constants/            Validation, cache, and rate-limit policy names and limits
Controllers/
  Admin/              Admin-only management endpoints
  Public/             Public catalog, cart, order, and authentication endpoints
  User/               Authenticated profile and order endpoints
Data/                 DbContext and development seed logic
Entities/             EF Core entities
Entities/Dtos/        Request and response contracts grouped by feature
Enums/                Order, payment, and reporting enums
Exceptions/           Application exceptions and global exception handling
Helpers/              Mapping, cart identity, and timezone helpers
Hubs/                 Strongly typed SignalR hubs and client contracts
Migrations/           EF Core migrations and model snapshot
Options/              Strongly typed configuration options
Services/             Feature interfaces and implementations
wwwroot/seeds/         Version-controlled default images
```

## Local setup

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- PostgreSQL
- EF Core CLI tools (`dotnet tool install --global dotnet-ef`) if not already installed
- A Resend account and verified sender when testing email flows

### 1. Clone and restore

```bash
git clone https://github.com/Vetelae/Pizza-API.git
cd Pizza-API
dotnet restore
```

### 2. Create development configuration

Create an ignored `appsettings.Development.json` file. The following example contains placeholders only:

```json
{
  "allowedOrigins": [
    "http://localhost:5173"
  ],
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=YOUR_DATABASE;Username=YOUR_USERNAME;Password=YOUR_PASSWORD"
  },
  "JwtSettings": {
    "Key": "GENERATE_A_LONG_RANDOM_SIGNING_KEY",
    "Issuer": "YOUR_ISSUER",
    "Audience": "YOUR_AUDIENCE",
    "DurationInMinutes": 15,
    "RefreshTokenValidityMins": 10080
  },
  "Resend": {
    "ApiKey": "YOUR_RESEND_API_KEY",
    "FromEmail": "YOUR_VERIFIED_SENDER_EMAIL",
    "FromName": "Pizza Shop"
  },
  "Frontend": {
    "Url": "http://localhost:5173"
  }
}
```

The same settings can be supplied as environment variables by replacing `:` with `__`, for example:

```text
ConnectionStrings__DefaultConnection
JwtSettings__Key
JwtSettings__Issuer
JwtSettings__Audience
JwtSettings__DurationInMinutes
JwtSettings__RefreshTokenValidityMins
Resend__ApiKey
Resend__FromEmail
Resend__FromName
Frontend__Url
allowedOrigins__0
```

`appsettings.json` already contains non-secret defaults for logging, the business timezone, rate limits, and reverse-proxy behavior. Override those settings per environment when needed.

### 3. Create the database

Create an empty PostgreSQL database matching your connection string, then apply the included migrations:

```bash
dotnet ef database update
```

The application seeds its roles, default categories, and default images at startup.

### 4. Configure email with Resend

Create a Resend API key and verify the sender address you place in `Resend:FromEmail`. `Frontend:Url` is used to build email-confirmation and password-reset links, so it should point to the running React client.

If Resend is not configured, catalog and ordering functionality can still be explored, but registration confirmation and password-reset email flows will not be complete.

### 5. Run the API

```bash
dotnet run
```

The development launch profile uses:

- `https://localhost:7205`
- `http://localhost:5193`
- Scalar UI: `https://localhost:7205/scalar/v1`
- Liveness: `https://localhost:7205/health`
- Database readiness: `https://localhost:7205/health/ready`

## Development demo account

In the Development environment only, `DbInitializer` creates a demo administrator if it does not already exist. Its credentials are defined in `Data/DbInitializer.cs` for local testing and are not intended for production use. No demo administrator is created outside Development.

No public live deployment or production demo account is currently provided.

## Related repository

- [Pizza Shop frontend — React application](https://github.com/Vetelae/pizza-react)
