# Getting started

## Develop locally

1. Update `SECRET_LANGUAGE_MODEL_API_KEY` environment variable
   - In M365Agent project, open `infra/.env.local.user` file, replace `REPLACE_WITH_YOUR_API_KEY` with your API key.
1. Create a Dev Tunnel
    - Open View menu -> expand Other Windows menu -> Select Dev Tunnels
    - In the Dev Tunnels pane, click **+** icon
    - In the dialog, complete the following fields:
    - Account: Select a connected account
      - Name: (appInternalName)
      - Tunnel Type: Persistent
      - Access: Public
    - Select OK to create the tunnel
1. Start debug session
    - Open Debug menu -> expand the second menu before Start Debugging -> **Select Microsoft Teams (web browser)**
    - Press F5 or select Debug menu -> Start Debugging to start a debugging session
    - Follow the prompts to provision resources in Azure.
      - Provision dialog
        - Select Azure subscription
        - Create a new resource group
        - Select an appropriate region
        - Select OK to continue
      - Azure warning prompt
        - Select OK to acknowledge the warning about the Azure resources being created.
      - Sign in to Azure CLI
        - The first time you run the project, you will be prompted to sign in to Azure CLI. Follow the prompt and sign in with your Azure account. This steps creates the service principal for the Azure Bot Service identity in Microsoft Entra.
      - Azure information prompt
        - Select **View provisioned resources** button, or close the dialog to continue. A browser window opens and navigates to Microsoft Teams.
    - Install and test in Microsoft Teams
      - In the install dialog, select the **Install** button to install the app.
      - In the confirmation dialog, select **Open with Copilot**
      - Wait for the agent to be loaded in the browser. Send a message to the agent to test it.

When starting a debugging session for the first time, the following resources are created:

- Microsoft Entra app registration for the Azure AI Bot Service
  - Client secret is generated and stored as encrypted value in the `.env.local.user` file
  - Service principal created
- Microsoft Entra app registration for API OAuth connection
  - Configured for single sign-on (SSO)
  - Client secret is generated and stored as encrypted value in the `.env.local.user` file 
  - User.Read permission is granted for API
- Azure AI Bot Service resource
  - Configured with the messaging endpoint using the active Dev Tunnel URL
  - Configured with Microsoft Teams channel
  - Configured with API OAuth connection

Conversation state is stored in an emulated Azure Storage Account on your local machine, using the Azurite service which is built into Visual Studio 2022. To manage conversation state, use [Azure Storage Explorer](https://azure.microsoft.com/en-gb/products/storage/storage-explorer/).

See [m365agents.local.yml](./M365Agent/m365agents.local.yml) for more details.

## Provision to the cloud

1. Update `SECRET_LANGUAGE_MODEL_API_KEY` environment variable
   - In M365Agent project, open `infra/.env.dev.user` file, replace `REPLACE_WITH_YOUR_API_KEY` with your API key.
1. Open project menu -> expand Microsoft 365 Agents Toolkit menu -> select **Provision in the Cloud...**
    - In the Provision dialog, complete the fields and select OK.
    - In the Azure warning prompt, select OK to acknowledge the warning about the Azure resources being created.
    - In the Azure information prompt, select **View provisioned resources** button, or close the dialog to continue.
1. Open project menu -> expand Microsoft 365 Agents Toolkit menu -> select **Deploy to the Cloud...** 
    - In the warning prompt, select Deploy to deploy the app code to the provisioned Azure App Service.
1. Open project menu -> expand Microsoft 365 Agents Toolkit menu -> expand **Zip App Package** menu -> select **For Azure**
    - In the file explorer, select **manifest.json** file and select **Open**.

After the app package is created, you can sideload it via your Microsoft Teams client, or deploy it to your organization through the Microsoft 365 Admin Center.

- **Sideload**, open Microsoft Teams, select Apps in the left navigation, select **Manage your apps** in the bottom left corner, select **Upload a customised app**. In the file explorer, select the app package you created and select open. In the install dialog, select **Install** to install the app, and then select **Open with Copilot** in the confirmation dialog.
- **Deploy**, navigate to the [Agents](https://admin.microsoft.com/#/copilot/agents) section in the Microsoft 365 Admin Center, select **Upload custom agent** and upload the app package you created. Follow the steps to complete the deployment. After the agent is deployed, it will be available in Microsoft 365 Copilot.

When provisioning resources to Azure, the following resources are created and configured:

- User Assigned Managed Identity
  - Used as identity for the Azure AI Bot Service
  - Assigned RBAC roles for Key Vault, Storage, and monitoring access
- App Service Plan
  - Hosting plan for the Azure App Service
- Azure App Service
  - Configured with all required application settings
  - Uses managed identity to access Azure Key Vault
  - Integrated with Log Analytics for diagnostics
- Azure Bot Service resource
  - Configured with the messaging endpoint using the Azure App Service URL
  - Configured with Microsoft Teams channel
  - Configured with API OAuth connection
  - Configured with Application Insights
- Azure Key Vault
  - RBAC-enabled with managed identity assigned Key Vault Secrets User role
  - Integrated with Log Analytics for audit logging
- Microsoft Entra app registration for API OAuth connection
  - Configured for single sign-on (SSO)
  - Client secret is generated and stored in Azure Key Vault
  - User.Read permission is granted for API
- Azure Storage Account
  - Managed identity assigned Storage Blob Data Contributor role
  - Integrated with Log Analytics for diagnostics and audit logging
- Log Analytics Workspace
  - Managed identity assigned Log Analytics Contributor role
- Application Insights
  - Managed identity assigned Monitoring Metrics Publisher role
  - Workspace-based integration with Log Analytics

The bot code uses managed identity to read and write conversation state to the Azure Storage Account and record insights in Application Insights when deployed to Azure.

See [m365agents.yml](./M365Agent/m365agents.yml) for more details.