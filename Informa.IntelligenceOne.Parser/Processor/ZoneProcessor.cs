using Informa.IntelligenceOne.Parser.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Informa.IntelligenceOne.Parser.Processor
{
    internal class ZoneProcessorState
    {
        public JArray _jsonZones;
        public JArray _jsonSheet;
        public string _sheetName;
        public int _configId;

        public JArray _jsonFinalZoneArray;
        public string _zoneName;

        public ZoneProcessorState(JArray jsonFinalZoneArray, string sheetName, string zoneName,int configId)
        {
            _jsonFinalZoneArray = jsonFinalZoneArray;
            _sheetName = sheetName;
            _zoneName = zoneName;
            _configId = configId;
        }
        public ZoneProcessorState(JArray jsonZones, JArray jsonSheet, string sheetName,int configId)
        {
            _jsonZones = jsonZones;
            _jsonSheet = jsonSheet;
            _sheetName = sheetName;
            _configId = configId;
        }
    }
    class ZoneProcessor
    {
        List<String> _jsonPagesToProcess;
        JObject _jsonZone;
        JArray _jsonZones;

        JArray _jsonSheet;

        JArray _jsonZoneArray = new JArray();
        JArray _jsonFinalZoneArray = new JArray();

        List<ZoneProcessorState> _zoneResults = new List<ZoneProcessorState>();

        ZoneLocator _zoneLocator;

        int _counter;
        int _counterMax;

        int _configId;

        StringBuilder _builderSheetName;
        
        StringBuilder _builderZoneStart;
        StringBuilder _builderZoneIncludeStart;
        StringBuilder _builderZoneStop;
        StringBuilder _builderZoneIncludeStop;
        StringBuilder _builderZoneEnd;
        StringBuilder _builderZoneStartOverride;
        StringBuilder _builderColumnRowValue;

        StringBuilder _builderStrColumnValue;

        bool _blnStart = false;
        bool _blnStop = false;
        bool _blnEnd = false;
        int _intStartRow = -1;
        int _intStartColumn = -1;
        int _intStopRow = -1;
        int _intStopColumn = -1;
        int _intEndColumn = -1;
        int _intEndRow = -1;
        int _intColumnCounter = 0;
        int _intRowCounter = 0;

        int _intStartTextInstance = 0;
        int _intEndTextInstance = 0;
        int _intStopTextInstance = 0;


        int _intDataRowCounter = 0;
        int _intDataColCounter = 0;

        public ZoneProcessor(JArray jsonZones, JArray jsonSheet,string sheetName,int configId)
        {
            _jsonZones = jsonZones;

            _counter = -1;

            _counterMax = _jsonZones.Count;

            _jsonSheet = jsonSheet;

            _configId = configId;

            Utilities.StringFunctions.Replace(ref _builderSheetName, sheetName);
        }

        // We have the entire Sheet and the Zones we need to pull
        public bool MoveNext()
        {
            bool success = false;

            //increment loop counter
            _counter++;

            if (_counterMax == 0)
                return success;

            //clear builders
            

            Utilities.StringFunctions.Replace(ref _builderZoneStart, string.Empty);
            Utilities.StringFunctions.Replace(ref _builderZoneIncludeStart, string.Empty);
            Utilities.StringFunctions.Replace(ref _builderZoneStop, string.Empty);
            Utilities.StringFunctions.Replace(ref _builderZoneIncludeStop, string.Empty);
            Utilities.StringFunctions.Replace(ref _builderZoneEnd, string.Empty);
            Utilities.StringFunctions.Replace(ref _builderZoneStartOverride, string.Empty);

            _jsonFinalZoneArray.Clear();
            _jsonZoneArray.Clear();

            if (_counter < _counterMax)
            {
                if (_zoneLocator == null)
                    _zoneLocator = new ZoneLocator((JObject)_jsonZones[_counter]);
                else
                    _zoneLocator.Reset((JObject)_jsonZones[_counter]);

                success = true;
            }
            
            return success;

        }

        public void Process()
        {
            if (_zoneLocator.IsValid())
            {
                FindData();

                GetData();

                PersistData();
            }
        }

        private void FindData()
        {
            bool allCordinatesFound = false;
            // Have config, so now loop through the entire Json doc and find the data
            foreach (JObject itemrow in _jsonSheet)
            {
                _intRowCounter++;

                if (allCordinatesFound)
                    break;

                // Loop through each column
                _intColumnCounter = 0;
                foreach (var column in itemrow)
                {
                    // Increment the Counter - since we continue if null value
                    _intColumnCounter++;

                    Utilities.StringFunctions.Replace(ref _builderColumnRowValue, column.Value, Const.Parser.UPPER_CASE_VALUE);

                    // Check to see if we have the Start of the Zone
                    // Only need to check if we haven't found the start
                    if (!_blnStart)
                    {
                        _blnStart = _zoneLocator.StartTextFound(_builderColumnRowValue.ToString(),
                            ref _intStartColumn, ref _intStartRow, ref _intStartTextInstance, _intColumnCounter,_intRowCounter);
                    }

                    // Check to see if we have the End of the Zone
                    // Only need to check if we haven't found the end
                    if (!_blnEnd)
                    {
                        _blnEnd = _zoneLocator.EndTextFound(_builderColumnRowValue.ToString(),
                            ref _intEndColumn, ref _intEndRow, ref _intEndTextInstance, _intColumnCounter, _intRowCounter);

                    }

                    // Check to see if we have the Stop of the Zone
                    // Only need to check if we haven't found the stopping point
                    if (!_blnStop)
                    {
                        _blnStop = _zoneLocator.StopTextFound(_builderColumnRowValue.ToString(),
                                ref _intStopColumn, ref _intStopRow, ref _intStopTextInstance, _intColumnCounter, _intRowCounter);
                    }

                    if (_blnStop)
                    {  
                        // Can we just stop, we have found the Stop Text
                        allCordinatesFound = true;

                        if (!_blnStart)
                        {
                            //log an error
                        }

                        if (!_blnEnd)
                        {
                            //log an error
                        }

                        break;
                    }

                }
            }

        }

        public void GetData()
        {
            _intDataRowCounter = 0;
            _intDataColCounter = 0;

            JObject jsonZoneRow = new JObject();

            // If we did not find a Stop, go until the end
            if (_intStopRow == -1)
            {
                _intStopRow = _jsonSheet.Count;
            }

            foreach (JObject dataitem in _jsonSheet)
            {
                _intDataRowCounter++;
                // If we are not at the start of our data, then skip
                if (_intDataRowCounter < _intStartRow)
                {
                    continue;
                }
                // if we hit the stop row, we can get out of our loop
                else if (_intDataRowCounter > _intStopRow && _intStopRow != -1)
                {
                    break;
                }
                // Start with a new Object for each Row
                jsonZoneRow = new JObject();


                
                _intDataColCounter = 0;
                
                bool blnData = false;

                // Loop through each Column in the Row
                foreach (var columnitem in dataitem)
                {
                    _intDataColCounter++;
                    // If we have not hit the start column go to the next column
                    // This will allow the bank to put in bogus columns before the data
                    if (_intDataColCounter < _intStartColumn)
                    {
                        continue;
                    }
                    // if we hit the end, we can stop
                    if (_intDataColCounter >= _intEndColumn)
                    {
                        // First check to ensure an end was detected, if end = -1, then we 
                        // will continue until there are no more columns in the JSON
                        if (_intEndColumn != -1)
                        {
                            break;
                        }
                    }
                    // Add the Data to our JSON Object
                    Utilities.StringFunctions.Replace(ref _builderStrColumnValue,columnitem.Value);
                    
                    //dont like this implementation - mg
                    if(_zoneLocator.ExcludeData(_builderStrColumnValue.ToString()))
                        Utilities.StringFunctions.Replace(ref _builderStrColumnValue, string.Empty);

                    jsonZoneRow.Add(columnitem.Key, _builderStrColumnValue.ToString());
                    if (_builderStrColumnValue != null)
                    {
                        if(_builderStrColumnValue.Length > 0)
                            blnData = true;
                    }
                }

                // We have an entire Row of Data, add to our Array
                // If all of the columns are null, then do not add
                if (blnData)
                {
                    _jsonZoneArray.Add(jsonZoneRow);
                }
            }

            // Insert these results into the database?  This is only for one Zone
            // If there are multiple zones, then we cannot save here
            JObject jsonZone = new JObject();
            jsonZone.Add("Zone", _zoneLocator.GetZoneName());
            jsonZone.Add("Sheet", _builderSheetName.ToString());

            // Add the Array to json, must use a property
            jsonZone.Add(new JProperty("ParsedData", _jsonZoneArray));

            //jsonFinalZoneArray.Add(jsonZoneArray);
            _jsonFinalZoneArray.Add(jsonZone);
        }

        public void PersistData()
        {
            
            _zoneResults.Add(new ZoneProcessorState(new JArray(_jsonFinalZoneArray), _builderSheetName.ToString(), _zoneLocator.GetZoneName(),_configId));
        }
        public List<ZoneProcessorState> ProcessedZones()
        {
            if (_zoneResults == null)
                return null;

            if (_zoneResults.Count == 0)
                return null;

            return _zoneResults;
        }
    }
}
