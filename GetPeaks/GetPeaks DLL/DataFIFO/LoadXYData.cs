using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using GetPeaks_DLL.Functions;

namespace GetPeaks_DLL.DataFIFO
{
    public class LoadXYData
    {
        public List<XYData> Import(string fileName, out string columnHeaders)
        {
            List<XYData> xyDataList = new List<XYData>();
            GetPeaks_DLL.DataFIFO.FileIterator.deliminator deliminatorFiletype;
            deliminatorFiletype = FileIterator.deliminator.Tab;

            StringLoadTextFileLine loadSpectraL = null;
            loadSpectraL = new StringLoadTextFileLine();
            List<string> stringListFromFiles = new List<string>();

            //load strings
            stringListFromFiles = loadSpectraL.SingleFileByLine(fileName);//loads all isos

            Console.WriteLine("Load file: " + fileName + "\n");

            //grab column header
            string isotopeColumnHeader = stringListFromFiles[0];

            #region load data and parse it
            ParseStringListToXYData newParser = new ParseStringListToXYData();

            List<XYData> loadedXYData;

            newParser.Parse(stringListFromFiles, deliminatorFiletype, out loadedXYData, out columnHeaders);

            #endregion

            return loadedXYData;
        }

        public List<XYData> Import(string fileName)
        {
            List<XYData> xyDataList = new List<XYData>();
            GetPeaks_DLL.DataFIFO.FileIterator.deliminator deliminatorFiletype;
            deliminatorFiletype = FileIterator.deliminator.Tab;

            StringLoadTextFileLine loadSpectraL = null;
            loadSpectraL = new StringLoadTextFileLine();
            List<string> stringListFromFiles = new List<string>();

            //load strings
            stringListFromFiles = loadSpectraL.SingleFileByLine(fileName);//loads all isos

            Console.WriteLine("Load file: " + fileName + "\n");

            //grab column header
            //string isotopeColumnHeader = stringListFromFiles[0];

            #region load data and parse it
            ParseStringListToXYData newParser = new ParseStringListToXYData();

            List<XYData> loadedXYData;

            newParser.Parse(stringListFromFiles, deliminatorFiletype, out loadedXYData);

            #endregion

            return loadedXYData;
        }

        public List<XYData> Import(string fileName, GetPeaks_DLL.DataFIFO.FileIterator.deliminator deliminatorFiletype)
        {
            List<XYData> xyDataList = new List<XYData>();
            
            StringLoadTextFileLine loadSpectraL = null;
            loadSpectraL = new StringLoadTextFileLine();
            List<string> stringListFromFiles = new List<string>();

            //load strings
            stringListFromFiles = loadSpectraL.SingleFileByLine(fileName);//loads all isos

            Console.WriteLine("Load file: " + fileName + "\n");

            //grab column header
            //string isotopeColumnHeader = stringListFromFiles[0];

            #region load data and parse it
            ParseStringListToXYData newParser = new ParseStringListToXYData();

            List<XYData> loadedXYData;

            newParser.Parse(stringListFromFiles, deliminatorFiletype, out loadedXYData);

            #endregion

            return loadedXYData;
        }
    }
}
