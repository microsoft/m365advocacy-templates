# Microsoft Teams message extension with search command project generator

Create a new [Microsoft Teams app](https://learn.microsoft.com/MicrosoftTeams/platform/overview) project with a message extension containing a search command using a single command.

## Usage

```bash
# install the templates
dotnet new install M365Advocacy.Teams.Templates
# create a new project
dotnet new teams-msgext-search [options] [template options]
# view template options
dotnet new teams-msgext-search -?
# uninstall templates
dotnet new uninstall M365Advocacy.Teams.Templates
```

## Example

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
