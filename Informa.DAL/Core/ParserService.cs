using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Informa.IntelligenceOne.Parser.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Informa.IntelligenceOne.Parser.DAL;

namespace Informa.IntelligenceOne.Parser.DAL.Core
{
    public class ParserService : IParserService
    {
        IParserRepository _parserRepo;
        InformaContext _context;

        public ParserService(IParserRepository parserRepo, InformaContext context)
        {
            _parserRepo = parserRepo;
            _context = context;
        }

        public async Task<IList<BankConfig>> GetBankConfig(int BankId)
        {
            //.Include(o => o.BankId)
            return await _context.Set<BankConfig>()
                .AsNoTracking()
                
                .Where(o => o.BankId == BankId)
                .ToListAsync();
        }

        public async void CreateBankResultEntry(List<NewBankResultInfomation> bankResults)
        {
            foreach (NewBankResultInfomation bInfo in bankResults)
            {
                _parserRepo.Create(bInfo);
                
            }
            _context.SaveChanges();
            //await _context.SaveChangesAsync();

        }

    }
}
