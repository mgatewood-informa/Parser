using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Informa.IntelligenceOne.Parser.Extractor
{
    class PDF : Base_Extractor
    {

        public PDF(FileInfo fInfo) : base(fInfo)
        {
            _fInfo = fInfo;
        }

        public new void Extract()
        {

        }
    }
}
