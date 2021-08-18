using EarnPerCall.Microservices.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace EarnPerCall.Microservices.Api.Controllers
{
    [ApiController]
    [Route("/api/micro/chat/sessions/{sessionId}/pauses")]
    public class ChatSessionPausesController : ChatSessionBaseController
    {
        private readonly ILogger<ChatSessionController> _logger;

        public ChatSessionPausesController(ILogger<ChatSessionController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post(int sessionId, [FromBody] ApiChatSessionPauseModel model)
        {
            var session = GetActor(sessionId);

            await session.SetPauseReminder(model.PauseTime);

            return new OkObjectResult(new { success = true });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int sessionId)
        {
            var session = GetActor(sessionId);

            await session.ClearPauseReminder();

            return new OkObjectResult(new { success = true });
        }
    }
}
