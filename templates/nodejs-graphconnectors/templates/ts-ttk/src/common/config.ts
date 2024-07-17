import { ExternalConnectors } from '@microsoft/microsoft-graph-types';
import { resultLayout } from "./resultLayout";

export const config = {
  aadAppTenantId: process.env.ENTRA_APP_TENANT_ID,
  aadAppClientId: process.env.ENTRA_APP_CLIENT_ID,
  aadAppClientSecret: process.env.ENTRA_APP_CLIENT_SECRET,
  storageAccountConnectionString: process.env.AzureWebJobsStorage,
  notificationEndpoint: process.env.NOTIFICATION_ENDPOINT,
  graphSchemaStatusInterval: parseInt(process.env.GRAPH_SCHEMA_STATUS_INTERVAL) || 60,
  connector: {
    // 3-32 characters
    id: '{{connectionId}}',
    name: '{{connectorName}}',
    description: '{{connectorDescription}}',
    activitySettings: {
      urlToItemResolvers: [
        {
          urlMatchInfo: {
            baseUrls: [
              'https://app.contoso.com'
            ],
            urlPattern: '/kb/(?<slug>[^/]+)'
          },
          itemId: '{slug}',
          priority: 1
        } as ExternalConnectors.ItemIdResolver
      ]
    },
    searchSettings: {
      searchResultTemplates: [
        {
          id: '{{connectionId}}',
          priority: 1,
          layout: resultLayout
        }
      ]
    },
    // https://learn.microsoft.com/graph/connecting-external-content-manage-schema
    schema: {
      baseType: 'microsoft.graph.externalItem',
      // Add properties as needed
      properties: [
        {
          name: 'title',
          type: 'string',
          isQueryable: true,
          isSearchable: true,
          isRetrievable: true,
          labels: [
            'title'
          ]
        },
        {
          name: 'url',
          type: 'string',
          isRetrievable: true,
          labels: [
            'url'
          ]
        },
        {
          name: 'iconUrl',
          type: 'string',
          isRetrievable: true,
          labels: [
            'iconUrl'
          ]
        }
      ]
    }
  } as ExternalConnectors.ExternalConnection
};