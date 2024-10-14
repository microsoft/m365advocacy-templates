namespace Custom.Engine.Agent;

public class ConfigOptions
{
    public string BOT_ID { get; set; }
    public string BOT_PASSWORD { get; set; }
    public string AZURE_OPENAI_KEY { get; set; }
    public string AZURE_OPENAI_ENDPOINT { get; set; }
    public string AZURE_OPENAI_DEPLOYMENT_NAME { get; set; }
    public string AZURE_STORAGE_CONNECTION_STRING { get; set; }
    public string AZURE_STORAGE_BLOB_CONTAINER_NAME { get; set; }
}
