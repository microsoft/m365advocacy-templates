@maxLength(20)
@minLength(4)
@description('Used to generate names for all resources in this file')
param resourceBaseName string

@description('Required when create Azure Bot service')
param botEntraAppClientId string

@secure()
param botEntraAppClientSecret string

param webAppSKU string

@maxLength(42)
param botDisplayName string

param serverfarmsName string = resourceBaseName
param webAppName string = resourceBaseName
param location string = resourceGroup().location

param modelDeploymentName string
param modelName string
param modelVersion string

// create storage account for bot state
resource storageAccount 'Microsoft.Storage/storageAccounts@2022-05-01' = {
  name: replace(resourceBaseName, '-', '')
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    supportsHttpsTrafficOnly: true
    defaultToOAuthAuthentication: true
  }
}

// create app service plan
resource serverfarm 'Microsoft.Web/serverfarms@2021-02-01' = {
  kind: 'app'
  location: location
  name: serverfarmsName
  sku: {
    name: webAppSKU
  }
}

// create azure app service
resource webApp 'Microsoft.Web/sites@2021-02-01' = {
  kind: 'app'
  location: location
  name: webAppName
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: serverfarm.id
    httpsOnly: true
    siteConfig: {
      alwaysOn: true
      ftpsState: 'FtpsOnly'
    }
  }
}

// set app settings on the app service
resource siteConfig 'Microsoft.Web/sites/config@2021-02-01' = {
  name: 'appsettings'
  parent: webApp
  properties: {
    WEBSITE_RUN_FROM_PACKAGE: '1'
    RUNNING_ON_AZURE: '1'
    BOT_ID: botEntraAppClientId
    BOT_PASSWORD: '@Microsoft.KeyVault(VaultName=${keyVault.name};SecretName=botPassword)'
    AZURE_OPENAI_DEPLOYMENT_NAME: modelDeploymentName
    AZURE_OPENAI_KEY: '@Microsoft.KeyVault(VaultName=${keyVault.name};SecretName=azureOpenAIKey)'
    AZURE_OPENAI_ENDPOINT: aiServices.outputs.AZURE_OPENAI_ENDPOINT
    AZURE_STORAGE_CONNECTION_STRING: '@Microsoft.KeyVault(VaultName=${keyVault.name};SecretName=storageAccountConnectionString)'
    AZURE_STORAGE_BLOB_CONTAINER_NAME: 'state'
    APPINSIGHTS_INSTRUMENTATIONKEY: applicationInsights.properties.InstrumentationKey
    APPINSIGHTS_PROFILERFEATURE_VERSION: '1.0.0'
    APPINSIGHTS_SNAPSHOTFEATURE_VERSION: '1.0.0'
    APPLICATIONINSIGHTS_CONNECTION_STRING: applicationInsights.properties.ConnectionString
    ApplicationInsightsAgent_EXTENSION_VERSION: '~2'
    DiagnosticServices_EXTENSION_VERSION: '~3'
    InstrumentationEngine_EXTENSION_VERSION: 'disabled'
    SnapshotDebugger_EXTENSION_VERSION: 'disabled'
    XDT_MicrosoftApplicationInsights_BaseExtensions: 'disabled'
    XDT_MicrosoftApplicationInsights_Java: '1'
    XDT_MicrosoftApplicationInsights_Mode: 'recommended'
    XDT_MicrosoftApplicationInsights_NodeJS: '1'
    XDT_MicrosoftApplicationInsights_PreemptSdk: 'disabled'
  }
}

// create application insights resource
resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: resourceBaseName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Request_Source: 'rest'
  }
}

// create azure key vault
resource keyVault 'Microsoft.KeyVault/vaults@2021-06-01-preview' = {
  name: resourceBaseName
  location: location
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
    accessPolicies: [
      {
        tenantId: subscription().tenantId
        objectId: webApp.identity.principalId
        permissions: {
          secrets: ['get', 'list']
        }
      }
    ]
  }
}

// add bot password to key vault
resource botPassword 'Microsoft.KeyVault/vaults/secrets@2021-06-01-preview' = {
  parent: keyVault
  name: 'botPassword'
  properties: {
    value: botEntraAppClientSecret
  }
}

// add azure openai key to key vault
resource appClientSecretVault 'Microsoft.KeyVault/vaults/secrets@2021-06-01-preview' = {
  parent: keyVault
  name: 'azureOpenAIKey'
  properties: {
    value: aiServices.outputs.SECRET_AZURE_OPENAI_API_KEY
  }
}

// add storage account connection string to key vault
resource storageAccountVault 'Microsoft.KeyVault/vaults/secrets@2021-06-01-preview' = {
  parent: keyVault
  name: 'storageAccountConnectionString'
  properties: {
    value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storageAccount.id, storageAccount.apiVersion).keys[0].value}'
  }
}

// create azure ai bot service
module azureBotRegistration './botRegistration/azurebot.bicep' = {
  name: 'Azure-Bot-registration'
  params: {
    resourceBaseName: resourceBaseName
    botEntraAppClientId: botEntraAppClientId
    botAppDomain: webApp.properties.defaultHostName
    botDisplayName: botDisplayName
  }
}

// create azure ai service and deployment
module aiServices './aiServices/aiServices.bicep' = {
  name: 'AI-Services'
  params: {
    resourceBaseName: resourceBaseName
    modelDeploymentName: modelDeploymentName
    modelName: modelName
    modelVersion: modelVersion
  }
}

// The output will be persisted in .env.{envName}. Visit https://aka.ms/teamsfx-actions/arm-deploy for more details.
output BOT_AZURE_APP_SERVICE_RESOURCE_ID string = webApp.id
output BOT_DOMAIN string = webApp.properties.defaultHostName
