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
    - Follow the prompts to provision resources in Azure
      - Microsoft 365 account dialog
        - Select Microsoft 365 account and select OK
      - Provision dialog
        - Select Azure subscription
        - Create a new resource group
        - Select an appropriate region
        -  Select OK to continue
      - Azure warning prompt
        - Select OK to acknowledge the warning about the Azure resources being created. An Azure Bot Service is provisioned using the free tier for local development which incurs no cost.
      - Sign in to Azure CLI
        - The first time you run the project, you will be prompted to sign in to Azure CLI. Follow the prompt and sign in with your Azure account. This steps creates the service principal for the Azure Bot Service identity in Microsoft Entra.
      - Azure information prompt
        - Select **View provisioned resources** button, or close the dialog to continue. A browser window opens and navigates to Microsoft Teams.
    - Install and test in Microsoft Teams
      - In the install dialog, select the **Install** button to install the app.
      - In the confirmation dialog, select **Open with Copilot**
      - Wait for the agent to be loaded in the browser. Send a message to the agent to test it.

## Provision to the cloud

1. Update `SECRET_LANGUAGE_MODEL_API_KEY` environment variable
   - In M365Agent project, open `infra/.env.dev.user` file, replace `REPLACE_WITH_YOUR_API_KEY` with your API key.
1. Open project menu -> expand Microsoft 365 Agents Toolkit menu -> select **Provision in the Cloud...**
    - In the Provision dialog, complete the fields and select OK.
    - In the Azure warning prompt, select OK to acknowledge the warning about the Azure resources being created.
    - The following resources are provisioned in Azure:
      - Azure App Service (B1)
      - Azure Bot Service (Free tier)
      - Azure Key Vault
      - Azure Storage Account
      - Application Insights
      - Log Analytics Workspace
      - User Assigned Managed Identity
    - In the Azure information prompt, select **View provisioned resources** button, or close the dialog to continue.
2. Open project menu -> expand Microsoft 365 Agents Toolkit menu -> select **Deploy to the Cloud...** 
    - In the warning prompt, select Deploy to deploy the app code to the provisioned Azure App Service.
3. Open project menu -> expand Microsoft 365 Agents Toolkit menu -> expand **Zip App Package** menu -> select **For Azure**
    - In the file explorer, select **manifest.json** file and select **Open**.

After the app package is created, you can sideload it via your Microsoft Teams client, or deploy it to your organization through the Microsoft 365 Admin Center.

- **Sideload**, open Microsoft Teams, select Apps in the left navigation, select **Manage your apps** in the bottom left corner, select **Upload a customised app**. In the file explorer, select the app package you created and select open. In the install dialog, select **Install** to install the app, and then select **Open with Copilot** in the confirmation dialog.
- **Deploy**, navigate to the [Agents](https://admin.microsoft.com/#/copilot/agents) section in the Microsoft 365 Admin Center, select **Upload custom agent** and upload the app package you created. Follow the steps to complete the deployment. After the agent is deployed, it will be available in Microsoft 365 Copilot.