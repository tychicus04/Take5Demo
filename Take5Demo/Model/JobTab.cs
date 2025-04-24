using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Take5Demo.Model
{
    public class JobTab
    {
        [JsonPropertyName("steppers")]
        public List<Stepper> Steppers { get; set; } = new List<Stepper>();
    }
}
