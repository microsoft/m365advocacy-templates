@description('Used to generate names for bot service resources')
param resourceBaseName string

@description('The bot display name')
param botDisplayName string

@description('The managed identity resource ID')
param identityId string

@description('The managed identity client ID')
param identityClientId string

@description('The managed identity tenant ID')
param identityTenantId string

@description('The web app default host name')
param webAppDefaultHostName string

@description('The Log Analytics workspace ID for diagnostics')
param logAnalyticsWorkspaceId string

@description('The Graph Entra App client ID')
param apiEntraAppClientId string

@description('The Graph Entra App tenant ID')
param apiEntraAppTenantId string

@description('The Graph Entra App client secret')
@secure()
param apiEntraAppClientSecret string

@description('The Application Insights instrumentation key')
param applicationInsightsInstrumentationKey string

@description('The Application Insights application ID')
param applicationInsightsAppId string

resource botService 'Microsoft.BotService/botServices@2021-03-01' = {
  kind: 'azurebot'
  location: 'global'
  name: resourceBaseName
  properties: {
    displayName: botDisplayName
    endpoint: 'https://${webAppDefaultHostName}/api/messages'
    msaAppId: identityClientId
    msaAppTenantId: identityTenantId
    msaAppType: 'UserAssignedMSI'
    msaAppMSIResourceId: identityId
    disableLocalAuth: true
    schemaTransformationVersion: '1.3'
    isCmekEnabled: false
    publicNetworkAccess: 'Enabled'
    developerAppInsightKey: applicationInsightsInstrumentationKey
    // developerAppInsightsApiKey: (generate a key in Application Insights and apply to the resource configuration manually)
    developerAppInsightsApplicationId: applicationInsightsAppId
  }
  sku: {
    name: 'F0'
  }
}

resource botServiceDiagnostics 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  name: 'botservice-diagnostics'
  scope: botService
  properties: {
    workspaceId: logAnalyticsWorkspaceId
    logs: [
      {
        category: 'BotRequest'
        enabled: true
      }
    ]
    metrics: [
      {
        category: 'AllMetrics'
        enabled: true
      }
    ]
  }
}

resource botServiceMsTeamsChannel 'Microsoft.BotService/botServices/channels@2021-03-01' = {
  parent: botService
  location: 'global'
  name: 'MsTeamsChannel'
  properties: {
    channelName: 'MsTeamsChannel'
  }
}

resource botServiceApiConnection 'Microsoft.BotService/botServices/connections@2021-03-01' = {
  parent: botService
  name: 'API'
  location: 'global'
  properties: {
    serviceProviderDisplayName: 'Azure Active Directory v2'
    serviceProviderId: '30dd229c-58e3-4a48-bdfd-91ec48eb906c'
    clientId: apiEntraAppClientId
    clientSecret: apiEntraAppClientSecret
    scopes: 'email offline_access openid profile User.Read'
    parameters: [
      {
        key: 'tenantID'
        value: apiEntraAppTenantId
      }
      {
        key: 'tokenExchangeUrl'
        value: 'api://${webAppDefaultHostName}/botid-${identityClientId}'
      }
    ]
  }
}
