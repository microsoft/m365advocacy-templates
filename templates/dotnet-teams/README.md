# Microsoft Teams templates

- [Custom Engine Agent project generator](#custom-engine-agent-project-generator)
- [Microsoft Teams message extension with search command project generator](#microsoft-teams-message-extension-with-search-command-project-generator)

## Setup

```bash
# install the templates
dotnet new install M365Advocacy.Teams.Templates

# uninstall templates
dotnet new uninstall M365Advocacy.Teams.Templates
```

## Custom Engine Agent project generator

Create a new [Microsoft Teams app](https://learn.microsoft.com/MicrosoftTeams/platform/overview) project with an AI powered bot using a single command.

### Usage

```bash
# create a new project
dotnet new custom-engine-agent [options] [template options]

# view template options
dotnet new custom-engine-agent -?
```

### Example

```pwsh
dotnet new custom-engine-agent --name "Custom.Engine.Agent" `
    --internal-name "custom-engine-agent" `
    --display-name "Custom engine agent" `
    --short-description "Custom engine agent" `
    --full-description "Custom engine agent powered by Teams AI library" `
    --model-name "gpt-4" `
    --model-version "1106-Preview" `
    --allow-scripts Yes
```

## Microsoft Teams message extension with search command project generator

Create a new [Microsoft Teams app](https://learn.microsoft.com/MicrosoftTeams/platform/overview) project with a message extension containing a search command using a single command.

### Usage

```bash
# create a new project
dotnet new teams-msgext-search [options] [template options]

# view template options
dotnet new teams-msgext-search -?
```

### Example

```pwsh
dotnet new teams-msgext-search --name "ProductsPlugin" `
    --internal-name "msgext-products" `
    --display-name "Contoso products" `
    --short-description "Product look up tool." `
    --full-description "Get real-time product information and share them in a conversation." `
    --command-id "Search" `
    --command-description "Find products by name" `
    --command-title "Products" `
    --parameter-name "ProductName" `
    --parameter-title "Product name" `
    --parameter-description "The name of the product as a keyword" `
    --allow-scripts Yes
```
