# sample-project-with-aspire
Just a sample project setup with third party integration and aspire

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) or [Podman](https://podman.io/)

Aspire will automatically start all dependencies (PostgreSQL, migrations, the third-party service, API, and web frontend) when the app host runs.

## Running the project

### Aspire CLI

Install the Aspire CLI if you haven't already:

```bash
dotnet tool install -g aspire
```

Then from the repository root:

```bash
aspire run
```

### dotnet CLI

Run the app host project directly:

```bash
dotnet run --project AspireSampleApp.AppHost
```

### Visual Studio Code

Open the repository in VS Code and press **F5**. This uses the pre-configured launch profile (`Aspire: Launch default apphost`) defined in `.vscode/launch.json`.

### Visual Studio

Open `AspireSampleApp.slnx`, set `AspireSampleApp.AppHost` as the startup project, and press **F5** or use the run button as normal.

## Podman

If you are using Podman instead of Docker, set the `ASPIRE_CONTAINER_RUNTIME` environment variable before running:

```bash
export ASPIRE_CONTAINER_RUNTIME=podman
```
