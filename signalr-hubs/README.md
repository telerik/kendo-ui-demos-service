# ASP.NET Core SignalR hubs

SignalR back-end for the Kendo UI demos, providing product and meeting hubs. The application targets .NET 8 and uses Entity Framework Core with a bundled SQLite database.

## Prerequisites

- .NET 8 SDK to restore, build, run, and publish the application.
- A trusted HTTPS development certificate for local development.

## Run locally

From the repository root, run in Bash:

```bash
cd ./signalr-hubs/signalr-for-aspnet-core
dotnet restore
dotnet dev-certs https --trust
dotnet run --launch-profile signalr_for_aspnet_core
```

The project launch profile in [launchSettings.json](signalr-for-aspnet-core/Properties/launchSettings.json) uses `https://localhost:5001` and `http://localhost:5000`. HTTP requests are redirected to HTTPS, so use the HTTPS address for SignalR connections.

Open `https://localhost:5001/health` to check that the application is running. A healthy response is `Healthy`. This is a back-end service; no home page is mapped at `/`.

## Endpoints

| Path | Type | Purpose |
| --- | --- | --- |
| `/products` | SignalR hub | Product demo data, implemented by [ProductHub](signalr-for-aspnet-core/Hubs/ProductHub.cs). |
| `/meetings` | SignalR hub | Meeting demo data, implemented by [MeetingHub](signalr-for-aspnet-core/Hubs/MeetingHub.cs). |
| `/health` | HTTP GET | Application liveness check. It does not check database availability. |

Connect to the hubs with an ASP.NET Core SignalR client. Both hubs expose these methods:

| Method | Behavior |
| --- | --- |
| `Read` | Returns records loaded from the sample database. |
| `Create` | Assigns an ID and creation timestamp, returns the new record, and broadcasts a `create` event. |
| `Update` | Broadcasts an `update` event with the supplied record. |
| `Destroy` | Broadcasts a `destroy` event with the supplied record. |

JSON payloads retain the model's PascalCase property names, such as `ID`, `ProductName`, and `Title`.

Connections are grouped by the remote IP address seen by the server. Change events are sent only to other connections in the same group, not to the initiating connection. Behind a reverse proxy, the remote address may be the proxy's address, so multiple clients can share a group.

## Data behavior

Both hubs read from the bundled [sample.db](signalr-for-aspnet-core/wwwroot/App_Data/sample.db) database at `wwwroot/App_Data/sample.db`. No external database server or connection-string setup is required.

Create, update, and delete operations broadcast changes but do not persist them to the database or maintain a shared in-memory data store. A subsequent `Read` reloads the database records rather than retaining those changes.

Preloaded meetings receive fresh GUIDs on every `Read`, rather than IDs derived from their database `MeetingID`. Independently loaded copies of the same meeting therefore have different IDs across clients or reloads. Newly created meetings also receive new GUIDs.

## Publish and deploy

1. From the project directory used above, publish a Release build:

   ```bash
   dotnet publish --framework net8.0 --configuration Release
   ```

   This command also restores dependencies.

2. Deploy the complete contents of the following directory, relative to the repository root:

   ```text
   signalr-hubs/signalr-for-aspnet-core/bin/Release/net8.0/publish
   ```

   Keep `wwwroot/App_Data/sample.db` in the deployed output; the hubs require it to read sample data.

3. This is a framework-dependent deployment. Install the .NET 8 ASP.NET Core Runtime on the target machine. For IIS hosting, install the .NET 8 Hosting Bundle.

4. Configure the hosting environment, including HTTPS bindings. For direct hosting, run the application from the deployed directory:

   ```bash
   dotnet ./signalr-for-aspnet-core.dll
   ```

   The development launch profile is not used by the published application. Configure production URLs and certificates through the hosting environment, and verify the deployment at `/health`.
