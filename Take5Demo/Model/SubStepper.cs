using System.Text.Json.Serialization;

namespace Take5Demo.Model
{
    public class SubStepper
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("featureKey")]
        public string FeatureKey { get; set; }

        [JsonPropertyName("subTitle")]
        public string SubTitle { get; set; }

        [JsonPropertyName("actionList")]
        public List<Action> ActionList { get; set; } = new List<Action>();
    }
}
