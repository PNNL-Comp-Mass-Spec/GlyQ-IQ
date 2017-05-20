using System;
using System.Collections.Generic;
using GetPeaksDllLite.Functions;
using GetPeaksDllLite.Objects;

namespace GetPeaksDllLite.DataFIFO.DeconFIFO
{
    public class PeaksImporter
    {
        public List<PeakDecon> ImportPeak(string fileName)
        {
            DeliminatorOfString deliminatorFiletype;
            deliminatorFiletype = DeliminatorOfString.Tab;

            StringLoadTextFileLine loadSpectraL = null;
            loadSpectraL = new StringLoadTextFileLine();
            List<string> stringListFromFiles = new List<string>();

            //load strings
            stringListFromFiles = loadSpectraL.SingleFileByLine(fileName);//loads all peaks

            Console.WriteLine("Load file: " + fileName + "\n");

            //grab column header
            string isotopeColumnHeader = stringListFromFiles[0];

            #region load data and parse it
            ParsePeaksToPeakDecon newParser = new ParsePeaksToPeakDecon();
            
            string columnHeaders;
            //List<PeakDecon> loadedPeakData;

            List<PeakDecon> loadedPeakData = new List<PeakDecon>(); ;
            newParser.Parse(stringListFromFiles, deliminatorFiletype, out loadedPeakData, out columnHeaders);

            //List<PeakDeconLite> loadedPeakData = new List<PeakDeconLite>(); ;
            //newParser.ParseLite(stringListFromFiles, deliminatorFiletype, out loadedPeakData, out columnHeaders);

            //List<double> loadedPeakData = new List<double>(); ;
            //newParser.ParseMass(stringListFromFiles, deliminatorFiletype, out loadedPeakData, out columnHeaders);

            #endregion

            return loadedPeakData;
        }

        public List<PeakDeconLite> ImportPeakLite(string fileName)
        {
            DeliminatorOfString deliminatorFiletype;
            deliminatorFiletype = DeliminatorOfString.Tab;

            StringLoadTextFileLine loadSpectraL = null;
            loadSpectraL = new StringLoadTextFileLine();
            List<string> stringListFromFiles = new List<string>();

            //load strings
            stringListFromFiles = loadSpectraL.SingleFileByLine(fileName);//loads all peaks

            Console.WriteLine("Load file: " + fileName + "\n");

            //grab column header
            string isotopeColumnHeader = stringListFromFiles[0];

            #region load data and parse it
            ParsePeaksToPeakDecon newParser = new ParsePeaksToPeakDecon();

            string columnHeaders;
            //List<PeakDecon> loadedPeakData;

            //List<PeakDecon> loadedPeakData = new List<PeakDecon>(); ;
            //newParser.Parse(stringListFromFiles, deliminatorFiletype, out loadedPeakData, out columnHeaders);

            List<PeakDeconLite> loadedPeakData = new List<PeakDeconLite>(); ;
            newParser.ParseLite(stringListFromFiles, deliminatorFiletype, out loadedPeakData, out columnHeaders);

            //List<double> loadedPeakData = new List<double>(); ;
            //newParser.ParseMass(stringListFromFiles, deliminatorFiletype, out loadedPeakData, out columnHeaders);

            #endregion

            return loadedPeakData;
        }

        public List<long> ImportMass(string fileName)
        {
            DeliminatorOfString deliminatorFiletype;
            deliminatorFiletype = DeliminatorOfString.Tab;

            StringLoadTextFileLine loadSpectraL = null;
            loadSpectraL = new StringLoadTextFileLine();
            List<string> stringListFromFiles = new List<string>();

            //load strings
            stringListFromFiles = loadSpectraL.SingleFileByLine(fileName);//loads all peaks

            Console.WriteLine("Load file: " + fileName + "\n");

            //grab column header
            string isotopeColumnHeader = stringListFromFiles[0];

            #region load data and parse it
            ParsePeaksToPeakDecon newParser = new ParsePeaksToPeakDecon();

            string columnHeaders;
            //List<PeakDecon> loadedPeakData;

            //List<PeakDecon> loadedPeakData = new List<PeakDecon>(); ;
            //newParser.Parse(stringListFromFiles, deliminatorFiletype, out loadedPeakData, out columnHeaders);

            //List<PeakDeconLite> loadedPeakData = new List<PeakDeconLite>(); ;
            //newParser.ParseLite(stringListFromFiles, deliminatorFiletype, out loadedPeakData, out columnHeaders);

            List<long> loadedPeakData = new List<long>(); ;
            newParser.ParseMass(stringListFromFiles, deliminatorFiletype, out loadedPeakData, out columnHeaders);

            #endregion

            return loadedPeakData;
        }    
    }
}
