using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Informa.IntelligenceOne.Parser.Extractor
{
    class Base_Extractor
    {
        protected FileInfo _fInfo;
        protected FileStream _fStream = null;

        public Base_Extractor(FileInfo aFInfo)
        {
            _fInfo = aFInfo;
        }

        public virtual void Open()
        {
            int intLockCounter = 0;


            // Check to see if the file exists - if not, jump out
            if (!File.Exists(_fInfo.FullName))
                throw new FileNotFoundException(String.Format("Exiting Process, File not Found: {0}", _fInfo.FullName));


            while (true)
            {    
                try
                {
                    if(intLockCounter > 10)
                        throw new FileNotFoundException(String.Format("Exiting Process, Unable to gain exclusive access to the file: {0}", _fInfo.FullName));

                    _fStream = File.Open(_fInfo.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                    break;
                }
                catch (Exception)
                {
                    intLockCounter++;
                    System.Threading.Thread.Sleep(5000);
                }
            }
        }

        public virtual void Extract()
        {
        }

        public virtual List<ProcessedPage> PagesToProcess()
        {
            return null;
        }
        public virtual void Cleanup()
        {
            if (_fStream != null)
            {
                try { _fStream.Close(); } catch { }
                _fStream = null;
            }
        }
    }
}
