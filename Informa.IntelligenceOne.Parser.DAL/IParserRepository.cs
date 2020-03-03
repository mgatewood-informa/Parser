using Informa.IntelligenceOne.Parser.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Informa.IntelligenceOne.Parser.DAL
{
    public interface IParserRepository
    {
        BankResults Create(NewBankResultInfomation bankResultInfo);

        Task<List<ZoneConfig>> GetByLenderAndSheetId(int lenderId, int sheetId);
    }
}
