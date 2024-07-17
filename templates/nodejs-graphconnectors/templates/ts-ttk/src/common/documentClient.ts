export async function getDocuments(): Promise<Doc[]> {
  // Implement logic to retrieve documents from the data source
  return [];
}

export async function getDocumentsModifiedSince(date: number): Promise<Doc[]> {
  // Implement logic to retrieve documents from the data source
  return [];
}

export async function getDocument(id: string): Promise<Doc> {
  // Implement logic to retrieve a document by ID from the data source
  return { id, title: '', content: '', url: '', lastModifiedDate: 0 };
}