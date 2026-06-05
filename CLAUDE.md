# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project purpose

DAIKON (Data Acquisition, Integration, and Knowledge Capture) is an open-source web platform for **target-based drug discovery**. It tracks the lifecycle from genes/targets through screens, hit assessments, and projects, with knowledge capture (comments, questionnaires, documents) layered across. This repository is the **backend** (a separate frontend React app consumes it via JSON over the gateway). The platform is referenced in *ACS Pharmacol Transl Sci. 2023; PMID 37470023*.

All code lives under `src/`. The solution file is `src/daikon.sln`.

## Stack & top-level layout

.NET 8 microservices, MongoDB (event store + read models), Kafka (async eventing), Neo4j (Horizon only), Docker Compose for local orchestration.

```
src/
├── BuildingBlocks/        # Shared libraries referenced by every service
├── Services/              # 12 domain microservices (one folder each)
├── Identities/UserStore/  # User / org / role service (non-event-sourced)
├── ApiGateways/SimpleGW/  # Single entry point: OIDC auth + path-based routing
├── Aggregators/           # Cross-service read API (composes data via HTTP)
├── Extensions/            # Operator-facing tools (e.g. EventManagement = event replay)
├── docker-compose*.yml    # Local orchestration
├── build-services.sh      # Sequential `docker compose build` (pretty output)
└── daikon.sln
```

## Architecture: CQRS + Event Sourcing

Most domain services follow the same pattern — read it once here, don't re-derive it per service.

**Write path:**
```
API Controller (V1/V2)
  → MediatR command
    → Command handler builds/loads aggregate
      → IEventSourcingHandler<TAggregate>
        → EventStore<TAggregate>  (BuildingBlocks/Daikon.EventStore/Stores/EventStore.cs)
          → 1. Append events to MongoDB event store (with optimistic concurrency check)
          → 2. Produce each event to the configured Kafka topic
```

**Read path:**
```
Kafka consumer hosted service (ConsumerHostedService)
  → Service-specific event handler (e.g. GeneEventHandler_*)
    → Updates MongoDB read model
                ↑
API queries hit the read repository directly (no event replay on read).
```

**Cross-service:**
- Async — Kafka topic per service.
- Sync — HTTP. Configured via `*API:Url` env vars (see `docker-compose.override.yml`). Aggregators-API and several services call MLogix/UserStore/etc. this way.
- All public traffic enters via SimpleGW.

### Per-service convention (the "4 projects")

Every domain service splits into four .NET projects with the same names — e.g. `Services/Gene/`:

| Project | Purpose |
|---|---|
| `<Svc>.API` | ASP.NET Core host. `Program.cs` is uniformly 3 lines: `AddDaikonApiDefaults` + `AddApplicationServices` + `AddInfrastructureService(configuration)`. Controllers under `Controllers/V1/` and `Controllers/V2/`. |
| `<Svc>.Application` | MediatR-based use cases. `Features/Command/<UseCase>/` and `Features/Queries/<UseCase>/`. `EventHandlers/` projects events to read models. `Contracts/Persistence/` defines repo interfaces. `Mappings/` is AutoMapper. `ApplicationServiceRegistration.cs` wires MediatR + FluentValidation + AutoMapper. |
| `<Svc>.Domain` | Aggregates split into partials by sub-area (e.g. `GeneAggregate_Core.cs`, `GeneAggregate_Essentiality.cs`, `GeneAggregate_CrispriStrain.cs` …). Plus `Entities/`. |
| `<Svc>.Infrastructure` | MongoDB repositories under `Query/Repositories/`, Kafka consumer under `Query/Consumers/`. `InfrastructureServiceRegistration.cs` registers Mongo settings, Kafka, the event store, and **explicitly registers BSON class maps for every event the service cares about** (see Gene's for the template). |

A few services have a `<Svc>.Test` project (Gene, Horizon). The platform-wide test command is `dotnet test src/daikon.sln`.

### BuildingBlocks (shared)

| Library | What it provides |
|---|---|
| `CQRS.Core` | `BaseCommand`, `BaseQuery`, `BaseEntity`, `DVariable<T>` (versioned/auditable property primitive), `Middlewares/GlobalErrorHandlingMiddleware`, `Middlewares/RequestorIdBehavior` (MediatR pipeline that pulls user identity off `IHttpContextAccessor`), and `Consumers/IEventConsumer`. |
| `Daikon.ApiHost` | `AddDaikonApiDefaults()` and `UseDaikonApiDefaults()` — **every** service Program.cs uses these. They wire up API versioning (URL segment + `x-api-version` header), lowercased + slugified routes, Swagger, FluentValidation, the global error middleware, and `/health` + `/health/ready` endpoints returning service/version/uptime. |
| `Daikon.EventStore` | Generic `EventStore<TAggregate>`, `EventSourcingHandler<TAggregate>`, `EventStoreRepository`, `SnapshotRepository`, `EventProducer` (Kafka), settings classes. This is the heart of the write path. |
| `Daikon.Events` | POCO event payloads grouped per domain (`Gene/`, `Targets/`, `Screens/`, `Project/`, `HitAssessment/`, `MLogix/`, `Comment/`, `DocuStore/`, `Strains/`). Multiple services depend on this to consume each other's events. |
| `Daikon.Shared` | Cross-cutting DTOs/VMs, `APIClients/` (typed HTTP clients), `Constants/`, `Kernel/`, `Embedded/` resources, `ConstantsVM.cs` (carries `AppVersion`). |
| `Daikon.VersionStore` | (Currently minimal — only `bin`/`obj` populated, project skeleton.) |

### Special-case services (don't fit the 4-project pattern)

- **SimpleGW** (`ApiGateways/SimpleGW/SimpleGW.API`) — Single-project gateway. Pipeline: `RequestTiming` → `AuthenticationValidator` → `UserProfileEnricher` → `MicroServiceRouting` → `RequestForwarding`. OIDC provider is pluggable: `OIDCProviders/MicrosoftEntraID.cs` and `OIDCProviders/KeyCloak.cs`, selected by `OIDCProvider` env (`EntraID` or `KeyCloak`). Routes by **URL path segment** using `EndPointRouting:<segment>` env vars. Also runs a `MicroserviceHealthPoller` background service that exposes `/health/services`.
- **Aggregators.API** — Single-project, not event-sourced. Uses `Daikon.ApiHost` defaults; calls multiple downstream services via HTTP to compose responses.
- **Identities/UserStore** — Has the 4-project layout but is **not event-sourced** (plain MongoDB CRUD).
- **Horizon** — Backed by **Neo4j**, not MongoDB. Consumes Kafka events from every service to build a graph of relationships.
- **Extensions/Daikon.EventManagement** — Operator tool that auto-discovers every `BaseEvent` subclass in the `Daikon.Events` assembly via reflection and replays them. Used when projections need to be rebuilt.

## Common commands

Run from `src/` unless noted.

```bash
# Build everything
dotnet build daikon.sln

# Build / test a single service
dotnet build Services/Gene/Gene.API/Gene.API.csproj
dotnet test  Services/Gene/Gene.Test/Gene.Test.csproj

# Run one test by name
dotnet test Services/Gene/Gene.Test/Gene.Test.csproj --filter "FullyQualifiedName~SomeTest"

# Build all docker images sequentially (pretty output, fails fast)
./build-services.sh
./build-services.sh -- --no-cache          # pass-through args after `--`

# Bring up the full local stack
docker compose -f docker-compose.yml -f docker-compose.override.yml up
docker compose -f docker-compose.yml -f docker-compose-dev.yml up   # hot-reload dev variant
```

The compose stack attaches to an **external** docker network `daikon-be-net` — create it first: `docker network create daikon-be-net`.

### Local environment

The override compose file reads many `${...}` env vars from your shell / `.env`. The important ones:

- `EventDB_ConnectionString` — MongoDB connection string for the event store
- `MongoDB_ConnectionString` — MongoDB connection string for read models / non-ES services
- `KafkaBootstrapServers`, `KafkaConnectionString`, `KafkaSecurityProtocol`, `KafkaTopic`, `KafkaConsumerTopics`
- `HorizonNeo4jURI`, `HorizonNeo4jUser`, `HorizonNeo4jPassword`
- OIDC: `EntraID_*` (Instance, Domain, TenantId, ClientId, Audience) **or** `Keycloak_*`

### Port map (compose override → host)

```
SimpleGW  8010   UserStore  8011   Gene 8001   Target 8002   Screen 8003
HitAssess 8004   Project    8005   Horizon 8006 MLogix 8007  Questionnaire 8008
Comment   8009   EventHist  8012   DocuStore 8014  EventMgmt 8015
UserPrefs 8016   Aggregators 8017
```

External entry is **SimpleGW (8010)**. Don't call the individual services from a browser — they expect headers added by the gateway middleware (auth context, user profile).

## API versioning

Versioned via `Daikon.ApiHost`: URL segment (`/api/v2/...`), `x-api-version` header, or media-type parameter — all three readers are registered. Default is v1.0 when unspecified. Controllers live under `Controllers/V1/` and `Controllers/V2/`. Routes are lowercased and slugified (`SlugifyParameterTransformer`).

## Adding a new microservice

When extending the platform, mirror the existing pattern end-to-end — several configs reference each service by name:

1. Create the 4 projects under `Services/<Name>/` (API, Application, Domain, Infrastructure).
2. `Program.cs` should be the 3-line standard form (see `Services/Gene/Gene.API/Program.cs`).
3. If event-sourced, add the event types to `BuildingBlocks/Daikon.Events/<Name>/` and register their BSON class maps in `<Name>.Infrastructure/InfrastructureServiceRegistration.cs` — events are not auto-discovered per service (only the EventManagement replay tool reflects).
4. Add the project to `src/daikon.sln`.
5. Add a service entry to `docker-compose.yml` (build context + Dockerfile) and `docker-compose.override.yml` (env vars + port + `daikon-be-net`). Add a `Dockerfile.dev` and corresponding `docker-compose-dev.yml` entry if hot-reload is wanted.
6. Add the Dockerfile to `.gitlab-ci.yml`'s `build` and `push` matrices (CI runs only on the `lab-main` branch).
7. Route public traffic by adding `EndPointRouting:<segment>=http://<name>-api` to the `simplegw-api` env block.

Use **Gene** as the reference template — it exercises every pattern (multiple aggregate partials, batch ops, multiple sub-feature event handlers, both controllers V1 and V2).

## CI

`.gitlab-ci.yml` runs on the `lab-main` branch: parallel matrix `docker build` for every service, then parallel `docker push` to `$CI_REGISTRY/sid/daikon/<service>:$CI_COMMIT_REF_NAME`.

There is no lint/test stage in CI — tests are run locally.
