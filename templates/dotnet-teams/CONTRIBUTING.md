# Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

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