using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects;
using System.Linq.Expressions;
using PNNLOmics.Data;
using PNNLOmics.Data.Constants;
using GetPeaks_DLL.Objects.DifferenceFinderObjects;

namespace GetPeaks_DLL.Functions
{
    public class DifferenceFinder
    {
        public List<double> GlycanDifferences { get; set; }

        /// <summary>
        /// this int can be anything
        /// </summary>
        /// <param name="forceMonos">this int can be anything</param>
        public DifferenceFinder(int forceMonos)
        {
            GlycanDifferences = new List<double>();
            GlycanDifferences.Add(Constants.Monosaccharides[MonosaccharideName.Deoxyhexose].MassMonoIsotopic);
            GlycanDifferences.Add(Constants.Monosaccharides[MonosaccharideName.Hexose].MassMonoIsotopic);
            GlycanDifferences.Add(Constants.Monosaccharides[MonosaccharideName.HexuronicAcid].MassMonoIsotopic);
            GlycanDifferences.Add(Constants.Monosaccharides[MonosaccharideName.KDN].MassMonoIsotopic);
            GlycanDifferences.Add(Constants.Monosaccharides[MonosaccharideName.NAcetylhexosamine].MassMonoIsotopic);
            GlycanDifferences.Add(Constants.Monosaccharides[MonosaccharideName.NeuraminicAcid].MassMonoIsotopic);
            GlycanDifferences.Add(Constants.Monosaccharides[MonosaccharideName.NGlycolylneuraminicAcid].MassMonoIsotopic);
            GlycanDifferences.Add(Constants.Monosaccharides[MonosaccharideName.Pentose].MassMonoIsotopic);
        }

        public DifferenceFinder()
        {
            GlycanDifferences = new List<double>();
        }

        public DifferenceFinder(List<double> inputDifferences)
        {
            GlycanDifferences = inputDifferences;
        }
        
        /// <summary>
        /// Process a list of doubles
        /// </summary>
        /// <param name="differences">list of double differences</param>
        /// <param name="dataXY">List of data as doubles</param>
        /// <param name="ppmError">ppm error, e.g. 10</param>
        /// <returns>results list</returns>
        public List<DifferenceObject<double>> FindDifferences(List<double> differences, ref List<double> data, double ppmError)
        {
            double holdValue = 0;
            double testValue = 0;
            double dataPoint = 0;
            double tolerance = 0;
            
            List<DifferenceObject<double>> results = new List<DifferenceObject<double>>();

            for (int i = 0; i < differences.Count; i++)
            {
                for (int j = 0; j < data.Count; j++)
                {
                    holdValue = data[j];
                    testValue = data[j] + differences[i];

                    tolerance = ErrorCalculator.PPMtoDaTollerance(ppmError, holdValue);

                    dataPoint = data.FirstOrDefault(c => c > testValue - tolerance & c < testValue + tolerance);

                    if (dataPoint > 0)
                    {
                        DifferenceObject<double> newMatch = new DifferenceObject<double>();
                        newMatch.IndexData = data.BinarySearch(holdValue);
                        newMatch.IndexMatch = data.FindIndex(c => c > testValue - tolerance & c < testValue + tolerance);
                        //newMatch.Index2 = data.BinarySearch(testValue);
                        newMatch.Value1 = holdValue;
                        newMatch.Value2 = dataPoint;
                        newMatch.Difference = differences[i];
                        newMatch.DifferenceIndex = i;
                        results.Add(newMatch);
                    }
                }
            }

            return results;
        }

        public List<DifferenceObject<double>> FindDifferencesDa(List<double> differences, ref List<double> data, double DaError)
        {
            double holdValue = 0;
            double testValue = 0;
            double dataPoint = 0;
            double tolerance = 0;

            List<DifferenceObject<double>> results = new List<DifferenceObject<double>>();

            for (int i = 0; i < differences.Count; i++)
            {
                for (int j = 0; j < data.Count; j++)
                {
                    holdValue = data[j];
                    testValue = data[j] + differences[i];

                    tolerance = DaError;

                    dataPoint = data.FirstOrDefault(c => c > testValue - tolerance & c < testValue + tolerance);

                    if (dataPoint > 0)
                    {
                        DifferenceObject<double> newMatch = new DifferenceObject<double>();
                        newMatch.IndexData = data.BinarySearch(holdValue);
                        newMatch.IndexMatch = data.FindIndex(c => c > testValue - tolerance & c < testValue + tolerance);
                        //newMatch.Index2 = data.BinarySearch(testValue);
                        newMatch.Value1 = holdValue;
                        newMatch.Value2 = dataPoint;
                        newMatch.Difference = differences[i];
                        newMatch.DifferenceIndex = i;
                        results.Add(newMatch);
                    }
                }
            }

            return results;
        }

        public List<DifferenceObject<double>> FindDifferencesDa(List<double> differences, ref List<double> data, List<double> DaError)
        {
            double holdValue = 0;
            double testValue = 0;
            double dataPoint = 0;
            double tolerance = 0;

            List<DifferenceObject<double>> results = new List<DifferenceObject<double>>();

            for (int i = 0; i < differences.Count; i++)
            {
                for (int j = 0; j < data.Count; j++)
                {
                    holdValue = data[j];
                    testValue = data[j] + differences[i];

                    tolerance = DaError[i];

                    dataPoint = data.FirstOrDefault(c => c > testValue - tolerance & c < testValue + tolerance);

                    if (dataPoint > 0)
                    {
                        DifferenceObject<double> newMatch = new DifferenceObject<double>();
                        newMatch.IndexData = data.BinarySearch(holdValue);
                        newMatch.IndexMatch = data.FindIndex(c => c > testValue - tolerance & c < testValue + tolerance);
                        //newMatch.Index2 = data.BinarySearch(testValue);
                        newMatch.Value1 = holdValue;
                        newMatch.Value2 = dataPoint;
                        newMatch.Difference = differences[i];
                        newMatch.DifferenceIndex = i;
                        results.Add(newMatch);
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// overload taking a list of XYData from Omics
        /// </summary>
        /// <param name="differences">list of double differences</param>
        /// <param name="dataXY">List of data in XYData.X</param>
        /// <param name="ppmError">ppm error, e.g. 10</param>
        /// <returns>results list</returns>
        public List<DifferenceObject<double>> FindDifferences(List<double> differences, ref List<XYData> dataXY, double ppmError)
        {
            List<double> data = this.ConvertXYDataToDouble(dataXY);

            List<DifferenceObject<double>> resultsOverall = this.FindDifferences(differences, ref data, ppmError);

            return resultsOverall;
        }

        /// <summary>
        /// overload taking a list of Processedpeaks from Omics
        /// </summary>
        /// <param name="differences">list of double differences</param>
        /// <param name="dataXY">List of data in XYData.X</param>
        /// <param name="ppmError">ppm error, e.g. 10</param>
        /// <returns>results list</returns>
        public List<DifferenceObject<double>> FindDifferences(List<double> differences, ref List<ProcessedPeak> dataPP, double ppmError)
        {
            List<double> data = this.ConvertProcessedPeakToDouble(dataPP);

            List<DifferenceObject<double>> resultsOverall = this.FindDifferences(differences, ref data, ppmError);

            return resultsOverall;
        } 


        public List<double> ConvertXYDataToDouble(List<XYData> dataListXY)
        {
            List<double> outList = dataListXY.ConvertAll<double>(delegate(XYData str)
            {
                return (double)str.X;
            });
            return outList;
        }

        public List<double> ConvertProcessedPeakToDouble(List<ProcessedPeak> dataListPP)
        {
            List<double> outList = dataListPP.ConvertAll<double>(delegate(ProcessedPeak str)
            {
                return (double)str.XValue;
            });
            return outList;
        }
    }
}
