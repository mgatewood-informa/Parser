using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using Informa.IntelligenceOne.Parser.DAL.Core;
using Informa.IntelligenceOne.Parser.DAL;
using Informa.IntelligenceOne.Parser.Const;
using Informa.IntelligenceOne.Parser.Extractor;
using Informa.IntelligenceOne.Parser.Processor;

using Microsoft.Extensions.DependencyInjection;

namespace Informa.IntelligenceOne.Parser
{
    class FileParser
    {
        ServiceCollection _serviceCollection;

        public FileParser()
        {
            ConfigureServices();
        }

        public void ConfigureServices()
        {
            _serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

            Informa.IntelligenceOne.Parser.DAL.Configure.ConfigureServices(_serviceCollection, ConfigurationManager.AppSettings["connectionString"]);

            _serviceCollection.AddTransient<IParserService, ParserService>();
        }
        
        public void Parse(String filePathUri)
        {
            string fileType;

            Base_Extractor fileExtractor;
            Base_Processor fileProcesser;

            FileInfo fInfo;


            if (File.Exists(filePathUri))
            {
                //this will determine the extension type
                fInfo = new FileInfo(filePathUri);
                fileType = fInfo.Extension;

                //call the extractor logic that will return the file as Json
                if (fileType == ProcessFileType.Pdf)
                    fileExtractor = new PDF(fInfo);
                else if (fileType.StartsWith(ProcessFileType.Excel))
                    fileExtractor = new Excel(fInfo,null);
                else
                    throw new FileNotFoundException(String.Format("Invalid file type: {0}", filePathUri));


                try
                {
                    fileExtractor.Open();
                    fileExtractor.Extract();

                   
                    fileProcesser = new Base_Processor(
                        _serviceCollection.BuildServiceProvider().GetRequiredService<IParserService>(),
                        fileExtractor.PagesToProcess(),
                        1, fInfo.Name);
                    
                    fileProcesser.Go();
                    
                    
                    fileProcesser.Cleanup();
                }
                finally
                {
                    fileExtractor.Cleanup();
                }
            }

        }
    }
}
