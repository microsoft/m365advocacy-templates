using AdaptiveCards;
using AdaptiveCards.Templating;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Newtonsoft.Json;

namespace Teams.MsgExt.Search.Search;

public class SearchApp : TeamsActivityHandler
{
    protected override async Task<MessagingExtensionResponse> OnTeamsMessagingExtensionQueryAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionQuery query, CancellationToken cancellationToken)
    {
        var text = query?.Parameters?[0]?.Value as string ?? string.Empty;

        var card = await File.ReadAllTextAsync(Path.Combine(".", "Resources", "card.json"), cancellationToken);
        var template = new AdaptiveCardTemplate(card);

        return new MessagingExtensionResponse
        {
            ComposeExtension = new MessagingExtensionResult
            {
                Type = "result",
                AttachmentLayout = "list",
                Attachments = [
                    new MessagingExtensionAttachment
                        {
                            ContentType = AdaptiveCard.ContentType,
                            Content = JsonConvert.DeserializeObject(template.Expand(new { title = text })),
                            Preview = new ThumbnailCard { Title = text }.ToAttachment()
                        }
                ]
            }
        };
    }
}
