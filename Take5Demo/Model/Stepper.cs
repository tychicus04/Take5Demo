using System.Text.Json.Serialization;

namespace Take5Demo.Model
{
    public class Stepper
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("subSteppers")]
        public List<SubStepper> SubSteppers { get; set; } = new List<SubStepper>();
    }
}