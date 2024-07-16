import { GraphError } from '@microsoft/microsoft-graph-client';
import { ExternalConnectors } from '@microsoft/microsoft-graph-types';
import { config } from './config.js';
import { client } from './graphClient.js';

// Represents the document to import
interface Document {
  // Document title
  title: string;
  // Document content. Can be plain-text or HTML
  content: string;
  // URL to the document in the external system
  url: string;
  // URL to the document icon. Required by Microsoft Copilot for Microsoft 365
  iconUrl: string;
}

function extract(): Document[] {
  // return the documents to import
  return [];
}

function getDocId(doc: Document): string {
  // Generate a unique ID for the document.
  // ID can't contain /
  // Generate an ID that you can resolve back to the document's URL
  // so that URL to item resolvers can properly record activity.
  return '';
}

function transform(documents: Document[]): ExternalConnectors.ExternalItem[] {
  return documents.map(doc => {
    const docId = getDocId(doc);
    return {
      id: docId,
      properties: {
        // Add properties as defined in the schema in config.ts
        title: doc.title ?? '',
        url: doc.url,
        iconUrl: doc.iconUrl
      },
      content: {
        value: doc.content ?? '',
        type: 'text'
      },
      acl: [
        {
          accessType: 'grant',
          type: 'everyone',
          value: 'everyone'
        }
      ]
    } as ExternalConnectors.ExternalItem
  });
}

async function load(externalItems: ExternalConnectors.ExternalItem[]) {
  const { id } = config.connection;
  for (const doc of externalItems) {
    try {
      console.log(`Loading ${doc.id}...`);
      await client
        .api(`/external/connections/${id}/items/${doc.id}`)
        .header('content-type', 'application/json')
        .put(doc);
      console.log('  DONE');
    }
    catch (e) {
      const graphError = e as GraphError;
      console.error(`Failed to load ${doc.id}: ${graphError.message}`);
      if (graphError.body) {
        console.error(`${JSON.parse(graphError.body)?.innerError?.message}`);
      }
      return;
    }
  }
}

export async function loadContent() {
  const content = extract();
  const transformed = transform(content);
  await load(transformed);
}

loadContent();