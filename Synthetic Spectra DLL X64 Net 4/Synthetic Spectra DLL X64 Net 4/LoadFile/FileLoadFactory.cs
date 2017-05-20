using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using Synthetic_Spectra_DLL_X64_Net_4.Framework;
using Synthetic_Spectra_DLL_X64_Net_4.Synthetic_Spectra_Module;


namespace Synthetic_Spectra_DLL_X64_Net_4.LoadFile
{
    //TODO: make or not make generic
    /// <summary>
    /// This class loads the individual files needed and parses them into the correct format for the algorithm
    /// This is not setup to be generic because the parsing is so fixed
    /// </summary>
    /// <typeparam name="T"> not implemented </typeparam>
    /// <typeparam name="U"> not implemented </typeparam>
    public class FileLoadFactory<T,U>
        where T : struct
        where U : struct
    {
        #region Public Methods

//        //parameter1=file list with list of file paths
//        //parameter2=a load method ("full" or "line")  load all at once or one line at a time

        //this is the largest number that can be stored in Int64
        const Int64 MaxdoubleValue = 4294967295;

        //TODO:  SyntheticSpectraRun may not need to be generic
        //this function is strongly syped to double for X and double for Y
        /// <summary>
        /// This load the actual files and parses them into memory
        /// </summary>
        /// <param name="syntheticFiles">
        /// Parameter file that may not need to be generic
        /// </param>
        /// <param name="massInt64Shift">
        /// Fixed point decimal shift factor
        /// </param>
        /// <param name="intensityInt64Shift">
        /// Fixed point intensity shift factor, usually 1 because floats are good enough.
        /// </param>
        public void LoadSyntheticFiles(SyntheticSpectraRun<Int64, double> syntheticFiles, int massInt64Shift, double intensityInt64Shift)
        {
            FileLoadXYTextLine loadSpectraL = new FileLoadXYTextLine();      

            //set up what to load
            List<string> fileNames = new List<string>();

            List<SyntheticSpectraFileType> fileTypes = new List<SyntheticSpectraFileType>();

            fileNames.Add(syntheticFiles.NoiseFileBox.FileName);
            fileTypes.Add(syntheticFiles.NoiseFileBox.FileKey);

            fileNames.Add(syntheticFiles.MZParameterFileBox.FileName);
            fileTypes.Add(syntheticFiles.MZParameterFileBox.FileKey);

            fileNames.Add(syntheticFiles.TheoryFileBox.FileName);
            fileTypes.Add(syntheticFiles.TheoryFileBox.FileKey);

            fileNames.Add(syntheticFiles.LCParametersFileBox.FileName);
            fileTypes.Add(syntheticFiles.LCParametersFileBox.FileKey);

            int fileToLoadLength = fileNames.Count;
            for (int i = 0; i < fileToLoadLength; i++)//for each file required
            {
                //TODO:  error handeling if a load does not work
                //load data return string list parsed by comma.  This is nice and generic :-)
                List<List<string>> textRowList = loadSpectraL.SingleFileByLine(fileNames[i]);
                int textRowListLength = textRowList.Count;

                switch (fileTypes[i])//depending on the file type, we will parse differently and load to memory
                {
                    case SyntheticSpectraFileType.Noise:
                        {
                            #region load and parse noise spectrum to a list of XYdata points
                            List<XYDataGeneric<Int64, double>> noiseDataList = new List<XYDataGeneric<Int64, double>>();
                            Decimal massPreInt64Shift = 0;
                            double intensityPreInt64Shift = 0;
                            Int64 massPostInt64Shift = 0;
                            double intensityPostInt64Shift = 0;
                            for (int j = 1; j < textRowListLength; j++)//row 0 is the header
                            {
                                massPreInt64Shift = Decimal.Parse(textRowList[j][0]);
                                massPostInt64Shift = (Int64)((massInt64Shift * massPreInt64Shift));

                                if (massPreInt64Shift * massInt64Shift >= MaxdoubleValue)
                                {
                                    Console.WriteLine("mass is too large, we need to correct Int64 shift");
                                }

                                intensityPreInt64Shift = double.Parse(textRowList[j][1]);
                                intensityPostInt64Shift = (double)(intensityInt64Shift * intensityPreInt64Shift);

                                XYDataGeneric<Int64, double> XYDataPoint = new XYDataGeneric<Int64, double>(massPostInt64Shift, intensityPostInt64Shift);
                                noiseDataList.Add(XYDataPoint);
                            }
                            #endregion
                            syntheticFiles.NoiseSpectra = noiseDataList;//output
                        }
                        break;
                    case SyntheticSpectraFileType.MZParameters:
                        {
                            #region load and parse parameter file into a parameterobject
                            SyntheticSpectraParameters synPeaksParameters = new SyntheticSpectraParameters();
                            synPeaksParameters.StartMass = decimal.Parse(textRowList[0][1]);//first mass in range (line 1)
                            synPeaksParameters.EndMass = decimal.Parse(textRowList[1][1]);//last mass in range (line2)  this range must incorporate all data points and shoulders of neyly created peaks
                            synPeaksParameters.PeakSpacing = decimal.Parse(textRowList[2][1]);//difference between points in the mass domain (line 3)
                            synPeaksParameters.Resolution = double.Parse(textRowList[3][1]);//10,000 to 100,000  this range is important to ensure correct peak shapes (line 4)
                            synPeaksParameters.MZextendLengthFactor = int.Parse(textRowList[4][1]);//100 Extend the tails of the peak in m/z 100 times the dx
                            synPeaksParameters.PercentHanning = double.Parse(textRowList[5][1]);//0-1 where 1=100% Hanning.  0.81 is experimentally good
                            synPeaksParameters.RTSigmaMultiplier = int.Parse(textRowList[6][1]);//2-4 sigma for footprint in scan space
                            
                            string averagineTypeText = textRowList[7][1];
                            switch (averagineTypeText)
                            {
                                case "Peptide":
                                {
                                    synPeaksParameters.AveragineType = AveragineType.Peptide;
                                }
                                break;
                                case "Glycan":
                                {
                                    synPeaksParameters.AveragineType = AveragineType.Glycan;
                                }
                                    break;
                                default:
                                {
                                    synPeaksParameters.AveragineType = AveragineType.Peptide;
                                    Console.WriteLine("Unknown averagine, use Peptide or Glycan as choises");
                                }
                                break;
                            }

                            synPeaksParameters.ScanSpacing = decimal.Parse(textRowList[8][1]);//scan spacing in ms
                            synPeaksParameters.NumberOfScans = int.Parse(textRowList[9][1]);//scan number for length of LC
                            synPeaksParameters.WithNoise = bool.Parse(textRowList[10][1]);//bool true for with noise and false for no noise

                            #endregion
                            syntheticFiles.MZParameters = synPeaksParameters;//output
                        }
                        break;
                    case SyntheticSpectraFileType.TheoryData:
                        {
                            #region load and parse monoisotpic list of data
                            List<XYDataGeneric<Int64, double>> theoryDataList = new List<XYDataGeneric<Int64, double>>();
                            List<int> chargeStateList = new List<int>();
                            Decimal massPreInt64ShiftTheory = 0;
                            double intensityPreInt64ShiftTheory = 0;
                            Int64 massPostInt64ShiftTheory = 0;
                            double intensityPostInt64ShiftTheory = 0;
                            for (int j = 1; j < textRowListLength; j++)//row 0 is the header
                            {
                                #region load monoisotopic list
                                massPreInt64ShiftTheory = Decimal.Parse(textRowList[j][0]);
                                massPostInt64ShiftTheory = (Int64)(massInt64Shift * massPreInt64ShiftTheory);

                                if (massPreInt64ShiftTheory * massInt64Shift >= MaxdoubleValue)
                                {
                                    Console.WriteLine("mass is too large, we need to correct Int64 shift");
                                }

                                intensityPreInt64ShiftTheory = double.Parse(textRowList[j][1]);
                                intensityPostInt64ShiftTheory = (double)(intensityInt64Shift * intensityPreInt64ShiftTheory);

                                XYDataGeneric<Int64, double> XYDataPoint = new XYDataGeneric<Int64, double>(massPostInt64ShiftTheory, intensityPostInt64ShiftTheory);
                                theoryDataList.Add(XYDataPoint);
                                #endregion

                                #region load charge state list
                                chargeStateList.Add(int.Parse(textRowList[j][2]));
                                #endregion
                            }
                            #endregion
                            syntheticFiles.MonoIsotopicPeakList = theoryDataList;//output
                            syntheticFiles.TheoryChargeStateList = chargeStateList;//output
                        }
                        break;
                    case SyntheticSpectraFileType.LCParameters:
                        {
                            #region load and parse retention time and full width half maximum retention times come in as scan number
                            List<XYDataGeneric<int,int>> RTSigmaList = new List<XYDataGeneric<int,int>>();
                            int scannumber = 0;//load data as scan number
                            int sigma = 0;//in Number of scans
                            for (int j = 1; j < textRowListLength; j++)//row 0 is the header
                            {
                                scannumber = int.Parse(textRowList[j][1]);
                                sigma = int.Parse(textRowList[j][2]);
                                XYDataGeneric<int, int> newDataPointRTFWHM = new XYDataGeneric<int, int>(scannumber, sigma);
                                RTSigmaList.Add(newDataPointRTFWHM);
                            }
                            #endregion
                            syntheticFiles.LCParametersRTSigma = RTSigmaList;
                        }
                        break;
                    default:
                        {
                            Console.WriteLine("funny load type, Danger (fileLoadFactory)");
                        }
                        break;
                }
            }
        }
        #endregion
    }
}
