using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Take5Demo.Model
{
    public class ConfigInfo
    {
        [JsonPropertyName("configVersion")]
        public int ConfigVersion { get; set; }
    }
}
