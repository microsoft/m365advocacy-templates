# Microsoft Graph connector

This project contains a [Microsoft Graph connector](https://learn.microsoft.com/graph/connecting-external-content-connectors-overview). Using Graph connectors you can import external data into Microsoft 365 and Microsoft Search. Ingested content is available to Microsoft Copilot for Microsoft 365, Microsoft Search and other Microsoft 365 services.

When developing the Graph connector, you can use [Dev Proxy](https://aka.ms/devproxy) to simulate Microsoft Graph API responses. This allows you to develop and test the connector without having to connect to the Microsoft Graph API and wait for the connection to be provisioned/removed.

## Prerequisites

- [Teams Toolkit for Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=TeamsDevApp.ms-teams-vscode-extension)
- [Azure Functions Visual Studio Code extension](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurefunctions)
- [Microsoft 365 Developer tenant](https://developer.microsoft.com/microsoft-365/dev-program) with [uploading custom apps enabled](https://learn.microsoft.com/microsoftteams/platform/m365-apps/prerequisites#prepare-a-developer-tenant-for-testing)
- [Dev Tunnels CLI](https://learn.microsoft.com/azure/developer/dev-tunnels/get-started#install)
- [Dev Proxy](https://aka.ms/devproxy) (optional)

## Minimal path to awesome

### Complete the project

- Restore dependencies: `npm install`
- Complete project
  - In `src/common/config.ts`:
    - Complete schema definition
    - Define URL to item resolvers (optional but recommended)
  - In `src/common/Doc.ts`:
    - Implement the `Doc` interface that represents documents from the external system to import
  - In `src/common/documentClient.ts`:
    - Implement functions to retrieve documents from the external system
  - In `src/common/resultLayout.ts`:
    - Add the result layout (Adaptive Card) for Microsoft Search
  - In `resultLayout.json`:
    - Add the result layout (Adaptive Card) for Microsoft Search

### Run the project

#### Simulated debugging with Dev Proxy

##### 1. Dev Proxy setup

- Open a terminal, change to the project directory
- Run `devproxy` to start Dev Proxy using the preset configuration

##### 2. Start debug session

In Visual Studio Code, open the Teams Toolkit extension:

- On the sidebar, open `Run and Debug` panel and change the dropdown to select the `Debug (Simulated)` profile
- Press <kbd>F5</kbd> to start a simulated debug session to start the Azure Functions host

##### 3. Simulate webhook notification

- Go to the running Dev Proxy process in your terminal, press <kbd>w</kbd> to simulate a webhook notification

#### Debug against a real Microsoft 365 tenant

##### 1. Build project

In Visual Studio Code:

- Press <kbd>F5</kbd>, follow the sign in prompts
- Wait for all tasks to complete

##### 2. Enable Graph connector

- In a web browser, navigate to the [Microsoft Teams Admin Center](https://admin.teams.microsoft.com)
- Open the [Manage apps](https://admin.teams.microsoft.com/policies/manage-apps) section
- In the table displaying `All apps`, search for `{{connectorName}}-local`
- Select the app in the table to open the app details page
- Select `Publish` and confirm the prompt. You will been taken back to the `All apps` page and a confirmation banner will be displayed
- Search for `{{connectorName}}-local` and open the app details page
- Select the `Graph Connector` tab
- A banner will be displayed. Click `Grant permissions`, this will open a permissions consent page in a pop-up window. Confirm the permissions. This will automatically toggle the connection status to on and start the setup process which includes:
  - creating an external connection
  - provisioning the schema
  - importing external content

The process will take several minutes in total. During this time you may see an error message on this page, however this can be ignored and you can refresh the page to check on the status.

> TIP: To monitor the activity, in Visual Studio Code, check out the output of the `func: host start` task. You'll see the status of the different activities as they are completed.

When the process is complete you will see a table confirming that the connection has been successful.

##### 3. Include data in results

- In the web browser navigate to the [Microsoft 365 admin center](https://admin.microsoft.com/)
- From the side navigation, open [Settings > Search & Intelligence](https://admin.microsoft.com/?source=applauncher#/MicrosoftSearch)
- On the page, navigate to the [Data Sources](https://admin.microsoft.com/?source=applauncher#/MicrosoftSearch/connectors) tab
- A table will display available connections. In the **Required actions** column, select the link to **Include Connector Results** and confirm the prompt

##### 4. Create the Result Type template

> There is a known issue whereby applying a result type programmatically results in an empty adaptive card, so we need to apply the card in the user interface

- In Visual Studio Code, open the `resultLayout.json` file and copy its contents to clipboard (<kdb>CTRL</kdb>+ <kbd>A</kbd> then <kbd>CTRL</kbd> + <kbd>C</kbd> on Windows, <kbd>CMD</kbd> + <kbd>A</kbd> then <kbd>CMD</kbd> + <kbd>C</kbd> on macOS)
- In the web browser, in the Microsoft 365 admin center, navigate to the [Settings > Search & Intelligence](https://admin.microsoft.com/?source=applauncher#/MicrosoftSearch) area
- Activate the [Customizations](https://admin.microsoft.com/?source=applauncher#/MicrosoftSearch/connectors) tab
- Select the [Result Types](https://admin.microsoft.com/?source=applauncher#/MicrosoftSearch/resulttypes) page
- Select `Add` to open the side panel to create a new result type.
- In the `Name` field, enter `{{connectionName}}`
- Confirm the changes by selecting `Next`
- In the `Content sources` field, select `{{connectorName}}-local`
- Confirm the changes by selecting `Next`
- Skip the `Set rules for the result type` section by selecting `Next`
- Under the `Result Layout` section, select `Edit`
- In the `Paste the JSON script that you created with Layout Designer` field, paste the contents of the clipboard (<kbd>CTRL</kbd> + <kbd>V</kbd> on Windows, <kbd>CMD</kbd> + <kbd>V</kbd> on macOS)
- Confirm the changes by selecting `Next`
- Confirm the changes by selecting `Add Result Type`
- Close the dialog by selecting `Done`
- Wait a few minutes for the changes to be applied

##### 5. Test search

- Navigate to [Microsoft365.com](https://www.microsoft365.com)
- Enter a search term into the search bar
- Items will be shown from the data ingested by the Graph connector in the search results.

## More information

This project contains a CodeTour that explains the project structure in more detail. Because the CodeTour refers to files that are created after the first run, you can start it after you have completed the project.
