using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;

namespace Custom.Engine.Agent.Controllers
{
    [Route("api/messages")]
    [ApiController]
    public class BotController(CloudAdapter adapter, IBot bot) : ControllerBase
    {
        private readonly CloudAdapter Adapter = adapter;
        private readonly IBot Bot = bot;

        [HttpPost]
        public async Task PostAsync(CancellationToken cancellationToken = default)
        {
            await Adapter.ProcessAsync(Request, Response, Bot, cancellationToken);
        }
    }
}