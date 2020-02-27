using Informa.IntelligenceOne.Parser.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Informa.IntelligenceOne.Parser.DAL
{
    class ParserRepository : IParserRepository
    {
        protected InformaContext _context;

        public ParserRepository(InformaContext context)
        {
            _context = context;
        }

        public async Task<List<BankConfig>> GetByBankId(int bankId)
        {
            return await _context.Set<BankConfig>()
                .Where(bc => bc.BankId == bankId)
                .ToListAsync();
        }

        public BankResults Create(NewBankResultInfomation bankResultInfo)
        {
            var bankResult = new BankResults()
            {
                ConfigId = bankResultInfo.ConfigId,
                Results = bankResultInfo.Results,
                FileName = bankResultInfo.FileName,
                UpdateDateTime = DateTime.Now,
                ZoneName = bankResultInfo.ZoneName
            };
            //_context.Entry(bankResult).State = EntityState.Added;

            _context.Add(bankResult);

            return bankResult;

        }
    }
}
