using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Functions;

namespace GetPeaks_DLL.DataFIFO
{
    public class LoadScanList
    {
        public List<int> Import(string fileName, out string columnHeaders)
        {
            List<int> xyDataList = new List<int>();
            GetPeaks_DLL.DataFIFO.FileIterator.deliminator deliminatorFiletype;
            deliminatorFiletype = FileIterator.deliminator.Comma;

            StringLoadTextFileLine loadSpectraL = null;
            loadSpectraL = new StringLoadTextFileLine();
            List<string> stringListFromFiles = new List<string>();

            //load strings
            stringListFromFiles = loadSpectraL.SingleFileByLine(fileName);//loads all isos

            Console.WriteLine("Load file: " + fileName + "\n");

            //grab column header
            string isotopeColumnHeader = stringListFromFiles[0];

            #region load data and parse it
            ParseScanFileToInt newParser = new ParseScanFileToInt();

            List<int> loadedData;

            newParser.Parse(stringListFromFiles, deliminatorFiletype, out loadedData, out columnHeaders);

            #endregion

            return loadedData;
        }
    }
}
