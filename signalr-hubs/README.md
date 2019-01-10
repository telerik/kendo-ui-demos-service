# ASP.NET Core SignalR hub

## Steps do deploy the application:
1. Navigate to the `signalr-for-aspnet-core` folder
```
cd signalr-for-aspnet-core
```
2. Execute:
```
dotnet restore
dotnet publish --framework netcoreapp2.1 --configuration Release
```
**Note:** The specific version depends on the version of ASP.NET Core that the project is targeting. A compatible version should also be installed on the staging machine.

3. The files that need to be deployed are located in the following folder:
```
signalr-for-aspnet-core\bin\Release\netcoreapp2.1\publish
```
