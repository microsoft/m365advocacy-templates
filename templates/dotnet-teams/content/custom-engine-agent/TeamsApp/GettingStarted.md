# Welcome to Teams Toolkit!

## Prerequisites

- Microsoft 365 tenant with [custom app upload](https://learn.microsoft.com/microsoftteams/platform/concepts/build-and-test/prepare-your-o365-tenant#enable-custom-teams-apps-and-turn-on-custom-app-uploading) enabled
- Azure subscription
- _Enable Multi-Project Launch Profiles_ preview feature enabled, **Tools > Options** </br>![image](https://raw.githubusercontent.com/microsoft/m365advocacy-templates/main/templates/dotnet-teams/content/assets/multi-project-feature.png)

## Quick Start

1. In the **debug** dropdown menu, select **Dev Tunnels > Create A Tunnel** (set authentication type to Public) or select an existing public dev tunnel
</br>![image](https://raw.githubusercontent.com/OfficeDev/TeamsFx/dev/docs/images/visualstudio/debug/create-devtunnel-button.png)
1. Right-click the **TeamsApp** project in Solution Explorer and select **Teams Toolkit > Prepare Teams App Dependencies** </br>![image](https://raw.githubusercontent.com/microsoft/m365advocacy-templates/main/templates/dotnet-teams/content/assets/teams-toolkit-menu.png)
1. Sign in to Visual Studio with a **Microsoft 365 work or school account** and select **Continue** </br>![image](https://raw.githubusercontent.com/microsoft/m365advocacy-templates/main/templates/dotnet-teams/content/assets/m365-account.png)
1. Sign in to Azure with an account that has access to an Azure subscription and select an active Azure subscription in the dropdown </br>![image](https://raw.githubusercontent.com/microsoft/m365advocacy-templates/main/templates/dotnet-teams/content/assets/provision-empty.png)
1. Select **New...** and create a new resource group </br>![image](https://raw.githubusercontent.com/microsoft/m365advocacy-templates/main/templates/dotnet-teams/content/assets/provision-new-rg.png)
1. Select a region and select **Provision** to continue </br>![image](https://raw.githubusercontent.com/microsoft/m365advocacy-templates/main/templates/dotnet-teams/content/assets/provision-complete.png)
1. In the warning prompt, select **Provision** to provision resources to the resource group </br>![image](https://raw.githubusercontent.com/microsoft/m365advocacy-templates/main/templates/dotnet-teams/content/assets/arm-deploy-warning.png)
1. Select **X** to close the success prompt </br>![image](https://raw.githubusercontent.com/microsoft/m365advocacy-templates/main/templates/dotnet-teams/content/assets/provision-success.png)
1. Change the selected debug profile to **Microsoft Teams (web browser)**
1. Press **F5**, or select **Debug > Start Debugging menu** in Visual Studio to start your app
</br>![image](https://raw.githubusercontent.com/OfficeDev/TeamsFx/dev/docs/images/visualstudio/debug/debug-button.png)
1. In the opened web browser, select **Add** button to test the app in Teams </br>![image](https://raw.githubusercontent.com/microsoft/m365advocacy-templates/main/templates/dotnet-teams/content/assets/app-install.png)
1. In the compose box, enter **Hi** and send the message. The bot returns a response.

## Run the app on other platforms

The Teams app can run in other platforms like Outlook and Microsoft 365 app. See https://aka.ms/vs-ttk-debug-multi-profiles for more details.

## Get more info

New to Teams app development or Teams Toolkit? Explore Teams app manifests, cloud deployment, and much more in the https://aka.ms/teams-toolkit-vs-docs.

## Report an issue

Select Visual Studio > Help > Send Feedback > Report a Problem.
Or, create an issue directly in our GitHub repository:
https://github.com/OfficeDev/TeamsFx/issues
