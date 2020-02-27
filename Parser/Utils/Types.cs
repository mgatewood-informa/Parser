using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Informa.IntelligenceOne.Parser.Types
{
    class ProcessedPage
    {
        public string Name;
        public JArray Json;

        public ProcessedPage(string name, JArray json)
        {
            Name = name;
            Json = json;
        }
    }
}
