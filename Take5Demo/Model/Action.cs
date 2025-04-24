using System.Text.Json.Serialization;

namespace Take5Demo.Model
{
    public class Action
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("responseType")]
        public string ResponseType { get; set; }
    }
}
