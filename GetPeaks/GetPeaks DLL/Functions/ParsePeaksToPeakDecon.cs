using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.DataFIFO;
using GetPeaks_DLL.Objects;

namespace GetPeaks_DLL.Functions
{
    public class ParsePeaksToPeakDecon
    {
        public void Parse(List<string> stringList, FileIterator.deliminator textDeliminator, out List<PeakDecon> outputPeakList, out string columnHeadders)
        {
            int startline = 0;

            outputPeakList = new List<PeakDecon>();

            string[] wordArray;

            int peak_index = 0;
            int scan_num = 0;
            double mz  = 0;
            double intensity = 0;
            float fwhm = 0;
            float signal_noise = 0;
            int MSFeatureID = 0;

            char spliter;
            switch (textDeliminator)
            {
                case GetPeaks_DLL.DataFIFO.FileIterator.deliminator.Comma:
                    {
                        spliter = ',';
                    }
                    break;
                case GetPeaks_DLL.DataFIFO.FileIterator.deliminator.Tab:
                    {
                        spliter = '\t';
                    }
                    break;
                default:
                    {
                        spliter = ',';
                    }
                    break;
            }

            columnHeadders = 
                "peak_index" + spliter +
                "scan_num" + spliter +
                "mz" + spliter +
                "intensity" + spliter +
                "fwhm" + spliter +
                "signal_noise" + spliter +
                "MSFeatureID";

            int length = stringList.Count;
            for (int i = startline; i < length; i++)//i=0 is the headder
            {
                string line = stringList[i];

                wordArray = line.Split(spliter);

                double.TryParse(wordArray[2], out mz);//this will make the header skip

                if (mz > 0)//this will make the header skip
                {
                    //Tryparse is best and should be fastest

                    int.TryParse(wordArray[0], out peak_index);
                    int.TryParse(wordArray[1], out scan_num);
                    double.TryParse(wordArray[2], out mz);
                    double.TryParse(wordArray[3], out intensity);
                    float.TryParse(wordArray[4], out fwhm);
                    float.TryParse(wordArray[5], out signal_noise);
                    int.TryParse(wordArray[6], out MSFeatureID);

                    PeakDecon newPeak = new PeakDecon();
                    newPeak.peak_index = peak_index;
                    newPeak.scan_num = scan_num;
                    newPeak.mz = mz;
                    newPeak.intensity = intensity;
                    newPeak.fwhm = fwhm;
                    newPeak.signal_noise = signal_noise;
                    newPeak.MSFeatureID = MSFeatureID;

                    outputPeakList.Add(newPeak);
                }
            }
        }

        public void ParseLite(List<string> stringList, FileIterator.deliminator textDeliminator, out List<PeakDeconLite> outputPeakList, out string columnHeadders)
        {
            int startline = 1;

            outputPeakList = new List<PeakDeconLite>();

            string[] wordArray;

            string wordsAtStart = "";
            double mz = 0;
            string wordsAtEnd = "";

            string word0 ="";
            string word1 = "";
            string word3 = "";
            string word4 = "";
            string word5 = "";
            string word6 = "";

            char spliter;
            switch (textDeliminator)
            {
                case GetPeaks_DLL.DataFIFO.FileIterator.deliminator.Comma:
                    {
                        spliter = ',';
                    }
                    break;
                case GetPeaks_DLL.DataFIFO.FileIterator.deliminator.Tab:
                    {
                        spliter = '\t';
                    }
                    break;
                default:
                    {
                        spliter = ',';
                    }
                    break;
            }

            columnHeadders = "X" + spliter + "Y";

            int length = stringList.Count;
            for (int i = startline; i < length; i++)//i=0 is the headder
            {
                string line = stringList[i];

                wordArray = line.Split(spliter);

                //Tryparse is best and should be fastest

                word0 = wordArray[0];
                word1 = wordArray[1];
                double.TryParse(wordArray[2], out mz);
                word3 = wordArray[3];
                word4 = wordArray[4];
                word5 = wordArray[5];
                word6 = wordArray[6];

                wordsAtStart = word0 + "\t" + word1 + "\t";
                wordsAtEnd = word3 + "\t" + word4 + "\t" + word5 + "\t" + word6;
                PeakDeconLite newPeak = new PeakDeconLite();
                newPeak.start = wordsAtStart;
                newPeak.mz = mz;
                newPeak.end = wordsAtEnd;

                outputPeakList.Add(newPeak);
            }
        }

        public void ParseMass(List<string> stringList, FileIterator.deliminator textDeliminator, out List<long> outputMassList, out string columnHeadders)
        {
            int startline = 1;

            outputMassList = new List<long>();

            string[] wordArray;

            string wordsAtStart = "";
            long mz = 0;
            string wordsAtEnd = "";

            string word0 = "";
            string word1 = "";
            string word3 = "";
            string word4 = "";
            string word5 = "";
            string word6 = "";

            char spliter;
            switch (textDeliminator)
            {
                case GetPeaks_DLL.DataFIFO.FileIterator.deliminator.Comma:
                    {
                        spliter = ',';
                    }
                    break;
                case GetPeaks_DLL.DataFIFO.FileIterator.deliminator.Tab:
                    {
                        spliter = '\t';
                    }
                    break;
                default:
                    {
                        spliter = ',';
                    }
                    break;
            }

            columnHeadders = "X" + spliter + "Y";

            int length = stringList.Count;
            for (int i = startline; i < length; i++)//i=0 is the headder
            {
                string line = stringList[i];

                wordArray = line.Split(spliter);

                //Tryparse is best and should be fastest

                word0 = wordArray[0];
                word1 = wordArray[1];
                long.TryParse(wordArray[2], out mz);
                word3 = wordArray[3];
                word4 = wordArray[4];
                word5 = wordArray[5];
                word6 = wordArray[6];

                wordsAtStart = word0 + "\t" + word1 + "\t";
                wordsAtEnd = word3 + "\t" + word4 + "\t" + word5 + "\t" + word6;
                PeakDeconLite newPeak = new PeakDeconLite();
                newPeak.start = wordsAtStart;
                newPeak.mz = mz;
                newPeak.end = wordsAtEnd;

                outputMassList.Add(mz);
            }
        }
    }
}
