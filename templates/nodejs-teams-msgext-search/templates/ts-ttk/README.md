# Search-based Message Extension

This app template is a search-based [message extension](https://docs.microsoft.com/microsoftteams/platform/messaging-extensions/what-are-messaging-extensions?tabs=nodejs) that allows users to search an external system and share results through the compose message area of the Microsoft Teams client. You can now build and run your search-based message extensions in Teams, Outlook for Windows desktop and web experiences.

This project is pre-configured with:

- Azure AI Bot Service
- Teams Single Sign-On (SSO)

## Prerequisites

To run the template in your local dev machine, you will need:

- [Node.js](https://nodejs.org/), supported versions: 16, 18
- [Teams Toolkit Visual Studio Code Extension](https://aka.ms/teams-toolkit) version 5.0.0 and higher or [Teams Toolkit CLI](https://aka.ms/teamsfx-toolkit-cli)
- An Azure subscription
- [Azure Account Visual Studio Code extension](https://marketplace.visualstudio.com/items?itemName=ms-vscode.azure-account) signed in to your Azure account

## Minimal path to awesome

- Press F5
- If prompted, sign in with your Microsoft 365 account
- Select your Azure subscription
- Select an existing resource group, or create a new one
- Wait for the deployment to finish and a new browser window will open with the app running in Teams
- In the app install dialog, select Add
- In the compose message area, select `+` to open the app flyout menu
- Select your message extension from the list
- Enter text into the search box
- When the search results appear, select the result to send it to the compose message area
