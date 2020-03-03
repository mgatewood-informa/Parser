using System;
using System.Collections.Generic;
using System.Text;

namespace Informa.IntelligenceOne.Parser.DAL
{
    public class NewBankResultInfomation
    {
        public int ConfigId { get; set; }
        public string Results { get; set; }
        public string FileName { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public string ZoneName { get; set; }

    }
}
