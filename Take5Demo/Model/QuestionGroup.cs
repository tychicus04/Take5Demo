using System.Text.Json.Serialization;

namespace Take5Demo.Model
{
    public class QuestionGroup
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("questions")]
        public List<Question> Questions { get; set; }
    }
}
