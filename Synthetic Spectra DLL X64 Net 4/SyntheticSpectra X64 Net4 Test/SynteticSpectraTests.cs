using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Synthetic_Spectra_DLL_X64_Net_4.Launch;
using Synthetic_Spectra_DLL_X64_Net_4.Framework;
using Synthetic_Spectra_DLL_X64_Net_4.LoadFile;
using Synthetic_Spectra_DLL_X64_Net_4.Synthetic_Spectra_Module;
using CompareContrast_DLL;
using System.IO;
using YafmsLibrary;



namespace SyntheticSpectraTests
{
    public class SynSpectraCreatorTests
    {
        [Test]
        public void testChromatogramFromSimplifedCall()
        {
            int massInt64Shift = 100000;//  1000.12345 will be 100012345
            double intensityInt64Shift = 1;
            string fileListLocation;
            string outputFileDestination = "";

            if (1 == 1)
            {
                //work
                fileListLocation = @"D:\Csharp\Syn Output\ParameterFiles\0_FileLoadIndex5Syn.txt";
                outputFileDestination = @"d:\Csharp\Syn Output\";
            }
            else
            {
                //home
                fileListLocation = @"G:\PNNL Files\CSharp\Syn Output\ParameterFiles\0_HomeFileLoadIndex5Syn.txt";
                outputFileDestination = @"g:\PNNL Files\Csharp\Syn Output\";
            }
            
            SyntheticSpectraLaunchParameters ParametersApplicationLevel = new SyntheticSpectraLaunchParameters();
            ParametersApplicationLevel.FileListLocation = fileListLocation;
            ParametersApplicationLevel.FileOutputLocation = outputFileDestination;
            ParametersApplicationLevel.MassInt64Shift = massInt64Shift;
            ParametersApplicationLevel.IntensityInt64Shift = intensityInt64Shift;
            ParametersApplicationLevel.OutputToTextFiles = true;
            ParametersApplicationLevel.OutputToYafms = true;
            SyntheticSpectraLaunch getMeASpectra = new SyntheticSpectraLaunch();
            getMeASpectra.Launch(ParametersApplicationLevel);
            

            Assert.AreEqual(10, ParametersApplicationLevel.Chromatogram.Count);
            //Assert.AreEqual(1, SpectraFiles.MonoIsotopicPeakList.Count);
        }

        
        [Test]
        public void testChromatogram()
        {
            int massInt64Shift = 100000;//  1000.12345 will be 100012345
            double intensityInt64Shift = 1;
            string fileListLocation;
            string outputFileDestination = "";

            if(1==1)
            {
                fileListLocation = @"D:\Csharp\Syn Output\ParameterFiles\0_FileLoadIndex5Syn.txt";
                outputFileDestination = @"d:\Csharp\Syn Output\LCSynthetic.txt";
            }
            else
            {
                fileListLocation = @"g:\PNNL Files\Csharp\0_HomeFileLoadIndex5Syn.txt";
                outputFileDestination = @"g:\PNNL Files\Csharp\Syn Output\LCSynthetic.txt";
            }
            //create object to hold the data
            SyntheticSpectraRun<Int64, double> SpectraFiles = new SyntheticSpectraRun<Int64, double>(fileListLocation);

            //string dataToUse = @"D:\Csharp\CSV Glycan Test3 10K.csv";
            //SpectraFiles.TheoryFileBox.FileName = dataToUse;

            //loads the data from the files listed in fileList and appends them to the DataProject as XYDataLists
            FileLoadFactory<Int64, double> iterateList = new FileLoadFactory<Int64, double>();
            iterateList.LoadSyntheticFiles(SpectraFiles, massInt64Shift, intensityInt64Shift);

            //SpectraFiles.Parameters.StartMass = 389;
            //SpectraFiles.Parameters.EndMass = 2100;

            #region deal with spectrum

            SyntheticChromatogramParameters ChromatogramParameters = new SyntheticChromatogramParameters(SpectraFiles.MZParameters.NumberOfScans);
            ChromatogramParameters.MassInt64Shift = massInt64Shift;
            ChromatogramParameters.SpectraFiles = SpectraFiles;
            SyntheticChromatogram newChromatogram = new SyntheticChromatogram();

            for (int i = 0; i < SpectraFiles.MonoIsotopicPeakList.Count; i++)
            {
                ChromatogramParameters.PeakToAdd = SpectraFiles.MonoIsotopicPeakList[i];
                ChromatogramParameters.RTFWHMToAdd = SpectraFiles.LCParametersRTSigma[i];
                newChromatogram.AddHill(ChromatogramParameters);
            }

            #endregion

            //output to real world
            bool textWriteData = false;
            textWriteData = true;
            bool yafmsWriteData = false;
            yafmsWriteData = true;

            List<PeakGeneric<Int64, float>> extractedSpectrum;

            if (yafmsWriteData)
            {
                Console.WriteLine("Write to Yafms");
                
                string outputFile = @"d:\Csharp\Syn Output\yafmsOut.yafms";

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
                    return;
                }
                #endregion

                YafmsWriter Ywriter = new YafmsWriter();

                //open connection
                Ywriter.OpenYafms(outputFile);

                #region convert to arrays and add spectra
                for (int i = 0; i < ChromatogramParameters.Chromatogram.Count; i++)
                {
                    extractedSpectrum = ChromatogramParameters.Chromatogram[i];                 
                    if (extractedSpectrum.Count > 0)//if there is data in the spectra
                    {
                        double[] mz = new double[extractedSpectrum.Count];
                        float[] intensities = new float[extractedSpectrum.Count];
                        for (int d = 0; d < extractedSpectrum.Count; d++)
                        {
                            mz[d] = extractedSpectrum[d].Mass / massInt64Shift;
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
                Ywriter.CloseYafms();
                Console.WriteLine("...Yafms Closed");
            }

            for (int i = 0; i < ChromatogramParameters.Chromatogram.Count; i++)
            {
                if (textWriteData)
                {
                    if(i==0)
                    {
                    Console.WriteLine("Print text files");
                    }
                    StringBuilder sb = new StringBuilder();
                    extractedSpectrum = ChromatogramParameters.Chromatogram[i];

                    string outputFileDestination2 = @"d:\Csharp\Syn Output\LCSynthetic";
                    StringBuilder sb2 = new StringBuilder();
                    string name;
                    if (i < 10)
                    {
                        name = outputFileDestination2 + "0" + i.ToString() + ".txt";
                    }
                    else
                    {
                        name = outputFileDestination2 + i.ToString() + ".txt";
                    }
                    using (StreamWriter writer = new StreamWriter(name))
                    {
                        for (int d = 0; d < extractedSpectrum.Count; d++)
                        {
                            sb2 = new StringBuilder();
                            sb2.Append((decimal)((decimal)extractedSpectrum[d].Mass/massInt64Shift));
                            sb2.Append("\t");
                            sb2.Append(extractedSpectrum[d].Intensity);

                            writer.WriteLine(sb2.ToString());
                        }
                    }
                }
            }

            Assert.AreEqual(211061, SpectraFiles.NoiseSpectra.Count);
            Assert.AreEqual(25000, SpectraFiles.MZParameters.Resolution);
            Assert.AreEqual(81, SpectraFiles.MonoIsotopicPeakList.Count);
            //Assert.AreEqual(1, SpectraFiles.MonoIsotopicPeakList.Count);
        }

        [Test]
        public void YAFMS()
        {
            //YafmsLibrary.YafmsReader reader = new YafmsLibrary.YafmsReader();
            //YafmsLibrary.YafmsWriter writer = new YafmsLibrary.YafmsWriter();

            //string FileLocation = @"D:\Csharp\Syn Output\ParameterFiles\Start.yafms";

            //writer.OpenYafms(FileLocation);
            //double[] mz = new double[] { 100.3, 212.5, 356.5 };
            //float[] intensities = new float[] { 100.0f, 260.3f, 175.5f };
            //writer.InsertData(1, 1, 10.2f, mz, intensities, 136.3f, 189.1f, 145.3f, "+", 0, 125.5);
            //writer.CloseYafms();
            Assert.AreEqual(1, 1);
        }


        [Test]
        public void test2()//test single spectrum
        {
            int massInt64Shift = 100000;//  1000.12345 will be 100012345
            double intensityInt64Shift = 1;
            string fileListLocation;
            string outputFileDestination;

            if(1==1)
            {
                fileListLocation = @"D:\Csharp\0_FileLoadIndex5Syn.txt";
                outputFileDestination = @"d:\Csharp\Syn Output\LCSynthetic.txt";
            }
            else
            {
                //fileListLocation = @"g:\PNNL Files\Csharp\0_HomeFileLoadIndex5Syn.txt"
            }
            //create object to hold the data
            SyntheticSpectraRun<Int64, double> SpectraFiles = new SyntheticSpectraRun<Int64, double>(fileListLocation);

            string dataToUse = @"D:\Csharp\CSV Glycan Test3 10K.csv";
            SpectraFiles.TheoryFileBox.FileName = dataToUse;

            //loads the data from the files listed in fileList and appends them to the DataProject as XYDataLists
            FileLoadFactory<Int64, double> iterateList = new FileLoadFactory<Int64, double>();
            iterateList.LoadSyntheticFiles(SpectraFiles, massInt64Shift, intensityInt64Shift);

            SpectraFiles.MZParameters.StartMass = 389;
            SpectraFiles.MZParameters.EndMass = 2100;

            #region deal with spectrum
            
            ISpectraGenerator syntheticSpectraController = new SyntheticSpectraController();
            syntheticSpectraController.SpectraParameters = SpectraFiles;

            List<PeakDecimal> existingGrid = new List<PeakDecimal>();
            List<PeakDecimal> outputSpectra = new List<PeakDecimal>();
            //outputSpectra=syntheticSpectraController.WithNoiseRun(massInt64Shift);
            outputSpectra = syntheticSpectraController.WithoutNoise(massInt64Shift,existingGrid);

            //shrink spectra
            List<PeakGeneric<Int64, float>> finalSpectraInt64 = new List<PeakGeneric<Int64, float>>();
            for (int i = 0; i < outputSpectra.Count; i++)
            {
                PeakGeneric<Int64, float> newPointInt64 = new PeakGeneric<Int64, float>();
                newPointInt64.Mass = (Int64)(outputSpectra[i].Mass * (decimal)massInt64Shift);
                newPointInt64.Intensity = (float)outputSpectra[i].Intensity;
                finalSpectraInt64.Add(newPointInt64);
            }
            #endregion

            //output to real world

            StringBuilder sb = new StringBuilder();
            using (StreamWriter writer = new StreamWriter(outputFileDestination))
            {
                for (int d = 0; d < finalSpectraInt64.Count; d++)
                {
                    sb = new StringBuilder();
                    sb.Append((decimal)((decimal)finalSpectraInt64[d].Mass / massInt64Shift));
                    sb.Append("\t");
                    sb.Append(finalSpectraInt64[d].Intensity);

                    writer.WriteLine(sb.ToString());
                }

            }

            Assert.AreEqual(211061, SpectraFiles.NoiseSpectra.Count);
            Assert.AreEqual(25000, SpectraFiles.MZParameters.Resolution);
            Assert.AreEqual(81, SpectraFiles.MonoIsotopicPeakList.Count);

        }

        [Test]
        public void compare()
        {
            List<double> libraryMasses = new List<double>();
            List<double> dataMasses = new List<double>();
            CompareResults Results = new CompareResults();
            double Tolerance = 0.5;

            libraryMasses.Add(0);
            libraryMasses.Add(1);
            libraryMasses.Add(2);
            libraryMasses.Add(3);
            libraryMasses.Add(4);
            libraryMasses.Add(103.1);
            libraryMasses.Add(103.2);
            libraryMasses.Add(103.3);
            libraryMasses.Add(110);
            libraryMasses.Add(111);
            libraryMasses.Add(112);

            dataMasses.Add(100);
            dataMasses.Add(101);
            dataMasses.Add(102);
            dataMasses.Add(103);
            dataMasses.Add(103.12);
            dataMasses.Add(103.21);
            dataMasses.Add(103.31);
            dataMasses.Add(150);
            dataMasses.Add(151);
            dataMasses.Add(152);


            
            CompareController letsCompare = new CompareController();
            letsCompare.CompareWithContrast(libraryMasses, dataMasses, Results, Tolerance);// abstracted call:  accepts a library mass and data mass and returns indexes

            //Aonly 0,1,2,7,8,9
            //AMatch 3,4,5,6,3,4,5,6,3,4,5,6
            //Bmatch 4,4,4,4,5,5,5,5,6,6,6,6
            //Bonly ,1,2,3,7,8,9
        }
    }
}
