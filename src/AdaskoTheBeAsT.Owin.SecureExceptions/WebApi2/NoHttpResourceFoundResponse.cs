using Newtonsoft.Json;

namespace AdaskoTheBeAsT.Owin.SecureExceptions.WebApi2;

public class NoHttpResourceFoundResponse
{
    [JsonProperty(nameof(Message))]
    public string? Message { get; set; }

    [JsonProperty(nameof(MessageDetail))]
    public string? MessageDetail { get; set; }
}
