# kendo-ui-demos-service

Back-end services used by the [Kendo UI Online Demos](https://demos.telerik.com/kendo-ui/).

The repository contains four independently hosted ASP.NET Core applications, all targeting .NET 8. They provide HTTP CRUD, OData v4, GraphQL, and SignalR examples backed by bundled SQLite sample databases.

## Projects

| Service | Project directory | Purpose |
| --- | --- | --- |
| CRUD demo service | [demos-core/KendoCRUDService/KendoCRUDService](demos-core/KendoCRUDService/KendoCRUDService) | HTTP endpoints for demo data, uploads, and exports, plus product and meeting SignalR hubs. |
| OData v4 | [odata-v4-core/kendo-northwind-pg/kendo-northwind-pg](odata-v4-core/kendo-northwind-pg/kendo-northwind-pg) | Northwind-style entity sets with OData queries and CRUD operations. |
| GraphQL | [graphql/graphql-aspnet-core/graphql-aspnet-core](graphql/graphql-aspnet-core/graphql-aspnet-core) | Product queries and mutations, plus customer-data and server-operation examples. |
| Standalone SignalR | [signalr-hubs/signalr-for-aspnet-core](signalr-hubs/signalr-for-aspnet-core) | Real-time product and meeting hubs. |

Each application has its own project and solution. There is no repository-level solution; select the application you want to run, build, or publish.

## Prerequisites

- .NET 8 SDK.
- For the GraphQL project, access to the authenticated Telerik NuGet feed to restore its `Telerik.UI.for.AspNet.Core` dependency. Follow the [Telerik NuGet setup instructions](https://www.telerik.com/aspnet-core-ui/documentation/installation/installation-options/nuget-install), and keep credentials outside the repository.
- For local HTTPS, create and trust a development certificate:

  ```bash
  dotnet dev-certs https --trust
  ```

## Run locally

The commands below use Bash syntax. From the repository root, start the CRUD demo service with:

```bash
dotnet run --project ./demos-core/KendoCRUDService/KendoCRUDService --launch-profile https
```

`dotnet run` restores dependencies and builds the selected project automatically. To run another application, replace the project directory with one from the table above and choose its launch profile:

| Service | Launch profile | HTTPS URL | HTTP URL |
| --- | --- | --- | --- |
| CRUD demo service | `https` | `https://localhost:7171` | `http://localhost:5099` |
| OData v4 | `https` | `https://localhost:7240` | `http://localhost:5259` |
| GraphQL | `graphql_aspnet_core` | Not configured | `http://localhost:5000` |
| Standalone SignalR | `signalr_for_aspnet_core` | `https://localhost:5001` | `http://localhost:5000` |

All listed profiles use the `Development` environment. The CRUD, OData, and standalone SignalR services enable HTTPS redirection; use their HTTPS URLs for requests.

GraphQL and standalone SignalR both bind HTTP port 5000 by default. Run them separately or override one application's URLs. For example, start GraphQL on port 5002:

```bash
dotnet run --project ./graphql/graphql-aspnet-core/graphql-aspnet-core --launch-profile graphql_aspnet_core -- --urls http://localhost:5002
```

Every service exposes `GET /health`. For example, open `https://localhost:7171/health` after starting the CRUD service. A healthy response is `Healthy`; these are application liveness checks, not database or external-service checks.

## Example endpoints

Paths below are relative to the corresponding service's base URL.

| Service | Endpoints |
| --- | --- |
| CRUD demo service | `GET /Products`, `GET /Products/Read?skip=0&take=10`; SignalR hubs at `/signalr/products` and `/signalr/meetings`. |
| OData v4 | `GET /$metadata`, `GET /Products`, and queries such as `GET /Products?$top=10`. Entity routes are at the service root, without an `/odata` prefix. |
| GraphQL | `POST /` with a GraphQL request body. The GraphQL endpoint is at the service root, not `/graphql`. |
| Standalone SignalR | Hubs at `/products` and `/meetings`. Connect with an ASP.NET Core SignalR client rather than treating them as REST endpoints. |

See the [standalone SignalR README](signalr-hubs/README.md) for hub methods, grouping, and data behavior.

## Sample data

The applications use the supplied SQLite databases; no separate database server is required for the sample-data endpoints. Paths below are relative to each project directory.

| Service | Database files |
| --- | --- |
| CRUD demo service and OData v4 | `wwwroot/data/sample.db` |
| GraphQL | `wwwroot/App_Data/sample.db` and `wwwroot/App_Data/customers.db` |
| Standalone SignalR | `wwwroot/App_Data/sample.db` |

Keep the complete `wwwroot` contents with each application, including databases and supporting data files.

- In the `Production` environment, the CRUD and OData services copy their sample database to `tmp/demos-local.db` under the application's content root. This working copy is replaced at each startup, and the application needs write access to that directory.
- GraphQL can seed its customer database at startup, so its customer database and containing directory must be writable.
- Standalone SignalR changes are broadcast to connected clients but are not persisted to the database.

## Publish and deploy

1. Open a terminal in the project directory of the application to deploy, then publish:

   ```bash
   dotnet publish --framework net8.0 --configuration Release
   ```

   The command restores dependencies and creates a framework-dependent Release build.

2. Deploy the complete contents of `bin/Release/net8.0/publish` from that project, including `wwwroot` and its database files.

3. Install the .NET 8 ASP.NET Core Runtime on the target machine. For IIS hosting, install the .NET 8 Hosting Bundle.

4. Configure the hosting environment's URLs, HTTPS certificates where applicable, and required filesystem permissions. Development launch profiles are not used by published applications. Verify each deployed service through its `/health` endpoint.
