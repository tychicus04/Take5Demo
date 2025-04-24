using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Take5Demo.Model
{
    public class SignatureInfo
    {
        public string Base64Image { get; set; }
        public bool HasSignature => !string.IsNullOrEmpty(Base64Image);
        public string Status { get; set; }
    }
}
