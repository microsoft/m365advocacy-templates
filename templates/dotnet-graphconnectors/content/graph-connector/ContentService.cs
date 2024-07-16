using Microsoft.Graph.Models.ExternalConnectors;

class Document
{
  public string? Title { get; set; }
  public string? Content { get; set; }
  public string? Url { get; set; }
  public string? IconUrl { get; set; }
}

static class ContentService
{
  static IEnumerable<Document> Extract()
  {
    // return the documents to import
    return [];
  }

  static string GetDocId(Document doc)
  {
    // Generate a unique ID for the document.
    // ID can't contain /
    // Generate an ID that you can resolve back to the document's URL
    // so that URL to item resolvers can properly record activity.
    return string.Empty;
  }

  static IEnumerable<ExternalItem> Transform(IEnumerable<Document> documents)
  {
    return documents.Select(doc =>
    {
      var docId = GetDocId(doc);
      return new ExternalItem
      {
        Id = docId,
        Properties = new()
        {
          AdditionalData = new Dictionary<string, object> {
            // Add properties as defined in the schema in ConnectionConfiguration.cs
            { "title", doc.Title ?? "" },
            { "url", doc.Url ?? "" },
            { "iconUrl", doc.IconUrl ?? "" }
          }
        },
        Content = new()
        {
          Value = doc.Content ?? "",
          Type = ExternalItemContentType.Text
        },
        Acl = new()
        {
          new()
          {
            Type = AclType.Everyone,
            Value = "everyone",
            AccessType = AccessType.Grant
          }
        }
      };
    });
  }

  static async Task Load(IEnumerable<ExternalItem> items)
  {
    foreach (var item in items)
    {
      Console.Write(string.Format("Loading item {0}...", item.Id));
      try
      {
        await GraphService.Client.External
          .Connections[Uri.EscapeDataString(ConnectionConfiguration.ExternalConnection.Id!)]
          .Items[item.Id]
          .PutAsync(item);
        Console.WriteLine("DONE");
      }
      catch (Exception ex)
      {
        Console.WriteLine("ERROR");
        Console.WriteLine(ex.Message);
      }
    }
  }

  public static async Task LoadContent()
  {
    var content = Extract();
    var transformed = Transform(content);
    await Load(transformed);
  }
}