using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Informa.IntelligenceOne.Parser.Processor
{
    internal class ZoneLocator
    {
        struct Const
        {
            public static readonly string ACTIVE_CONFIG = "Active";

            public static readonly string EXCLUSIONS = "Exclusions";

            public static readonly string CONFIG_DOC_YES = "YES";
            public static readonly string CONFIG_DOC_NO = "NO";

            public static readonly string ZONE_NAME = "Name";

            public static readonly string START_OVERRIDE = "StartOverrideColumn";
            public static readonly string STOP_OVERRIDE = "StopOverrideColumn";
            public static readonly string END_OVERRIDE = "EndOverrideColumn";

            public static readonly string SECONDARY_START = "SecondaryStart";
            public static readonly string SECONDARY_END = "SecondaryEnd";
            public static readonly string SECONDARY_STOP = "SecondaryStop";

            public static readonly string PRIMARY_START = "PrimaryStart";
            public static readonly string PRIMARY_END = "PrimaryEnd";
            public static readonly string PRIMARY_STOP = "PrimaryStop";

            public static readonly string INCLUDE_START = "IncludeStart";
            public static readonly string INCLUDE_STOP = "IncludeStop";
            public static readonly string INCLUDE_END = "IncludeEnd";

            public static readonly string START_TEXT = "StartText";
            public static readonly string END_TEXT = "EndText";
            public static readonly string STOP_TEXT = "StopText";

            public static readonly string START_TYPE = "StartType";
            public static readonly string END_TYPE = "EndType";
            public static readonly string STOP_TYPE = "StopType";

            public static readonly string DOCUMENT_STOP_EXACT = "Exact";
            public static readonly string DOCUMENT_STOP_CONTAINS = "Contains";
            public static readonly string DOCUMENT_STOP_STARTSWITH = "StartsWith";

            public static readonly string DOCUMENT_START_BEGINNING_TEXT = "@START";
            public static readonly string DOCUMENT_END_LAST_TEXT = "@END";
            public static readonly string DOCUMENT_STOP_ENDING_TEXT = "@STOP";

            public static readonly string INCLUDE_START_TEXT = "IncludeStart";
            public static readonly int INCLUDE_START_TEXT_FALSE = 0;
            public static readonly int INCLUDE_START_TEXT_TRUE = 1;

            public static readonly string INCLUDE_STOP_TEXT = "IncludeStop";
            public static readonly int INCLUDE_STOP_TEXT_FALSE = 0;
            public static readonly int INCLUDE_STOP_TEXT_TRUE = 1;

            public static readonly string INCLUDE_END_TEXT = "IncludeEnd";
            public static readonly int INCLUDE_END_TEXT_FALSE = 0;
            public static readonly int INCLUDE_END_TEXT_TRUE = 1;

            public static readonly int DOCUMENT_START_BEGINNING_TEXT_TRUE = 1;
            public static readonly int DOCUMENT_START_BEGINNING_TEXT_FALSE = 0;

            public static readonly int DOCUMENT_END_LAST_TEXT_TRUE = 1;
            public static readonly int DOCUMENT_END_LAST_TEXT_FALSE = 0;

            public static readonly int DOCUMENT_STOP_ENDING_TEXT_TRUE = 1;
            public static readonly int DOCUMENT_STOP_ENDING_TEXT_FALSE = 0;

            public static readonly int DOCUMENT_STOP_ENDING_COLUMN_MAX = 999;

            public static readonly int DOCUMENT_STOP_EXACT_INT = 0;
            public static readonly int DOCUMENT_STOP_CONTAINS_INT = 1;
            public static readonly int DOCUMENT_STOP_STARTSWITH_INT = 2;

            public static readonly int UPPER_CASE_VALUE = 1;
            public static readonly int DEFAULT_INT_VALUE = -99990999;
            public static readonly int NO_START_OVERRIDE = -1;
            public static readonly int NO_STOP_OVERRIDE = -1;
            public static readonly int NO_END_OVERRIDE = -1;



        }

        public int StartTextType = Const.DEFAULT_INT_VALUE;
        public int StopTextType = Const.DEFAULT_INT_VALUE;
        public int EndTextType = Const.DEFAULT_INT_VALUE;
        
        public int StartAtBeginning = Const.DEFAULT_INT_VALUE;
        public int EndAtLast = Const.DEFAULT_INT_VALUE;
        public int StopAtEnding = Const.DEFAULT_INT_VALUE;

        public int StartTextInstance = 0;
        public int StopTextInstance = 0;
        public int EndTextInstance = 0;

        public int IncludeStartText = Const.DEFAULT_INT_VALUE;
        public int IncludeStopText = Const.DEFAULT_INT_VALUE;
        public int IncludeEndText = Const.DEFAULT_INT_VALUE;

        StringBuilder _builderZoneName = new StringBuilder();

        StringBuilder _secondaryOption = new StringBuilder();
        StringBuilder _secondaryStopOption = new StringBuilder();
        StringBuilder _secondaryEndOption = new StringBuilder();

        StringBuilder _builderStopStartOption = new StringBuilder();
        StringBuilder _builderEndStartOption = new StringBuilder();
        StringBuilder _builderBeginStartOption = new StringBuilder();

        StringBuilder _builderIncludeStart = new StringBuilder();
        StringBuilder _builderIncludeStop = new StringBuilder();
        StringBuilder _builderIncludeEnd = new StringBuilder(); 

        StringBuilder _builder_StartText = new StringBuilder();

        StringBuilder _builder_StartBeginning = new StringBuilder();
        StringBuilder _builder_EndLast = new StringBuilder();
        
        StringBuilder _builder_EndText = new StringBuilder();
        

        StringBuilder _builder_StopText = new StringBuilder();
        StringBuilder _builder_StopEnding = new StringBuilder();

        JObject _zoneConfig;

        StringBuilder _active = new StringBuilder();

        int _startOverride = Const.DEFAULT_INT_VALUE;
        int _stopOverride = Const.DEFAULT_INT_VALUE;
        int _endOverride = Const.DEFAULT_INT_VALUE;

        StringDictionary _exClusionList;
        public ZoneLocator(JObject aJSonConfig)
        {
            _zoneConfig = aJSonConfig;
        }

        public bool IsValid()
        {
            if (IsActive())
            {
                //check for more reasons to make the config invalid;
                return true;
            }

            return false;
        }

        public bool ExcludeData(string aValue)
        {
            if (_exClusionList == null)
                return false;

            if (_exClusionList[aValue] == null)
                return false;

            return true;
        }
                    
        
        public bool IsActive()
        {
            if (_active.Length == 0)
                Utilities.StringFunctions.Replace(ref _active, _zoneConfig[Const.ACTIVE_CONFIG], Const.UPPER_CASE_VALUE);

            if (_active.ToString() == Const.CONFIG_DOC_YES)
            {
                //process exclusion list
                JArray jExclusions = (JArray) _zoneConfig[Const.EXCLUSIONS];
                if (jExclusions != null)
                {
                    _exClusionList = new StringDictionary();
                    foreach (JObject jExclusion in jExclusions)
                    {
                        if (jExclusion != null)
                        {
                            if(jExclusion.ToString().Trim().Length>0)
                                _exClusionList.Add(jExclusion.ToString(), string.Empty);
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public void Reset(JObject zoneConfig)
        {
            _active.Clear();

            StartTextType = Const.DEFAULT_INT_VALUE;

            Utilities.StringFunctions.Replace(ref _builderZoneName, string.Empty);

            _zoneConfig = zoneConfig;
        }   
        public string GetZoneName()
        {
            if (_builderZoneName.Length == 0)
                Utilities.StringFunctions.Replace(ref _builderZoneName, _zoneConfig[Const.ZONE_NAME]);

            return _builderZoneName.ToString();
        }

        #region StartZone
        public int GetStartBeginningOverride()
        {
            if (StartAtBeginning == Const.DEFAULT_INT_VALUE)
            {
                Utilities.StringFunctions.Replace(ref _builder_StartBeginning, _zoneConfig[Const.START_TEXT], Const.UPPER_CASE_VALUE);
                if (_builder_StartBeginning.ToString() == Const.DOCUMENT_START_BEGINNING_TEXT)
                    StartAtBeginning = Const.DOCUMENT_START_BEGINNING_TEXT_TRUE;
                else
                    StartAtBeginning = Const.DOCUMENT_START_BEGINNING_TEXT_FALSE;
            }

            return StartAtBeginning;
        }
        public bool StartTextFound(string aValue,ref int aIntStartColumn,ref int aIntStartRow,ref int aInstance, int aColumnCounter, int aRowCounter)
        {
            bool success = false;
            bool startTextBeginDoc = false;

            //CHECK FOR BEGINNING OVERRIDE
            if (GetStartBeginningOverride() == Const.DOCUMENT_START_BEGINNING_TEXT_TRUE)
            {
                success = true;
                startTextBeginDoc = true;

                // Set the Start Row to 1
                aIntStartRow = 1;
            }
            else
            {
                if (String.IsNullOrEmpty(aValue))
                    return false;

                if (GetStartType() == Const.DOCUMENT_STOP_EXACT_INT)
                    success = (aValue == GetStartText());
                else if (GetStartType() == Const.DOCUMENT_STOP_STARTSWITH_INT)
                    success = aValue.StartsWith(GetStartText());
                else if (GetStartType() == Const.DOCUMENT_STOP_CONTAINS_INT)
                    success = aValue.Contains(GetStartText());

                if (success)
                {
                    //set the column values
                    aIntStartRow = aRowCounter;
                    aIntStartColumn = aColumnCounter;
                }
            }

            if (success)
                success = SetStart(ref aIntStartColumn, ref aIntStartRow, ref aInstance, startTextBeginDoc, aColumnCounter, aRowCounter);
            
            return success;
        }
        public string GetStartText()
        {
            if(_builder_StartText.Length == 0)
                Utilities.StringFunctions.Replace(ref _builder_StartText, _zoneConfig[Const.START_TEXT],Const.UPPER_CASE_VALUE);

            return _builder_StartText.ToString();
        }
        public int GetStartType()
        {
            if (StartTextType == Const.DEFAULT_INT_VALUE)
            {
                Utilities.StringFunctions.Replace(ref _builderStopStartOption, _zoneConfig[Const.START_TYPE]);
                if (_builderStopStartOption.ToString() == Const.DOCUMENT_STOP_EXACT)
                    StartTextType = Const.DOCUMENT_STOP_EXACT_INT;
                else if (_builderStopStartOption.ToString() == Const.DOCUMENT_STOP_STARTSWITH)
                    StartTextType = Const.DOCUMENT_STOP_STARTSWITH_INT;
                else if (_builderStopStartOption.ToString() == Const.DOCUMENT_STOP_CONTAINS)
                    StartTextType = Const.DOCUMENT_STOP_CONTAINS_INT;
            }

            return StartTextType;
        }
        private bool SetStart(ref int aIntStartColumn, ref int aIntStartRow, ref int aInstance,bool aStartTextBeginDoc, int aColumnCounter, int aRowCounter)
        {
            //START TEXT FOUND!

            if (!aStartTextBeginDoc)
            {
                //is it the correct instance
                //if not, keep walking for the start
                if (GetInstanceStartOption() != aInstance)
                    return false;
            }

            //we will process the document to find the END cordinates
            //so just return the start override value.
            if (GetStartOverride() != Const.NO_START_OVERRIDE)
            {
                aIntStartColumn = GetStartOverride();
                return true;
            }

            if (GetIncludeStart() == Const.INCLUDE_START_TEXT_FALSE)
                aIntStartColumn = aColumnCounter + 1;

            return true;
        }
        public int GetIncludeStart()
        {
            if (IncludeStartText == Const.DEFAULT_INT_VALUE)
            {
                Utilities.StringFunctions.Replace(ref _builderIncludeStart, _zoneConfig[Const.INCLUDE_START_TEXT], Const.UPPER_CASE_VALUE);

                if (_builderIncludeStart.ToString() == Const.CONFIG_DOC_YES)
                    IncludeStartText = Const.INCLUDE_START_TEXT_TRUE;
                else
                    IncludeStartText = Const.INCLUDE_START_TEXT_FALSE;
            }
            
            return IncludeStartText;
        }   
        public int GetInstanceStartOption()
        {
            if (_secondaryOption.Length == 0)
                Utilities.StringFunctions.Replace(ref _secondaryOption, _zoneConfig[Const.SECONDARY_START]);

            if (_secondaryOption.Length > 0)
                return 2;

            return 1;
        }
        public int GetStartOverride()
        {
            if (_startOverride == Const.DEFAULT_INT_VALUE)
            {
                _startOverride = Utilities.StringFunctions.ConvertToInt(_zoneConfig[Const.START_OVERRIDE], Const.NO_START_OVERRIDE);
                if (_startOverride <= 0)
                    _startOverride = Const.NO_START_OVERRIDE;
            }

            return _startOverride;
        }
        #endregion

        #region StopZone
        public int GetStopEndingOverride()
        {
            if (StopAtEnding == Const.DEFAULT_INT_VALUE)
            {
                Utilities.StringFunctions.Replace(ref _builder_StopEnding, _zoneConfig[Const.STOP_TEXT], Const.UPPER_CASE_VALUE);
                if (_builder_StopEnding.ToString() == Const.DOCUMENT_STOP_ENDING_TEXT)
                    StopAtEnding = Const.DOCUMENT_STOP_ENDING_TEXT_TRUE;
                else
                    StopAtEnding = Const.DOCUMENT_STOP_ENDING_TEXT_FALSE;
            }

            return StopAtEnding;
        }
        public bool StopTextFound(string aValue, ref int aIntStopColumn, ref int aIntStopRow, ref int aInstance, int aColumnCounter, int aRowCounter)
        {
            bool success = false;
            bool stopTextBeginDoc = false;

            if (GetStopEndingOverride() == Const.DOCUMENT_STOP_ENDING_TEXT_TRUE)
            {
                if (aIntStopRow != aRowCounter)
                {
                    if (aIntStopRow < aRowCounter)
                    {
                        // Set the End row to the larger number until we get to the max rows in the document
                        aIntStopRow = aRowCounter;
                        aIntStopColumn = aColumnCounter;
                    }
                    else
                    {
                        success = true;
                        stopTextBeginDoc = true;
                    }
                }
            }
            else
            {
                if (String.IsNullOrEmpty(aValue))
                    return false;

                if (GetStopType() == Const.DOCUMENT_STOP_EXACT_INT)
                    success = (aValue == GetStopText());
                else if (GetStopType() == Const.DOCUMENT_STOP_STARTSWITH_INT)
                    success = aValue.StartsWith(GetStopText());
                else if (GetStopType() == Const.DOCUMENT_STOP_CONTAINS_INT)
                    success = aValue.Contains(GetStopText());

                if (success)
                {
                    //set the column values
                    aIntStopRow = aRowCounter;
                    aIntStopColumn = aColumnCounter;
                }
            }

            if (success)
                success = SetStop(ref aIntStopColumn, ref aIntStopRow, ref aInstance, stopTextBeginDoc, aColumnCounter, aRowCounter);

            return success;
        }
        public string GetStopText()
        {
            if (_builder_StopText.Length == 0)
                Utilities.StringFunctions.Replace(ref _builder_StopText, _zoneConfig[Const.STOP_TEXT]);

            return _builder_StopText.ToString();
        }
        public int GetStopType()
        {
            if (StopTextType == Const.DEFAULT_INT_VALUE)
            {
                Utilities.StringFunctions.Replace(ref _builderStopStartOption, _zoneConfig[Const.STOP_TYPE]);
                if (_builderStopStartOption.ToString() == Const.DOCUMENT_STOP_EXACT)
                    StopTextType = Const.DOCUMENT_STOP_EXACT_INT;
                else if (_builderStopStartOption.ToString() == Const.DOCUMENT_STOP_STARTSWITH)
                    StopTextType = Const.DOCUMENT_STOP_STARTSWITH_INT;
                else if (_builderStopStartOption.ToString() == Const.DOCUMENT_STOP_CONTAINS)
                    StopTextType = Const.DOCUMENT_STOP_CONTAINS_INT;
            }

            return StopTextType;
        }
        private bool SetStop(ref int aIntStopColumn, ref int aIntStopRow, ref int aInstance, bool aStopTextBeginDoc, int aColumnCounter, int aRowCounter)
        {
            //START TEXT FOUND!

            if (!aStopTextBeginDoc)
            {
                //is it the correct instance
                //if not, keep walking for the start
                if (GetInstanceStopOption() != aInstance)
                    return false;
            }

            //we will process the document to find the END cordinates
            //so just return the start override value.
            if (GetStopOverride() != Const.NO_STOP_OVERRIDE)
            {
                aIntStopColumn = GetStopOverride();
                return true;
            }

            if (GetIncludeStop() == Const.INCLUDE_STOP_TEXT_FALSE)
                aIntStopColumn = aColumnCounter + 1;

            return true;
        }
        public int GetIncludeStop()
        {
            if (IncludeStopText == Const.DEFAULT_INT_VALUE)
            {
                Utilities.StringFunctions.Replace(ref _builderIncludeStop, _zoneConfig[Const.INCLUDE_STOP_TEXT], Const.UPPER_CASE_VALUE);

                if (_builderIncludeStart.ToString() == Const.CONFIG_DOC_YES)
                    IncludeStopText = Const.INCLUDE_STOP_TEXT_TRUE;
                else
                    IncludeStopText = Const.INCLUDE_STOP_TEXT_FALSE;
            }

            return IncludeStopText;
        }
        public int GetInstanceStopOption()
        {
            if (_secondaryStopOption.Length == 0)
                Utilities.StringFunctions.Replace(ref _secondaryStopOption, _zoneConfig[Const.SECONDARY_STOP]);

            if (_secondaryStopOption.Length > 0)
                return 2;

            return 1;
        }
        public int GetStopOverride()
        {
            if (_stopOverride == Const.DEFAULT_INT_VALUE)
            {
                _stopOverride = Utilities.StringFunctions.ConvertToInt(_zoneConfig[Const.STOP_OVERRIDE], Const.NO_STOP_OVERRIDE);
                if (_stopOverride <= 0)
                    _stopOverride = Const.NO_STOP_OVERRIDE;
            }
            return _stopOverride;
        }
        #endregion

        #region EndZone
        public int GetEndLastOverride()
        {
            if (EndAtLast == Const.DEFAULT_INT_VALUE)
            {
                Utilities.StringFunctions.Replace(ref _builder_StartBeginning, _zoneConfig[Const.END_TEXT], Const.UPPER_CASE_VALUE);
                if (_builder_StartBeginning.ToString() == Const.DOCUMENT_END_LAST_TEXT)
                    EndAtLast = Const.DOCUMENT_END_LAST_TEXT_TRUE;
                else
                    EndAtLast = Const.DOCUMENT_END_LAST_TEXT_FALSE;
            }

            return EndAtLast;
        }
        public bool EndTextFound(string aValue, ref int aIntEndColumn, ref int aIntEndRow, ref int aInstance, int aColumnCounter, int aRowCounter)
        {
            bool success = false;
            bool endTextBeginDoc = false;

            if (GetEndLastOverride() == Const.DOCUMENT_END_LAST_TEXT_TRUE)
            {
                if (aIntEndColumn != aColumnCounter)
                {
                    if (aIntEndColumn < aColumnCounter)
                    {
                        //get the end column
                        aIntEndColumn = aColumnCounter;
                    }
                    else
                    {
                        success = true;
                        endTextBeginDoc = true;
                    }
                }
            }
            else
            {
                if (String.IsNullOrEmpty(aValue))
                    return false;

                if (GetEndType() == Const.DOCUMENT_STOP_EXACT_INT)
                    success = (aValue == GetEndText());
                else if (GetEndType() == Const.DOCUMENT_STOP_STARTSWITH_INT)
                    success = aValue.StartsWith(GetEndText());
                else if (GetEndType() == Const.DOCUMENT_STOP_CONTAINS_INT)
                    success = aValue.Contains(GetEndText());

                if (success)
                {
                    //set the column values
                    aIntEndColumn = aColumnCounter;
                    aIntEndRow = aRowCounter;
                }
            }

            if (success)
                success = SetEnd(ref aIntEndColumn, ref aInstance, endTextBeginDoc, aColumnCounter, aRowCounter);

            return success;
        }
        public string GetEndText()
        {
            if (_builder_EndText.Length == 0)
                Utilities.StringFunctions.Replace(ref _builder_EndText, _zoneConfig[Const.END_TEXT]);

            return _builder_EndText.ToString();
        }
        public int GetEndType()
        {
            if (EndTextType == Const.DEFAULT_INT_VALUE)
            {
                Utilities.StringFunctions.Replace(ref _builderEndStartOption, _zoneConfig[Const.END_TYPE]);
                if (_builderEndStartOption.ToString() == Const.DOCUMENT_STOP_EXACT)
                    EndTextType = Const.DOCUMENT_STOP_EXACT_INT;
                else if (_builderEndStartOption.ToString() == Const.DOCUMENT_STOP_STARTSWITH)
                    EndTextType = Const.DOCUMENT_STOP_STARTSWITH_INT;
                else if (_builderEndStartOption.ToString() == Const.DOCUMENT_STOP_CONTAINS)
                    EndTextType = Const.DOCUMENT_STOP_CONTAINS_INT;
            }

            return StartTextType;
        }
        private bool SetEnd(ref int aIntEndColumn, ref int aInstance, bool aEndTextBeginDoc, int aColumnCounter, int aRowCounter)
        {
            //START TEXT FOUND!

            if (!aEndTextBeginDoc)
            {
                //is it the correct instance
                //if not, keep walking for the start
                if (GetInstanceEndOption() != aInstance)
                    return false;
            }

            //we will process the document to find the END cordinates
            //so just return the start override value.
            if (GetEndOverride() != Const.NO_END_OVERRIDE)
            {
                aIntEndColumn = GetEndOverride();
                return true;
            }

            if (GetIncludeEnd() == Const.INCLUDE_END_TEXT_FALSE)
                aIntEndColumn = aColumnCounter + 1;

            return true;
        }
        public int GetIncludeEnd()
        {
            if (IncludeEndText == Const.DEFAULT_INT_VALUE)
            {
                Utilities.StringFunctions.Replace(ref _builderIncludeEnd, _zoneConfig[Const.INCLUDE_END_TEXT], Const.UPPER_CASE_VALUE);

                if (_builderIncludeEnd.ToString() == Const.CONFIG_DOC_YES)
                    IncludeEndText = Const.INCLUDE_END_TEXT_TRUE;
                else
                    IncludeEndText = Const.INCLUDE_END_TEXT_FALSE;
            }

            return IncludeEndText;
        }
        public int GetInstanceEndOption()
        {
            if (_secondaryEndOption.Length == 0)
                Utilities.StringFunctions.Replace(ref _secondaryEndOption, _zoneConfig[Const.SECONDARY_END]);

            if (_secondaryEndOption.Length > 0)
                return 2;

            return 1;
        }
        public int GetEndOverride()
        {
            if (_endOverride == Const.DEFAULT_INT_VALUE)
            {
                _endOverride = Utilities.StringFunctions.ConvertToInt(_zoneConfig[Const.END_OVERRIDE], Const.NO_END_OVERRIDE);
                if (_endOverride <= 0)
                    _endOverride = Const.NO_END_OVERRIDE;
            }
            
            return _endOverride;
        }
        #endregion
    }
}
