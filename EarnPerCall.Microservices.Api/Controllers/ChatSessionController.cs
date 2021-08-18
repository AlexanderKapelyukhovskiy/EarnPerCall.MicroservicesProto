using EarnPerCall.Microservices.Api.Models;
using EarnPerCall.Microservices.ChatSessionActorService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using System;
using System.Threading.Tasks;

namespace EarnPerCall.Microservices.Api.Controllers
{
    public class ChatSessionBaseController : ControllerBase
    {
        protected IChatSessionActorService GetActor(int sessionId)
        {
            return ActorProxy.Create<IChatSessionActorService>(
                new ActorId(sessionId),
                new Uri("fabric:/EarnPerCall.Microservices/ChatSessionActorServiceActorService")
            );
        }
    }

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
