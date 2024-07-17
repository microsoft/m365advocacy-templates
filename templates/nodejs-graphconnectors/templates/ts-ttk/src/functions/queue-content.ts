import { InvocationContext, app } from "@azure/functions";
import { ContentMessage, CrawlType, ItemAction } from "../common/ContentMessage";
import { config } from "../common/config";
import { client } from "../common/graphClient";
import { enqueueItemDeletion, enqueueItemUpdate } from "../common/queueClient";
import { addItemToTable, getItemIds, getLastModifiedDate, recordLastModified, removeItemFromTable } from "../common/tableClient";
import { getDocument, getDocuments, getDocumentsModifiedSince } from "../common/documentClient";

async function crawl(crawlType: CrawlType, context: InvocationContext) {
  switch (crawlType) {
    case 'full':
    case 'incremental':
      await crawlFullOrIncremental(crawlType, context);
      break;
    case 'removeDeleted':
      await removeDeleted(context);
      break;
  }
}

async function crawlFullOrIncremental(crawlType: CrawlType, context: InvocationContext) {
  let documents: Doc[] = [];

  if (crawlType === 'incremental') {
    const lastModifiedDate = await getLastModifiedDate(context);
    documents = await getDocumentsModifiedSince(lastModifiedDate);
  }
  else {
    documents = await getDocuments();
  }

  context.log(`Retrieved ${documents.length} documents`);

  for (const doc of documents) {
    context.log(`Enqueuing item update for ${doc.id}...`);
    enqueueItemUpdate(doc.id);
  }
}

async function removeDeleted(context: InvocationContext) {
  const documents = await getDocuments();
  context.log(`Retrieved ${documents.length} documents`);

  context.log('Retrieving ingested items...');
  const ingestedItemIds = await getItemIds(context);

  ingestedItemIds.forEach(ingestedItemId => {
    if (documents.find(doc => doc.id === ingestedItemId)) {
      context.log(`Item ${ingestedItemId} still exists, skipping...`);
    }
    else {
      context.log(`Item ${ingestedItemId} no longer exists, deleting...`);
      enqueueItemDeletion(ingestedItemId);
    }
  });
}

async function processItem(itemId: string, itemAction: ItemAction, context: InvocationContext) {
  switch (itemAction) {
    case 'update':
      await updateItem(itemId, context);
      break;
    case 'delete':
      await deleteItem(itemId, context);
      break;
  }
}

async function updateItem(itemId: string, context: InvocationContext) {
  const doc = await getDocument(itemId);

  context.log(JSON.stringify(doc, null, 2));

  const externalItem = {
    id: doc.id,
    properties: {
      // update schema properties as needed
      title: doc.title
    },
    content: {
      value: doc.content,
      type: 'text'
    },
    acl: [
      {
        accessType: 'grant',
        type: 'everyone',
        value: 'everyone'
      }
    ]
  }

  context.log(`Transformed item`);
  context.log(JSON.stringify(externalItem, null, 2));

  const externalItemUrl = `/external/connections/${config.connector.id}/items/${doc.id}`;
  context.log(`Updating external item ${externalItemUrl}...`)

  await client
    .api(externalItemUrl)
    .header('content-type', 'application/json')
    .put(externalItem);

  context.log(`Adding item ${doc.id} to table storage...`);
  // track item to support deletion
  await addItemToTable(doc.id, context);
  context.log(`Tracking last modified date ${doc.lastModifiedDate}`);
  // track last modified date for incremental crawl
  await recordLastModified(doc.lastModifiedDate, context);
}

async function deleteItem(itemId: string, context: InvocationContext) {
  const externalItemUrl = `/external/connections/${config.connector.id}/items/${itemId}`;
  context.log(`Deleting external item ${externalItemUrl}...`)

  await client
    .api(externalItemUrl)
    .delete();

  context.log(`Removing item ${itemId} from table storage...`);
  await removeItemFromTable(itemId, context);
}

app.storageQueue("contentQueue", {
  connection: "AzureWebJobsStorage",
  queueName: "queue-content",
  handler: async (message: ContentMessage, context: InvocationContext) => {
    context.log('Received message from queue queue-content');
    context.log(JSON.stringify(message, null, 2));

    const { action, crawlType, itemAction, itemId } = message;

    switch (action) {
      case 'crawl':
        await crawl(crawlType, context);
        break;
      case 'item':
        await processItem(itemId, itemAction, context);
        break;
      default:
        break;
    }
  }
});