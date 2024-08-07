import {
  TeamsActivityHandler,
  CardFactory,
  TurnContext,
  MessagingExtensionQuery,
  MessagingExtensionResponse,
  AttachmentLayoutTypes,
} from "botbuilder";
import * as ACData from "adaptivecards-templating";
import card from "../cards/card.json";

export class SearchApp extends TeamsActivityHandler {

  public async handleTeamsMessagingExtensionQuery(
    context: TurnContext,
    query: MessagingExtensionQuery
  ): Promise<MessagingExtensionResponse> {

    const text = query.parameters[0]?.value as string ?? "";
    var template = new ACData.Template(card);

    return {
      composeExtension: {
        type: 'result',
        attachmentLayout: AttachmentLayoutTypes.List,
        attachments: [
          {
            contentType: 'application/vnd.microsoft.card.adaptive',
            content: template.expand({ $root: { text } }),
            preview: CardFactory.thumbnailCard(text)
          },
        ],
      },
    };
  }
}
