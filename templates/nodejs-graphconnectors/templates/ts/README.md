# Microsoft Graph connector

This project contains a [Microsoft Graph connector](https://learn.microsoft.com/graph/connecting-external-content-connectors-overview). Using Graph connectors you can import external data into Microsoft 365 and Microsoft Search. Ingested content is available to Microsoft Copilot for Microsoft 365, Microsoft Search and other Microsoft 365 services.

When developing the Graph connector, you can use [Dev Proxy](https://aka.ms/devproxy) to simulate Microsoft Graph API responses. This allows you to develop and test the connector without having to connect to the Microsoft Graph API and wait for the connection to be provisioned/removed.

## Minimal path to awesome

- Create a new Microsoft Entra app registration (uses [CLI for Microsoft 365](https://aka.ms/cli-m365)):
  - Bash:
    ```bash
    chmod +x ./setup.sh
    ./setup.sh
  - PowerShell:
    ```powershell
    ./setup.ps1
    ```
- Restore dependencies: `npm install`
- Complete project
  - In `config.ts`:
    - Complete schema definition
    - Define URL to item resolvers (optional but recommended)
  - In `loadContent.ts`:
    - Implement the `loadContent` function to load content to import
    - Implement the `getDocId` function to generate a unique ID for each document
    - Update the `transform` function that convert documents from the external system into Microsoft Graph external items, with the schema you defined previously
- Build project: `npm run build`
- Create connection: `npm run start:createConnection`
- Load content: `npm run start:loadContent`

If you want to simulate Microsoft Graph API responses using Dev Proxy:

- [Install Dev Proxy](https://learn.microsoft.com/microsoft-cloud/dev/dev-proxy/get-started)
- Open terminal
  - Change the working directory to the project root
  - Start Dev Proxy: `devproxy`
- Open a new terminal
  - Change the working directory to the project root
  - Create connection: `npm run start:createConnection:proxy`
  - Load content: `npm run start:loadContent:proxy`
- When finished, stop Dev Proxy:
  - In the terminal where Dev Proxy is running, press `Ctrl+C` to stop Dev Proxy

## Project structure

```text
.
├── .devproxy
│   └── graph-connector-mocks.json
├── .gitignore
├── .vscode
│   ├── extensions.json
│   └── launch.json
├── README.md
├── devproxyrc.json
├── package.json
├── resultLayout.json
├── setup.ps1
├── setup.sh
├── src
│   ├── completeJobWithDelayMiddleware.ts
│   ├── config.ts
│   ├── createConnection.ts
│   ├── graphClient.ts
│   └── loadContent.ts
└── tsconfig.json (TypeScript configuration)
```

### .devproxy/graph-connector-mocks.json

This file contains the Dev Proxy Microsoft Graph API mocks for the Graph connector.

### devproxyrc.json

This file contains the Dev Proxy configuration for use with the project.

### resultLayout.json

This file contains the result layout (Adaptive Card) for Microsoft Search.

### setup.ps1 and setup.sh

These scripts create a new Microsoft Entra app registration using CLI for Microsoft 365. Both scripts invoke CLI using npx, so that you don't need to manually install CLI for Microsoft 365 before you run these scripts.

### src/completeJobWithDelayMiddleware.ts

This file contains the Microsoft Graph SDK Middleware to handle the long-running job of provisioning the connection's schema. The middleware waits for the job to complete before continuing. It waits the recommended 60 seconds before checking the job status.

### src/config.ts

This file contains the Graph connector configuration. You can define the schema for the external items, the URL to the item resolvers, and the connection name.

### src/createConnection.ts

This file contains the code to create the external connection and provision the schema.

### src/graphClient.ts

This file contains the Microsoft Graph SDK client instantiation code.

### src/loadContent.ts

This file contains the code to load content to import. The code is organized following the Extract-Transform-Load (ETL) principle.

### tscconfig.json

This file contains the TypeScript configuration for the project.