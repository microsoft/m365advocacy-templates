# Microsoft 365 Copilot templates

## Setup

```bash
# install the templates
dotnet new install M365Advocacy.M365Copilot.Templates

# uninstall templates
dotnet new uninstall M365Advocacy.M365Copilot.Templates
```

## Custom engine agent project generator

Create a new [custom engine agent](https://learn.microsoft.com/MicrosoftTeams/platform/overview) project to extend Microsoft 365 Copilot with your own agent that uses your own language model.

The template supports any language model that can be accessed via an OpenAI compatible endpoint. The project uses [OpenAI client library for .NET](https://github.com/openai/openai-dotnet) to interact with the language model. If an Azure OpenAI Service endpoint is provided during setup, it will use the [Azure OpenAI client library for .NET](https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/openai/Azure.AI.OpenAI/README.md) instead. Both libraries are included to provide flexibility, you may want to use a different provider during local development, i.e. GitHub Models, and then switch to Azure OpenAI Service for production.

### Prerequisites

- [Visual Studio 2022 17.14](https://visualstudio.microsoft.com/downloads/) (any edition)
- [Microsoft 365 Agents Toolkit workload component](https://learn.microsoft.com/microsoftteams/platform/toolkit/toolkit-v4/install-teams-toolkit-vs#install-teams-toolkit-for-visual-studio)
- Azure subscription
- [Azure CLI](https://aka.ms/azure-cli)
- [.NET 9.0](https://dotnet.microsoft.com/download/dotnet/9.0)

### Usage

```bash
# create a new project
dotnet new cea [options] [template options]

# view template options
dotnet new cea -?
```

### Examples

#### GitHub Models

GitHub Models is a free service that provides access to open source language models. You can use it to create a custom engine agent that uses a model hosted on GitHub.

```pwsh
dotnet new cea `
    --name "MyAgent" `
    --internal-name "my-agent" `
    --display-name "My agent" `
    --language-model-name "openai/gpt-4-mini" `
    --language-model-endpoint "https://models.github.ai/inference" `
    --allow-scripts Yes
```

See [GettingStarted.md](./content/custom-engine-agent/GettingStarted.md) for next steps.

#### Azure OpenAI Service

> [!NOTE]
> Replace `<deployment-name>` with the name of your Azure OpenAI model deployment and `<azure-resource-name>` with the name of your Azure OpenAI resource.

```pwsh
dotnet new cea `
    --name "MyAgent" `
    --internal-name "my-agent" `
    --display-name "My agent" `
    --language-model-name "<deployment-name>" `
    --language-model-endpoint "https://<azure-resource-name>.cognitiveservices.azure.com" `
    --allow-scripts Yes
```

See [GettingStarted.md](./content/custom-engine-agent/GettingStarted.md) for next steps.

#### OpenAI

```pwsh
dotnet new cea `
    --name "MyAgent" `
    --internal-name "my-agent" `
    --display-name "My agent" `
    --language-model-name "gpt-4.1" `
    --language-model-endpoint "https://api.openai.com" `
    --allow-scripts Yes
```

See [GettingStarted.md](./content/custom-engine-agent/GettingStarted.md) for next steps.
