﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Informa.IntelligenceOne.Parser.DAL;
using Informa.IntelligenceOne.Parser.Models;

namespace Informa.IntelligenceOne.Parser.DAL.Core
{
    public interface IParserService
    {
        Task<IList<ZoneConfig>> GetZoneConfig(int LenderId, int SheetId);

        void CreateBankResultEntry(List<NewBankResultInfomation> bankResults);
    }
}
