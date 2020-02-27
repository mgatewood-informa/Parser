using Informa.IntelligenceOne.Parser;
using System;

namespace Parser
{
    class Program
    {
        static void Main(string[] args)
        {
            FileParser parser = new FileParser();
            parser.Parse(@"C:\My.Net\Informa\Parser\Spreadsheets\_intranet_rates_Template.xlsx");
        }
    }
}
