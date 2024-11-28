using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Teams.AI;

namespace Custom.Engine.Agent.Controllers
{
    [Route("api/messages")]
    [ApiController]
    public class BotController(TeamsAdapter adapter, IBot bot) : ControllerBase
    {
        private readonly TeamsAdapter Adapter = adapter;
        private readonly IBot Bot = bot;

        [HttpPost]
        public async Task PostAsync(CancellationToken cancellationToken = default)
        {
            await Adapter.ProcessAsync(Request, Response, Bot, cancellationToken);
        }
    }
}