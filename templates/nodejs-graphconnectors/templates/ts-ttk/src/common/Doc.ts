// Represents the document to import
interface Doc {
  id: string;
  // Document title
  title: string;
  // Document content. Can be plain-text or HTML
  content: string;
  // URL to the document in the external system
  url: string;
  // URL to the document icon. Required by Microsoft Copilot for Microsoft 365
  iconUrl?: string;
  // Used for incremental updates
  lastModifiedDate: number;
}