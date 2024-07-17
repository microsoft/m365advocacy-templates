# Microsoft Graph connector project generator

Create a new [Microsoft Graph connector](https://learn.microsoft.com/graph/connecting-external-content-connectors-overview) project with a single command.

Using Graph connectors you can import external data into Microsoft 365 and Microsoft Search. Ingested content is available to Microsoft Copilot for Microsoft 365, Microsoft Search and other Microsoft 365 services.

## Usage

```bash
# install the template
dotnet new install M365Advocacy.GraphConnectors.Templates
# create a new Graph connector project
dotnet new graph-connector [options]
```

### Options

Option|Description|Default value
-|-|-
`connectionId`|This ID must be unique in your Microsoft 365 tenant and be between 3 and 32 characters long.|`graphconnector`
`connectorName`|Microsoft 365 uses this name in the list of connectors in the Microsoft 365 admin center. The setup script also uses this name to create a new Microsoft Entra app registration.|`Graph connector`
`connectorDescription`|This description helps Microsoft 365 administrators understand the purpose of your Graph connector in the Microsoft 365 admin center.|`Imports data from Contoso app`
