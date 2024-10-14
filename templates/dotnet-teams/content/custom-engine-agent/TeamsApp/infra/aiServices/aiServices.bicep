@maxLength(20)
@minLength(4)
@description('Used to generate names for all resources in this file')
param resourceBaseName string

param location string = resourceGroup().location
param modelName string
param modelVersion string
param modelDeploymentName string

resource account 'Microsoft.CognitiveServices/accounts@2023-05-01' = {
  name: resourceBaseName
  location: location
  kind: 'OpenAI'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    customSubDomainName: resourceBaseName
    publicNetworkAccess: 'Enabled'
    networkAcls: {
      defaultAction: 'Allow'
      ipRules: []
      virtualNetworkRules: []
    }
    disableLocalAuth: false
  }
  sku: {
    name: 'S0'
  }
}

resource deployment 'Microsoft.CognitiveServices/accounts/deployments@2023-05-01' = {
  parent: account
  name: modelDeploymentName
  properties: {
    model: {
      format: 'OpenAI'
      name: modelName
      version: modelVersion
    }
  }
  sku: {
    name: 'Standard'
    capacity: 10
  }
}

output SECRET_AZURE_OPENAI_API_KEY string = listKeys(account.id, '2022-12-01').key1
output AZURE_OPENAI_ENDPOINT string = account.properties.endpoint
