# Microsoft Graph connector

This project contains a [Microsoft Graph connector](https://learn.microsoft.com/graph/connecting-external-content-connectors-overview). Using Graph connectors you can import external data into Microsoft 365 and Microsoft Search. Ingested content is available to Microsoft Copilot for Microsoft 365, Microsoft Search and other Microsoft 365 services.

When developing the Graph connector, you can use [Dev Proxy](https://aka.ms/devproxy) to simulate Microsoft Graph API responses. This allows you to develop and test the connector without having to connect to the Microsoft Graph API and wait for the connection to be provisioned/removed.

## Prerequisites

- [Microsoft 365 Developer tenant](https://developer.microsoft.com/microsoft-365/dev-program)
- [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Microsoft Graph PowerShell SDK](https://learn.microsoft.com/powershell/microsoftgraph/installation?view=graph-powershell-1.0)
- Microsoft PowerShell

## Minimal path to awesome

- Create a new Microsoft Entra app registration (uses [Microsoft Graph PowerShell SDK](https://aka.ms/cli-m365)) by running `./setup.ps1`.
- Complete project
  - In `ConnectionConfiguration.cs`:
    - Complete schema definition
    - Define URL to item resolvers (optional but recommended)
  - In `ContentService.cs`:
    - Update the `Document` class to match the schema you defined previously
    - Implement the `Extract` method to load the content to import
    - Implement the `GetDocId` method to generate a unique ID for each document
    - Update the `Transform` method that convert documents from the external system into Microsoft Graph external items, with the schema you defined previously
  - In `resultLayout.json`:
    - Update the Adaptive Card layout to match the schema you defined previously
- Build the project: `dotnet build`
- Create connection: `dotnet run -- create-connection` (this will take several minutes)
- Load content: `dotnet run -- load-content`

If you want to simulate Microsoft Graph API responses using Dev Proxy:

- [Install Dev Proxy](https://learn.microsoft.com/microsoft-cloud/dev/dev-proxy/get-started)
- Open terminal
  - Change the working directory to the project root
  - Start Dev Proxy: `devproxy`
- Open a new terminal
  - Use the project as described above
- When finished, stop Dev Proxy:
  - In the terminal where Dev Proxy is running, press `Ctrl+C` to stop Dev Proxy

## Project structure

```text
.
├── .devproxy
│   └── graph-connector-mocks.json
├── .vscode
│   ├── extensions.json
│   ├── launch.json
│   └── tasks.json
├── CompleteJobWithDelayHandler.cs
├── ConnectionConfiguration.cs
├── ConnectionService.cs
├── ContentService.cs
├── DebugRequestHandler.cs
├── DebugResponseHandler.cs
├── GraphService.cs
├── MsGraphDocs.csproj
├── Program.cs
├── devproxyrc.json
├── resultLayout.json
└── setup.ps1
```

### .devproxy/graph-connector-mocks.json

Dev Proxy Microsoft Graph API mocks for the Graph connector.

### .vscode

Visual Studio Code configuration files.

### CompleteJobWithDelayMiddleware.cs

Microsoft Graph SDK Middleware to handle the long-running job of provisioning the connection's schema. The middleware waits for the job to complete before continuing. It waits the recommended 60 seconds before checking the job status.

### ConnectionConfiguration.cs

Graph connector configuration. You can define the schema for the external items, the URL to the item resolvers, and the connection name.

### ConnectionService.cs

Code to create the external connection and provision the schema.

### ContentService.cs

Code to load content to import. The code is organized following the Extract-Transform-Load (ETL) principle.

### DebugRequestHandler.cs/DebugResponseHandler.cs

Microsoft Graph SDK Middleware to log requests and responses to the console. Disabled by default. Enable in the **GraphService.cs** file.

### GraphService.cs

Microsoft Graph SDK client instantiation code.

### Program.cs

Main entry point that defines the commands to create the connection and load content.

### devproxyrc.json

Dev Proxy configuration for use with the project.

### resultLayout.json

Result layout (Adaptive Card) for Microsoft Search.

### setup.ps1

Setup script to create a new Microsoft Entra app registration using Microsoft Graph PowerShell SDK. You need to install Microsoft Graph PowerShell SDK before using this script.
