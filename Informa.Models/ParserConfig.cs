using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Informa.IntelligenceOne.Parser.Models
{
    public partial class BankConfig : TrackedEntity
    {
        public int BankId { get; set; }

        public string Configuration {get;set;}

        public DateTime UpdateDateTime { get; set; }
    }
}
