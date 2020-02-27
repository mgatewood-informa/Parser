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

        int _counter;
        int _counterMax;

        int _configId;

        StringBuilder _builderSheetName;
        StringBuilder _builderZoneName;
        StringBuilder _builderZoneStart;
        StringBuilder _builderZoneIncludeStart;
        StringBuilder _builderZoneStop;
        StringBuilder _builderZoneIncludeStop;
        StringBuilder _builderZoneEnd;
        StringBuilder _builderZoneStartOverride;
        StringBuilder _builderStrValue;

        StringBuilder _builderStrColumnValue;

        bool _blnStart = false;
        bool _blnStop = false;
        bool _blnEnd = false;
        int _intStartRow = -1;
        int _intStartColumn = -1;
        int _intStopRow = -1;
        int _intStopColumn = -1;
        int _intEnd = -1;
        int _intCounter = 0;
        int _intRowCounter = 0;

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
            Utilities.StringFunctions.Replace(ref _builderZoneName, string.Empty);

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
                _jsonZone = (JObject)_jsonZones[_counter];

                Utilities.StringFunctions.Replace(ref _builderZoneName, _jsonZone["Name"]);

                Utilities.StringFunctions.Replace(ref _builderZoneStart, _jsonZone["Start"], Const.Parser.UPPER_CASE_VALUE);
                Utilities.StringFunctions.Replace(ref _builderZoneIncludeStart, _jsonZone["IncludeStart"]);
                Utilities.StringFunctions.Replace(ref _builderZoneStop, _jsonZone["Stop"], Const.Parser.UPPER_CASE_VALUE);
                Utilities.StringFunctions.Replace(ref _builderZoneIncludeStop, _jsonZone["IncludeStop"]);
                Utilities.StringFunctions.Replace(ref _builderZoneEnd, _jsonZone["End"]);
                Utilities.StringFunctions.Replace(ref _builderZoneStartOverride, _jsonZone["StartOverride"]);

                success = true;
            }
            
            return success;

        }

        public void Process()
        {
            FindData();

            GetData();

            PersistData();
        }

        private void FindData()
        {
            // Have config, so now loop through the entire Json doc and find the data
            foreach (JObject itemrow in _jsonSheet)
            {
                _intRowCounter++;

                // Loop through each column
                _intCounter = 0;
                foreach (var column in itemrow)
                {
                    // Increment the Counter - since we continue if null value
                    _intCounter++;

                    Utilities.StringFunctions.Replace(ref _builderStrValue, column.Value, Const.Parser.UPPER_CASE_VALUE);

                    if (string.IsNullOrEmpty(_builderStrValue.ToString()))
                    {
                        // Move to the next, no values
                        continue;
                    }

                    // Check to see if we have the Start of the Zone
                    // Only need to check if we haven't found the start
                    if (!_blnStart)
                    {
                        if (_builderZoneStart.ToString() == "@START")
                        {
                            _blnStart = true;
                            // Set the Start Row to 1
                            _intStartRow = 1;
                            // Check to see if we have an override
                            if (_builderZoneStartOverride != null)
                            {
                                _intStartColumn = Utilities.StringFunctions.ConvertToInt(_builderZoneStartOverride.ToString(), 1);
                            }
                            else
                            {
                                // Set the Column to 1
                                _intStartColumn = 1;
                            }
                        }
                        else if (_builderStrValue.ToString().Trim() == _builderZoneStart.ToString().Trim())
                        {
                            // Found the start of the Zone, record the Row and Column
                            _blnStart = true;
                            _intStartRow = _intRowCounter;

                            if (_builderZoneStartOverride != null)
                            {
                                _intStartColumn = Utilities.StringFunctions.ConvertToInt(_builderZoneStartOverride.ToString(), 1);
                            }

                            _intStartColumn = _intCounter;
                        }
                    }

                    // Check to see if we have the Stop of a Zone
                    if (_builderStrValue.ToString().Trim() == _builderZoneStop.ToString().Trim())
                    {
                        // Found the Stop of the Zone, record the Row and Column
                        _blnStop = true;
                        _intStopRow = _intRowCounter;
                        // If a Start Column Override was entered, we 
                        // need to ensure that the Stop Column is the same
                        if (_builderZoneStartOverride != null)
                        {
                            _intStopColumn = Utilities.StringFunctions.ConvertToInt(_builderZoneStartOverride.ToString(), 1);
                        }
                        else
                        {
                            _intStopColumn = _intCounter;
                        }
                        // Can we just stop, we have found the Stop Text
                        break;
                    }

                    if (_builderStrValue.ToString().Trim() == _builderZoneEnd.ToString().Trim() && !_blnEnd)
                    {
                        // Found the End of the Zone, record the Column
                        _blnEnd = true;
                        _intEnd = _intCounter;
                    }

                    // If we hit the Stop, then we can break out of this loop
                    if (_blnStop)
                    {

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
                    if (_intDataColCounter >= _intEnd)
                    {
                        // First check to ensure an end was detected, if end = -1, then we 
                        // will continue until there are no more columns in the JSON
                        if (_intEnd != -1)
                        {
                            break;
                        }
                    }
                    // Add the Data to our JSON Object
                    Utilities.StringFunctions.Replace(ref _builderStrColumnValue,columnitem.Value);

                    jsonZoneRow.Add(columnitem.Key, columnitem.Value);
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
            jsonZone.Add("Zone", _builderZoneName.ToString());
            jsonZone.Add("Sheet", _builderSheetName.ToString());

            // Add the Array to json, must use a property
            jsonZone.Add(new JProperty("ParsedData", _jsonZoneArray));

            //jsonFinalZoneArray.Add(jsonZoneArray);
            _jsonFinalZoneArray.Add(jsonZone);
        }

        public void PersistData()
        {
            
            _zoneResults.Add(new ZoneProcessorState(new JArray(_jsonFinalZoneArray), _builderSheetName.ToString(), _builderZoneName.ToString(),_configId));
        }
        public List<ZoneProcessorState> ProcessedZones()
        {
            return _zoneResults;
        }
    }
}
