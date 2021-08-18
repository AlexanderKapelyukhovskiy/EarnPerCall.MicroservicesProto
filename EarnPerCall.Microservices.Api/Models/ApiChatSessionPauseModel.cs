using Newtonsoft.Json;
using System;

namespace EarnPerCall.Microservices.Api.Models
{
    public class ApiChatSessionModel
    {
        [JsonProperty("sessionId")]
        public int SessionId { get; set; }
        [JsonProperty("customerId")]
        public int CustomerId { get; set; }
        [JsonProperty("advisorId")]
        public int AdvisorId { get; set; }
    }

    public class ApiChatSessionPauseModel
    {
        [JsonProperty("pauseTime")]
        public DateTime PauseTime { get; set; }
    }
}
