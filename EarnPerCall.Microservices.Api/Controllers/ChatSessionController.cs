using EarnPerCall.Microservices.ChatSessionActorService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using System;
using System.Threading.Tasks;

namespace EarnPerCall.Microservices.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class ChatSessionController : ControllerBase
    {
        private readonly ILogger<ChatSessionController> _logger;

        public ChatSessionController(ILogger<ChatSessionController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post(int sessionId, int avaliableTime)
        {
            await GetActor(sessionId).StartSession(sessionId, avaliableTime);

            return new OkObjectResult(new { success = true });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int sessionId)
        {
            await GetActor(sessionId).StopSession(sessionId);

            return new OkObjectResult(new { success = true });
        }

        private IChatSessionActorService GetActor(int sessionId)
        {
            return ActorProxy.Create<IChatSessionActorService>(
                new ActorId(sessionId),
                new Uri("fabric:/EarnPerCall.Microservices/ChatSessionActorServiceActorService")
            );
        }
    }
}
