using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Barkod2021API_INTelligence.Models
{
    public class CurrencyModel
    {
        public string fromCurr { get; set; } = "";
        public string toCurr { get; set; } = "";
        public decimal amount { get; set; } = 0;
        public string time { get; set; } = "";
    }
}
