using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Informa.IntelligenceOne.Parser.Models
{
    public partial class ZoneConfig : TrackedEntity
    {
        public int LenderId { get; set; }

        public int SheetId { get; set; }

        public int Priority  { get; set; }

        public string Zones { get;set;}
    }
}
