using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Synthetic_Spectra_DLL_X64_Net_4.LoadFile;
using Synthetic_Spectra_DLL_X64_Net_4.Framework;
using System.IO;
using YafmsLibrary;
using Synthetic_Spectra_DLL_X64_Net_4.Synthetic_Spectra_Module;

namespace Synthetic_Spectra_DLL_X64_Net_4.Launch
{
    /// <summary>
    /// This is the highest level class that runs the synthetic specta generator
    /// </summary>
    public class SyntheticSpectraLaunch
    {
        /// <summary>
        /// This method runs the synthetic specta creator
        /// </summary>
        /// <param name="launchParameters">
        /// This parameter object is used to hold all the inputs that devine the model
        /// </param>
        /// <returns>
        /// returns 1 if sucessfull (not implemented yet)
        /// </returns>
        public int Launch(SyntheticSpectraLaunchParameters launchParameters)
        {
            int sucess = 0;//did this complete correctly

            int massInt64Shift = launchParameters.MassInt64Shift;
            double intensityInt64Shift = launchParameters.IntensityInt64Shift;
            string fileListLocation = launchParameters.FileListLocation;
            string fileOutputLocation = launchParameters.FileOutputLocation;
            bool outputToYafms = launchParameters.OutputToYafms;
            bool outputToTextFiles = launchParameters.OutputToTextFiles;

            outputToYafms = true;
            outputToYafms = false;
            outputToTextFiles = true;
            //fileOutputLocation = @"d:\Csharp\Syn Output\";
            //fileOutputLocation = @"G:\PNNL Files\CSharp\Syn Output\";
            

            //create a spectra object to hold the data
            SyntheticSpectraRun<Int64, double> spectraFiles = new SyntheticSpectraRun<Int64, double>(fileListLocation);

            //loads the data from the files listed in fileList and appends them to the DataProject as XYDataLists
            FileLoadFactory<Int64, double> iterateList = new FileLoadFactory<Int64, double>();
            iterateList.LoadSyntheticFiles(spectraFiles, massInt64Shift, intensityInt64Shift);

            #region deal with spectrum
            //setup parameters to run the LC dimention
            SyntheticChromatogramParameters ChromatogramParameters = new SyntheticChromatogramParameters(spectraFiles.MZParameters.NumberOfScans);
            ChromatogramParameters.MassInt64Shift = massInt64Shift;
            ChromatogramParameters.SpectraFiles = spectraFiles;
            ChromatogramParameters.GenerateWithNoise = spectraFiles.MZParameters.WithNoise;
            //for each feature entered, add a hill
            SyntheticChromatogram newChromatogram = new SyntheticChromatogram();

            for (int i = 0; i < spectraFiles.MonoIsotopicPeakList.Count; i++)
            {
                ChromatogramParameters.PeakToAdd = spectraFiles.MonoIsotopicPeakList[i];
                ChromatogramParameters.RTFWHMToAdd = spectraFiles.LCParametersRTSigma[i];
                newChromatogram.AddHill(ChromatogramParameters);
                float percentdone = (float)((float)i / (float)spectraFiles.MonoIsotopicPeakList.Count)*100;
                string updatePrint = "-->ion #" + i.ToString() + " has been added and we are " + percentdone.ToString() + "% done";
                Console.WriteLine(updatePrint);
            }

            #endregion

            #region output to real world

            List<PeakGeneric<Int64, float>> extractedSpectrum;

            if (outputToYafms)
            {
                #region YAFMS output
                Console.WriteLine("Write to Yafms");

                //string outputFile = @"d:\Csharp\Syn Output\yafmsOut.yafms";
                string outputFile =  fileOutputLocation + "yafmsOut.yafms";
                #region setup writer and make sure it will open
                try
                {
                    FileInfo TheFile = new FileInfo(outputFile);
                    if (TheFile.Exists)
                    {
                        File.Delete(outputFile);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + "\n" + @"No file was found with this reference" + "\n");
                    return -1;
                }
                #endregion

                YafmsWriter Ywriter = new YafmsWriter();

                //open connection
                Ywriter.OpenYafms(outputFile);

                #region convert to arrays and add spectra.  Note decimal shift
                for (int i = 0; i < ChromatogramParameters.Chromatogram.Count; i++)
                {
                    extractedSpectrum = ChromatogramParameters.Chromatogram[i];
                    if (extractedSpectrum.Count > 0)//if there is data in the spectra
                    {
                        double[] mz = new double[extractedSpectrum.Count];
                        float[] intensities = new float[extractedSpectrum.Count];
                        for (int d = 0; d < extractedSpectrum.Count; d++)
                        {
                            mz[d] = (double)extractedSpectrum[d].Mass / (double)massInt64Shift;
                            intensities[d] = extractedSpectrum[d].Intensity;
                        }

                        Ywriter.InsertData(1, i, 0.0F, mz, intensities, "+");
                    }
                    else
                    {
                        double[] mz = new double[0];
                        float[] intensities = new float[0];
                        Ywriter.InsertData(1, i, 0.0F, mz, intensities, "+");
                    }
                }
                #endregion

                Console.WriteLine("...Yafms written");
                
                //close connection
                Ywriter.CloseYafms();
                Console.WriteLine("...Yafms Closed");
                #endregion
            }

            if (outputToTextFiles)
            {
                #region Text files output
                Console.WriteLine("Print text files");

                for (int i = 0; i < ChromatogramParameters.Chromatogram.Count; i++)
                {
                    StringBuilder sb = new StringBuilder();
                    extractedSpectrum = ChromatogramParameters.Chromatogram[i];

                    //string outputFileDestination2 = @"d:\Csharp\Syn Output\LCSynthetic";
                    string outputFileDestination2 = fileOutputLocation + "LCSynthetic";

                    StringBuilder sb2 = new StringBuilder();

                    #region correct name so the places line up up to 999 scans
                    string name = outputFileDestination2;//starting point
                    
                    if (i < 100)
                    {
                        name = outputFileDestination2 + "0" + i.ToString() + ".txt";

                        if (i < 10)
                        {
                            name = outputFileDestination2 + "0" + i.ToString() + ".txt";
                        }
                        else
                        {
                            name = outputFileDestination2 + i.ToString() + ".txt";
                        }
                    }
                    else
                    {
                        name = outputFileDestination2 + i.ToString() + ".txt";
                    }

                    #endregion

                    using (StreamWriter writer = new StreamWriter(name))
                    {
                        for (int d = 0; d < extractedSpectrum.Count; d++)
                        {
                            sb2 = new StringBuilder();
                            sb2.Append((decimal)((decimal)extractedSpectrum[d].Mass / (decimal)massInt64Shift));//decimal shift
 
                            sb2.Append("\t");
                            sb2.Append(extractedSpectrum[d].Intensity);

                            float printedint = extractedSpectrum[d].Intensity;
                            if (printedint < 0)
                            {
                                printedint = printedint;
                            }
                            writer.WriteLine(sb2.ToString());
                        }
                    }
                }
                #endregion
            }
            #endregion

            //output data to parameters
            launchParameters.Chromatogram = ChromatogramParameters.Chromatogram;

            sucess = 1;
            return sucess;
        }   
    }
}
