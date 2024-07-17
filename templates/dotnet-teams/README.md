# Microsoft Teams message extension with search command project generator

Create a new [Microsoft Teams app](https://learn.microsoft.com/MicrosoftTeams/platform/overview) project with a message extension containing a search command using a single command.

## Usage

```bash
# install the template
dotnet new install M365Advocacy.Teams.Templates
# create a new project
dotnet new teams-msgext-search [options] [template options]
```

### Template options

None

## Test locally

To test your connector locally, you need to run the following commands:

```bash
# build the project
dotnet pack
# install the local template
dotnet new install ./bin/Release/M365Advocacy.Teams.Templates.0.1.0.nupkg
# create a new project
dotnet new teams-msgext-search -n My.Teams.MsgExt
# see available template options
dotnet new teams-msgext-search -h
# uninstall the local template
dotnet new uninstall M365Advocacy.Teams.Templates
```

## License

This project is licensed under the [MIT License](LICENSE).
