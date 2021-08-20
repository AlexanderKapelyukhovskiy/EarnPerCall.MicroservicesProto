using EarnPerCall.Microservices.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace EarnPerCall.Microservices.Api.Controllers
{
    [ApiController]
    [Route("/api/micro/chat/sessions")]
    public class ChatSessionController : ChatSessionBaseController
    {
        private readonly ILogger<ChatSessionController> _logger;

        public ChatSessionController(ILogger<ChatSessionController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ApiChatSessionModel model)
        {
            var session = GetActor(model.SessionId);

            await session.InitChatSession(model.AdvisorId, model.CustomerId);

            return new OkObjectResult(new { success = true });
        }
    }
}
