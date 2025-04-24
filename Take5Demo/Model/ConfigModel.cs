using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Take5Demo.Model
{
    public class ConfigModel
    {
        [JsonPropertyName("configInfo")]
        public ConfigInfo ConfigInfo { get; set; }

        [JsonPropertyName("jobTab")]
        public JobTab JobTab { get; set; }

        [JsonPropertyName("dataLists")]
        public List<FeatureData> DataLists { get; set; }
    }
}
