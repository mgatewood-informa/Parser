using Informa.IntelligenceOne.Parser.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;

namespace Informa.IntelligenceOne.Parser.Extractor
{
    class Excel : Base_Extractor
    {
        List<String> _sheetsToProcess;
        List<ProcessedPage> _sheetsProcessed;
        OleDbConnection _conn;

        public Excel(FileInfo fInfo, List<String> sheetsToProcess) : base(fInfo)
        {
            _sheetsToProcess = sheetsToProcess;
        }

        /// <summary>
        /// Gain exclusive access to the file
        /// </summary>
        public override void Open()
        {
            _conn = new OleDbConnection(GetOLDBConnectionString());
            _conn.Open();
        }
        
        protected String GetOLDBConnectionString()
        {
            // Using the filename, get the data from the specified sheet
            // Convert the data to Json and return as an Array
            StringBuilder connectionString = new StringBuilder(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source='");
            connectionString.Append(_fInfo.FullName);
            connectionString.Append("';Extended Properties=");
            connectionString.Append(@"""");
            connectionString.Append("Excel 12.0 Xml;HDR=NO;IMEX=1;MAXSCANROWS=0;");
            connectionString.Append(@"""");

            //@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source='c:\Temp\WF.xlsx';Extended Properties=""Excel 12.0;HDR=NO;""";

            return connectionString.ToString();
        }

        private void GetAllSheets()
        {
            StringBuilder tableName = null;
            _sheetsToProcess = new List<string>();


            DataTable testTable = _conn.GetSchema("Tables");

            int i = 0;

            foreach (DataRow row in testTable.Rows)
            {
                Utilities.StringFunctions.Replace(ref tableName, row["TABLE_NAME"].ToString());
                
                if (tableName.ToString().EndsWith("$"))
                {
                    _sheetsToProcess.Add(tableName.ToString());
                    i++;
                }
                else
                {
                    Utilities.StringFunctions.Replace(ref tableName,
                    tableName.ToString().Substring(1, tableName.ToString().LastIndexOf('$')));

                    if (tableName.ToString().EndsWith("$"))
                        _sheetsToProcess.Add(tableName.ToString());

                        i++;
                }

            }
        }

        public override List<ProcessedPage> PagesToProcess()
        {
            return _sheetsProcessed;
        }

        public override void Extract()
        {          
            StringBuilder commandText = null;

            if (_sheetsToProcess == null)
                GetAllSheets();

            if (_sheetsToProcess != null)
            {
                if (_sheetsToProcess.Count > 0)
                {
                    //init if sheets are found to process
                    _sheetsProcessed = new List<ProcessedPage>();

                    for (int x = 0; x < _sheetsToProcess.Count; x++)
                    {
                        Utilities.StringFunctions.Replace(ref commandText, "select * from [");
                        commandText.Append(_sheetsToProcess[x]);
                        commandText.Append("]");

                        // Get the data from the sheet that was passed in
                        OleDbDataAdapter objDA = new System.Data.OleDb.OleDbDataAdapter
                        (commandText.ToString(), _conn);

                        DataSet excelDataSet = new DataSet();
                        objDA.Fill(excelDataSet);
                        DataTable dt = excelDataSet.Tables[0];

                        _sheetsProcessed.Add(new ProcessedPage(_sheetsToProcess[x],
                        JArray.Parse(JsonConvert.SerializeObject(dt))));
                    }

                }
            }
        }

        public override void Cleanup()
        {
            base.Cleanup();

            if (_conn != null)
            {
                _conn.Close();
                _conn.Dispose();

                _conn = null;
            }

            if (_sheetsToProcess != null)
                _sheetsToProcess = null;

            if (_sheetsProcessed != null)
            {
                _sheetsProcessed.Clear();
                _sheetsProcessed = null;
            }

        }
    }
}
