using System.Text.Json.Serialization;

namespace Take5Demo.Model
{
    public class QuestionSet
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("questionGroups")]
        public List<QuestionGroup> QuestionGroups { get; set; }
    }
}
