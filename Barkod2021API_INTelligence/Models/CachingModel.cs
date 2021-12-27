using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Barkod2021API_INTelligence.Models
{
    public class CachingModel
    {
        public string quote { get; set; }
        public long timestamp { get; set; }

        public CachingModel()
        {
            quote = "";
            timestamp = 0;
        }
    }
}
