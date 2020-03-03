using Informa.IntelligenceOne.Parser.DAL.Core;
using Informa.IntelligenceOne.Parser.DAL;
using Informa.IntelligenceOne.Parser.Models;
using Informa.IntelligenceOne.Parser.Extractor;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Informa.IntelligenceOne.Parser.Processor
{
    class Base_Processor
    {
        static dynamic _jsonconfig;

        int _intLenderId;
        int _intCompanySheetId;

        int _intConfigId;
        string _fileName;

        JArray _jsonZones;
        StringBuilder _builderConfigSheetName;
        StringBuilder _builderProcessedPageSheetName;

        IParserService _parserService;
        List<ProcessedPage> _pagesToProcess;
        List<ZoneProcessorState> _parserResultsToPersist;

        InformaContext _context;
        Hashtable _threadSyncLockHash = Hashtable.Synchronized(new Hashtable());

        /*private async Task<IList<BankConfig>> GetConfiguration()
        {

            //lenderSheetZoneConfigs = GetConfiguration().GetAwaiter().GetResult();

            IList<BankConfig> parserConfigsToProcess = await _parserService.GetBankConfig(_intBankId).GetAwaiter().GetResult();
            return parserConfigsToProcess;
        }*/
        private IList<ZoneConfig> GetConfiguration()
        {

            //lenderSheetZoneConfigs = GetConfiguration().GetAwaiter().GetResult();

            IList<ZoneConfig> parserConfigsToProcess = _parserService.GetZoneConfig(_intLenderId,_intCompanySheetId).GetAwaiter().GetResult();
            return parserConfigsToProcess;
        }


        public virtual void Cleanup()
        {
        }

        public virtual void Go()
        {

            //IList<ParserConfig> parserToProcess = GetConfiguration();

            //get the configuration in the database for the file
            IList<ZoneConfig> lenderSheetZoneConfigs = GetConfiguration();

            //for each config to process
            foreach (ZoneConfig lenderSheetZoneConfig in lenderSheetZoneConfigs)
            {
                _parserResultsToPersist = new List<ZoneProcessorState>();

                //look at the json to see if 
                _jsonconfig = JObject.Parse(lenderSheetZoneConfig.Zones);

                //get sheet name
                Utilities.StringFunctions.Replace(ref _builderConfigSheetName,
                    _jsonconfig["Sheets"][0]["Name"], Const.Parser.UPPER_CASE_VALUE);
                _builderConfigSheetName.Append("$");

                ProcessedPage extractedPage = _pagesToProcess.Find(x => x.Name.ToUpper().EndsWith(_builderConfigSheetName.ToString()));

                if(extractedPage != null)
                {
                    //thread safe
                    ProcessZone(new ZoneProcessorState(
                        (JArray)_jsonconfig["Sheets"][0]["Zones"], 
                        extractedPage.Json, 
                        extractedPage.Name, 
                        lenderSheetZoneConfig.Id));
                }

            }

            PersistConfigResults();
        }

        private void AddZoneResults(List<ZoneProcessorState> aResult)
        {
            if (aResult == null)
                return;

            lock (_threadSyncLockHash.SyncRoot)
            {
                _parserResultsToPersist.AddRange(aResult);
            }
        }
        private void ProcessZone(object state)
        {
            ZoneProcessorState zoneState = (ZoneProcessorState)state;

            ZoneProcessor zoneProcessor = new ZoneProcessor(zoneState._jsonZones, zoneState._jsonSheet, zoneState._sheetName,zoneState._configId);

            while (zoneProcessor.MoveNext())
            {
                zoneProcessor.Process();
            }


            //(all threads join background results) thread safe
            AddZoneResults(zoneProcessor.ProcessedZones());
        }

        private void PersistConfigResults()
        {
            List<NewBankResultInfomation> bankResults = new List<NewBankResultInfomation>();
            foreach (ZoneProcessorState zoneProcessorState in _parserResultsToPersist)
            {
                dynamic jsonResults = new JObject();

                jsonResults.ParsedResults = zoneProcessorState._jsonFinalZoneArray;

                bankResults.Add(new NewBankResultInfomation(){
                     ConfigId = zoneProcessorState._configId,
                     Results = jsonResults.ToString(),
                     FileName = _fileName,
                     UpdateDateTime = DateTime.Now,
                     ZoneName = zoneProcessorState._zoneName });
            }

            if(bankResults.Count > 0)
                _parserService.CreateBankResultEntry(bankResults);
        }
        public Base_Processor(IParserService parserService, List<ProcessedPage> pages,int lenderId,int companySheetId, string fileName)
        {
            //, IParserRepository<BankConfig>
            _parserService = parserService;

            _pagesToProcess = pages;

            _intLenderId = lenderId;

            _intCompanySheetId = companySheetId;

            _fileName = fileName;
        }
    }
}
