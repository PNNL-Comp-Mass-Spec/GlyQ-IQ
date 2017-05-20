using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using GetPeaks_DLL;
using DeconTools.Backend.Core;
using GetPeaks_DLL.DataFIFO.DeconFIFO;
using PNNLOmics.Data.Features;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL.AnalysisSupport;
using GetPeaks_DLL.Go_Decon_Modules;
using GetPeaks_DLL.Objects.TandemMSObjects;
using GetPeaks_DLL.Functions;
using System.Text.RegularExpressions;
using MemoryOverloadProfilierX86;
using GetPeaks_DLL.DataFIFO;
using PNNLOmics.Data;
using PNNLOmics.Data.Constants;
using System.IO;
using GetPeaks_DLL.Glycolyzer;
using GlycolyzerGUImvvm.Models;
using GlycolyzerGUImvvm.ViewModels;
using GetPeaks_DLL.Objects.ParameterObjects;
using GetPeaks_DLL.SQLiteEngine;
using GetPeaks_DLL.Parallel;

namespace GetPeaks.UnitTests
{
    
    class FIFOTests
    {
        /// <summary>
        /// load an isos file to a IsosResultsObjectList
        /// </summary>
        [Test]
        public void LoadISOSTest()
        {
            

            //string fileName = @"V:\caoli_B_2_CID_09022011_isos.csv";
            string fileName = @"D:\PNNL\Projects\Glycopeptide\IsosDMS\caoli_B_2_CID_09022011_isos.csv";
            int numberOfIsosLoaded = 0;

            ISOSImporterGetPeaks getData = new ISOSImporterGetPeaks();
            

            List<FeatureLight> importedFeatures;
            List<int> scanSet;
            getData.Import(fileName, out importedFeatures, out scanSet);

            numberOfIsosLoaded = importedFeatures.Count;
            //Assert.AreEqual(121219, numberOfIsosLoaded);//lorentzian
            Assert.AreEqual(212717, numberOfIsosLoaded);//lorentzian
        }

        [Test]
        public void LoadISOSAndWriteTest()
        {
            string fileName = @"D:\PNNL\Projects\Glycopeptide\IsosDMS\caoli_B_2_CID_09022011_isos.csv";
            
            List<IsosObject> isosList = new List<IsosObject>();
            GetPeaks_DLL.DataFIFO.FileIterator.deliminator deliminatorFiletype = FileIterator.deliminator.Comma;

            StringLoadTextFileLine loadSpectraL = null;
            loadSpectraL = new StringLoadTextFileLine();
            List<string> stringListFromFiles = new List<string>();

            //load strings
            stringListFromFiles = loadSpectraL.SingleFileByLine(fileName);//loads all isos

            Console.WriteLine("Load file: " + fileName + "\n");

            //grab column header
            

            #region load data and parse it
            ParseStringListToIsos newParser = new ParseStringListToIsos();

            List<IsosObject> loadedIsos;
            string isotopeColumnHeader;
            newParser.Parse(stringListFromFiles, deliminatorFiletype, out loadedIsos, out isotopeColumnHeader);

            #endregion

            int numberOfIsosLoaded = loadedIsos.Count;

            //create strings
            List<string> isosStringsForExport;
            ConvertIsosToString newConcatonation = new ConvertIsosToString();
            newConcatonation.Convert(loadedIsos, deliminatorFiletype, out isosStringsForExport);

            StringListToDisk newWriter = new StringListToDisk();
            string outputLocation = @"V:\newIsosFile";
            string isotopeFile = outputLocation + "_Isotopes.csv";
            //write features
            newWriter.toDiskStringList(isotopeFile, isosStringsForExport, isotopeColumnHeader);
            
            Assert.AreEqual(212717, numberOfIsosLoaded);

            Assert.AreEqual("7,1,7905,429.09423,0.0212,428.3487,428.08754,428.08754,0.0066,243.07,7905,1056,0,0.14869", isosStringsForExport[0]);
        }

        [Test]//this takes a long time
        public void LoadDeconPeaksFileAndCalibrateTest()
        {
            decimal calibrationCoeffSlope = 0;
            decimal calibrationCoeffIntercept = 0;

            string fileName = "";
            //string folderIn = @"D:\PNNL\Projects\DATA 2012_01_11 SPIN QTOF3\1_DeconTools Sum 5 scans non-calibrated\";
            string folderIn = @"D:\PNNL\Projects\DATA 2012_01_11 SPIN QTOF3\1_DeconTools Sum5 6br charge7 not Calibrated Test folder\";
            string folderOut = @"D:\PNNL\Projects\DATA 2012_01_11 SPIN QTOF3\1_DeconTools Sum5 6br charge7 not Calibrated Test folder\Results\";

            //int U = 0;//ESI2
            int U=1;//SPIN2
            switch (U)
            {
                case 0:
                    {
                        fileName = "GLY06_11JAN12_LYNX_SN7980_TOP4wList_75000_ESI_2_peaks.txt";
                        calibrationCoeffSlope = 0.999984656323467M;
                        calibrationCoeffIntercept = 0.00179408173084994M;
                    }
                    break;
                case 1:
                    {
                        fileName = "GLY06_11JAN12_LYNX_SN7980_TOP4wList_75000_SPIN_2_peaks.txt";
                        calibrationCoeffSlope = 0.99997924507673M;
                        calibrationCoeffIntercept = 0.00157911039514147M;
                    }
                    break;
            }


           
            string fileNameIN = folderIn + fileName;
            string fileNameOut = folderOut + fileName;

            

            #region loadingpeaks file runs out of memory so load one line at a time
            //PeaksImporter getData = new PeaksImporter();
            //List<PeakDecon> deconPeaksLoaded1 = getData.ImportPeak(fileName);
            //List<PeakDeconLite> deconPeaksLoaded2 = getData.ImportPeakLite(fileName);
            //List<long> deconPeaksLoaded3 = getData.ImportMass(fileName);
            #endregion

            // This text is added only once to the file.
            if (File.Exists(fileNameOut))
            {
                Console.WriteLine("File allready exists.  Delete file first");
            }
            else
            {
                if (!File.Exists(fileNameOut))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(fileNameOut))
                    {
                        //sw.WriteLine("Hello");
                        //sw.WriteLine("And");
                        //sw.WriteLine("Welcome");
                    }
                }

                //set up variables
                string lineIn;          //single line loaded

                //set up Lines
                string existingLine;
                string processedLine;
                //Create a streamReader one line at at time
                using (StreamReader streamReadLine = new StreamReader(fileNameIN))
                {
                    // load one line at a time till end
                    while ((lineIn = streamReadLine.ReadLine()) != null)
                    {
                        existingLine = lineIn.ToString();

                        processedLine = ProcessLine(existingLine, calibrationCoeffSlope, calibrationCoeffIntercept);

                        AppendToTextFile(fileNameOut, processedLine);

                    }

                    //close file
                    streamReadLine.Close();
                }
            }
        }

        private static string ProcessLine(string lineIn, decimal slope, decimal intercept)
        {
            ParsePeaksToPeakDecon newParser = new ParsePeaksToPeakDecon();
            List<string> lineList = new List<string>();
            lineList.Add(lineIn);

            string columnheaders;
            List<PeakDecon> newPeaks;
            GetPeaks_DLL.DataFIFO.FileIterator.deliminator textDeliminator = GetPeaks_DLL.DataFIFO.FileIterator.deliminator.Tab;

            newParser.Parse(lineList, textDeliminator, out newPeaks, out columnheaders);

            string lineOut = columnheaders;
            string splitter = "\t";
            
            if (newPeaks.Count>0)
            {
                PeakDecon peakD = newPeaks[0];
                decimal mzNonCal = Convert.ToDecimal(peakD.mz);
                decimal mzCal = mzNonCal * slope + intercept;
                mzCal = Math.Round(mzCal, 5);
                string mzCalString = Convert.ToString(mzCal); 

                lineOut = 
                    peakD.peak_index.ToString() + splitter +
                    peakD.scan_num.ToString() + splitter +
                    mzCalString + splitter +
                    peakD.intensity.ToString() + splitter + 
                    peakD.fwhm.ToString() + splitter + 
                    peakD.signal_noise.ToString() + splitter +
                    peakD.MSFeatureID;
                
            }
            
            return lineOut;
        }

        private static void AppendToTextFile(string OutputPath, string lineToWrite)
        {
            // This text is always added, making the file longer over time
            // if it is not deleted.
            using (StreamWriter sw = File.AppendText(OutputPath))
            {
                sw.WriteLine(lineToWrite);
            }
        }

        [Test]
        public void LoadISOSANDCalibrateAndWriteTest()
        {

            #region parameters

            string folderIn = @"D:\PNNL\Projects\DATA 2012_01_11 SPIN QTOF3\1_DeconTools Sum5 6br charge7 not Calibrated Test folder\";
            string folderOut = @"D:\PNNL\Projects\DATA 2012_01_11 SPIN QTOF3\1_DeconTools Sum5 6br charge7 not Calibrated Test folder\ResultsISOS";
            decimal calibrationCoeffSlope = 0;
            decimal calibrationCoeffIntercept = 0;
            string fileName = "";
            //int U=0;//ESI2
            int U=1;//SPIN2
            //int U = 2;//test

            switch (U)
            {
                case 0:
                    {
                        fileName = "GLY06_11JAN12_LYNX_SN7980_TOP4wList_75000_ESI_2";
                        calibrationCoeffSlope = 0.999984656323467M;
                        calibrationCoeffIntercept = 0.00179408173084994M;
                    }
                    break;
                case 1:
                    {
                        fileName = "GLY06_11JAN12_LYNX_SN7980_TOP4wList_75000_SPIN_2";
                        calibrationCoeffSlope = 0.99997924507673M;
                        calibrationCoeffIntercept = 0.00157911039514147M;
                    }
                    break;
                case 2:
                    {
                        folderIn = @"D:\Csharp\Peaks test file\";
                        folderOut = @"D:\Csharp\ConosleApps\LocalServer\ISOS File Processing\";
                        fileName = "shortGLY06_11JAN12_LYNX_SN7980_TOP4wList_75000_SPIN_2";
                        calibrationCoeffSlope = 0.99997924507673M;
                        calibrationCoeffIntercept = 0.00157911039514147M;
                    }
                    break;
                
            }



            string fileNameIsos = folderIn + fileName + "_isos.csv";
            string fileNamePeaks = folderIn + fileName + "_peaks.txt";
            string fileNameIsosOut = folderOut + fileName + "_isos.csv";
            string fileNamePeaksOut = folderOut + fileName + "_peaks.txt";
            //string fileNameIn = @"D:\PNNL\Projects\Glycopeptide\IsosDMS\caoli_B_2_CID_09022011_isos.csv";

            #endregion

            #region load data

            List<IsosObject> isosList = new List<IsosObject>();
            GetPeaks_DLL.DataFIFO.FileIterator.deliminator deliminatorFiletype = FileIterator.deliminator.Comma;

            StringLoadTextFileLine loadSpectraL = null;
            loadSpectraL = new StringLoadTextFileLine();
            List<string> stringListFromFiles = new List<string>();

            //load strings
            stringListFromFiles = loadSpectraL.SingleFileByLine(fileNameIsos);//loads all isos

            Console.WriteLine("Loading file: " + fileNameIsos + "\n");
            Console.WriteLine("...");

            //grab column header
            // load data and parse it
            ParseStringListToIsos newParser = new ParseStringListToIsos();

            List<IsosObject> loadedIsos;
            string isotopeColumnHeader;
            newParser.Parse(stringListFromFiles, deliminatorFiletype, out loadedIsos, out isotopeColumnHeader);

            int numberOfIsosLoaded = loadedIsos.Count;

            Console.WriteLine("FileLoaded: " + fileNameIsos.ToString() + "\n");
            //calibrate data

            #endregion

            #region calibrate data

            Console.WriteLine("Calibrating Data...");
            decimal mass;
            decimal newMass;
            foreach (IsosObject isos in loadedIsos)
            {
                mass = Convert.ToDecimal(isos.average_mw);
                newMass = mass * calibrationCoeffSlope + calibrationCoeffIntercept;
                isos.average_mw = Convert.ToDouble(newMass);

                mass = Convert.ToDecimal(isos.mostabundant_mw);
                newMass = mass * calibrationCoeffSlope + calibrationCoeffIntercept;
                isos.mostabundant_mw = Convert.ToDouble(newMass);

                mass = Convert.ToDecimal(isos.mz);
                newMass = mass * calibrationCoeffSlope + calibrationCoeffIntercept;
                isos.mz = Convert.ToDouble(newMass);

                int chargestate = isos.charge;
                mass = Convert.ToDecimal(isos.monoisotopic_mw);
                newMass = Convert.ToDecimal((isos.mz*chargestate - Constants.SubAtomicParticles[SubAtomicParticleName.Proton].MassMonoIsotopic * chargestate));
                isos.monoisotopic_mw = Convert.ToDouble(newMass);
            }

            #endregion

            #region write data

            //create strings
            List<string> isosStringsForExport;
            ConvertIsosToString newConcatonation = new ConvertIsosToString();
            newConcatonation.Convert(loadedIsos, deliminatorFiletype, out isosStringsForExport);

            StringListToDisk newWriter = new StringListToDisk();
            //string outputLocation = folderOut;
            string isotopeFile = fileNameIsosOut;
            Console.WriteLine("Writing : " + isotopeFile.ToString() + "\n");

            //write features
            newWriter.toDiskStringList(isotopeFile, isosStringsForExport, isotopeColumnHeader);

            #endregion
            
            //Assert.AreEqual(212717, numberOfIsosLoaded);

            //Assert.AreEqual("7,1,7905,429.09423,0.0212,428.3487,428.08754,428.08754,0.0066,243.07,7905,1056,0,0.14869", isosStringsForExport[0]);
        }

        /// <summary>
        /// load a tab deliminated XYData file to a XYDataList
        /// </summary>
        [Test]
        public void LoadXYDataTest()
        {
            //string fileName = @"L:\PNNL Files\CSharp\RAW\N046 10P_G.txt";
            string fileName = @"D:\Csharp\RAW\N046 10P_G.txt";
            int numberOfIsosLoaded = 0;

            LoadXYData getData = new LoadXYData();

            List<XYData> importedData;
            string fileHeader;
            importedData = getData.Import(fileName, out fileHeader);

            numberOfIsosLoaded = importedData.Count;
            //Assert.AreEqual(121219, numberOfIsosLoaded);//lorentzian
            Assert.AreEqual(974137, numberOfIsosLoaded);//lorentzian
        }

        public void LoadDYDataAndWriteTestSyntheticDataGenerator()
        {
            //string fileName = @"L:\PNNL Files\CSharp\RAW\N046 10P_G.txt";
            string fileName = @"D:\Csharp\RAW\N046 10P_G.txt";
            Console.WriteLine("Load file: " + fileName + "\n");

            int numberOfXYDataLoaded = 0;

            LoadXYData getData = new LoadXYData();

            List<XYData> importedData;
            string fileHeader;
            importedData = getData.Import(fileName, out fileHeader);

            numberOfXYDataLoaded = importedData.Count;

            //0 start code__________________________________________________________________________________
            //string outputLocation = @"L:\SpotToWrite\";
            string outputLocation = @"D:\Csharp\RAW\SpotToWrite\";
            Random newRandomGenerator = new Random();
            double randomNumber;
            double randWithinNormalDistribution;

            double rangePercent = 40;//this is for bounding the values to +-10% //default 10 for 10%CV  (10,20,40,60)
            int start = 0; int stop = 48;

            bool writeDataControl = false;
            bool writeDataCase = true;
            double biofactorControl = 1.00; string idnameControl = "BF100";
            //double biofactorCase = 1.05; string idname = "BF105";
            //double biofactorCase = 1.10;string idname = "BF110";
            //double biofactorCase = 1.25;string idname = "BF125";
            //double biofactorCase = 1.50;string idname = "BF150";
            //double biofactorCase = 2.00;string idname = "BF200";
            double biofactorCase = 2.50;string idname = "BF250";

            double massToModify = 1809.63930;


            double multiplyFactor = 0; 
            List<double> randomNumberList = new List<double>();

            for (int i = start; i < stop; i++)
            {
                //1.  create random number
                randomNumber = newRandomGenerator.NextDouble();
                randomNumberList.Add(randomNumber);

                //2.  make sure the number comes from a normal distribution
                //SQRT(-2*LN(RAND()))*SIN(2*PI()*RAND())

                randWithinNormalDistribution = Math.Sqrt(-2 * Math.Log(randomNumber)) * Math.Sin(2 * Math.PI * randomNumber);

                //3.  
                multiplyFactor = rangePercent / 100 * randWithinNormalDistribution + 1;

                //4.  fixed window around mass
                double window = 0.2;

                if (writeDataControl)
                { 
                    BioFactor newMarkerControl = new BioFactor(biofactorControl, massToModify, window, multiplyFactor);
                    List<XYData> newSpecrtraControl = ModifyIon(importedData, newMarkerControl);

                    //create strings
                    Console.WriteLine("Writing Control Spectra " + i.ToString());
                    WriteDataSet(outputLocation, fileHeader, i, newSpecrtraControl, BioFactor.SetType.Control, idnameControl);
                }

                if (writeDataCase)
                { 
                    BioFactor newMarkerCase = new BioFactor(biofactorCase, massToModify, window, multiplyFactor);
                    List<XYData> newSpecrtraCase = ModifyIon(importedData, newMarkerCase);

                    //create strings
                    Console.WriteLine("Writing Spectra " + i.ToString());
                    WriteDataSet(outputLocation, fileHeader, i, newSpecrtraCase, BioFactor.SetType.Case, idname);
                }
            }

            Assert.AreEqual("Finished", "Finished");
        }

        #region private functions

        private static void WriteDataSet(string outputLocation, string fileHeader, int i, List<XYData> newSpecrtra, GetPeaks_DLL.Objects.BioFactor.SetType setType, string idName)
        {
            List<string> XYDataStringsForExport;
            ConvertXyDataToString newConverter = new ConvertXyDataToString();
            newConverter.Convert(newSpecrtra, FileIterator.deliminator.Tab, out XYDataStringsForExport);

            StringListToDisk newWriter = new StringListToDisk();
            
            
            string name = "";
            switch (setType)
            {
                case BioFactor.SetType.Case:
                    {
                        name = "Case";
                    }
                    break;
                case BioFactor.SetType.Control:
                    {
                        name = "Control";
                    }
                    break;
            }
            

            string isotopeFile;
            if (i < 10)
            {
                isotopeFile = outputLocation + name + "_" + idName +"_0" + i + ".txt";
            }
            else
            {
                isotopeFile = outputLocation + name + "_" + idName + "_" + i + ".txt";
            }


            //write features
            newWriter.toDiskStringList(isotopeFile, XYDataStringsForExport, fileHeader);
        }

        private static List<XYData> ModifyIon(List<XYData> importedData, BioFactor newMarker)
        {
            List<XYData> newSpecrtra = new List<XYData>();
            foreach (XYData data in importedData)
            {
                //modify multiply factor for biomarkers
                if (data.X > newMarker.Mass + 1.00253 * 0 - newMarker.WindowAroundMass && data.X < newMarker.Mass + 1.00253 * 0 + newMarker.WindowAroundMass ||
                    data.X > newMarker.Mass + 1.00253 * 1 - newMarker.WindowAroundMass && data.X < newMarker.Mass + 1.00253 * 1 + newMarker.WindowAroundMass ||
                    data.X > newMarker.Mass + 1.00253 * 2 - newMarker.WindowAroundMass && data.X < newMarker.Mass + 1.00253 * 2 + newMarker.WindowAroundMass ||
                    data.X > newMarker.Mass + 1.00253 * 3 - newMarker.WindowAroundMass && data.X < newMarker.Mass + 1.00253 * 3 + newMarker.WindowAroundMass ||
                    data.X > newMarker.Mass + 1.00253 * 2 - newMarker.WindowAroundMass && data.X < newMarker.Mass + 1.00253 * 2 + newMarker.WindowAroundMass)
                {
                    newSpecrtra.Add(new XYData(data.X, data.Y * newMarker.IntensityMultiplyFactor * newMarker.BioFactorChange));
                }
                else
                {
                    newSpecrtra.Add(new XYData(data.X, data.Y * newMarker.IntensityMultiplyFactor));
                }
            }
            return newSpecrtra;
        }

        #endregion

        [Test]
        public void TestScanInfoParcing()
        {
            string scanInfo = @"FTMS + p NSI d Full ms2 572.77@cid35.00 [145.00-1160.00]";

            double mz = getMZFromScanInfo(scanInfo);

            Assert.AreEqual(572.77, mz);
        }


        private double getMZFromScanInfo(string scanInfo)
        {
            string pattern = @"(?<mz>[0-9.]+)@cid";
            var match = Regex.Match(scanInfo, pattern);

            double mzScanInfo = 0;
            if (match.Success)
            {
                mzScanInfo = Convert.ToDouble(match.Groups["mz"].Value);
            }

            return mzScanInfo;

        }


        [Test]
        public void LoadXYDataFromTandem()
        {

            Profiler newMemoryProfilier = new Profiler();
            newMemoryProfilier.printMemory("Start");

            GetDataController dataLoad = new GetDataController();

            SimpleWorkflowParameters parameters = new SimpleWorkflowParameters();
            //parameters.Part1Parameters.StartScan = 349;
            parameters.Part1Parameters.StartScan = 370;//the first full scan is 370 and the next full scan is 381
            parameters.Part1Parameters.StopScan = 386;

            InputOutputFileName newFile = new InputOutputFileName();
            
            //the raw is working and the YAFMS is not, partially because of a failed conversion
            //current home
            //newFile.InputFileName = @"L:\PNNL Files\PNNL\Projects\Glycopeptide\RAW Data\caoli_B_2_CID_09022011.raw";
            //newFile.InputFileName = @"L:\PNNL Files\Csharp\YAFMS\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.yafms";

            //current work
            newFile.InputFileName = @"D:\PNNL\Projects\Glycopeptide\RAW Data\caoli_B_2_CID_09022011.raw";

            //current yafms that does not pass the tests
            //newFile.InputFileName = @"D:\PNNL\Projects\Glycopeptide\RAW Data\caoli_B_2_CID_09022011.yafms";
            //newFile.InputFileName = @"D:\Csharp\YAFMS\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.yafms";

            string[] argsstd = new string[0];
            PopulatorArgs args = new PopulatorArgs(argsstd);
            //char letter = 'K';
            //string sqLiteFile;
            //string sqLiteFolder;
            //int computersToDivideOver;
            //int coresPerComputer;
            //string logFile;
            //ParametersTHRASH thrashParameters = ParametersForTesting.Load(letter, out sqLiteFile, out sqLiteFolder, out computersToDivideOver, out coresPerComputer, out logFile);


            char letter = 'K';
            const bool overrideParameterFile = true; //fit and ThrashPBR//false will use the parameter file
            ParalellController engineController;
            string sqLiteFile;
            string sqLiteFolder;
            int computersToDivideOver;
            int coresPerComputer;
            string logFile;
            //InputOutputFileName newFile;
            ParametersSQLite sqliteDetails;

            ParametersForTesting.Load(letter, overrideParameterFile, out engineController, out sqLiteFile, out sqLiteFolder, out computersToDivideOver, out coresPerComputer, out logFile, out newFile, out sqliteDetails, args);

            ParametersTHRASH thrashParameters = (ParametersTHRASH)engineController.ParameterStorage1;

            thrashParameters.FileInforamation = newFile;
           

            newMemoryProfilier.printMemory("1");
            newMemoryProfilier.printMemory("1");

            #region units

            for (int j = 0; j < 1; j++)
            {
                List<TandemObject> precursorList = new List<TandemObject>();
                precursorList = dataLoad.GetTandemData(newFile, parameters.Part1Parameters.StartScan, thrashParameters);

                //foreach (TandemObject tandemObject in precursorList)
                //{
                //    tandemObject.LoadPrecursorData();

                //    tandemObject.PeakPickPrecursorData();
                //    tandemObject.LoadPrecursorMass();//must follow PeakPickPrecursorData

                //    tandemObject.LoadFragmentationData();
                //}

                //List<TandemObject> precursorList2 = new List<TandemObject>();
                //parameters.Part1Parameters.StartScan = 381;
                //precursorList2 = dataLoad.GetTandemData(newFile, parameters.Part1Parameters.StartScan);

                //foreach (TandemObject tandemObject in precursorList)
                //{
                //    tandemObject.LoadPrecursorData();

                //    tandemObject.PeakPickPrecursorData();
                //    tandemObject.LoadPrecursorMass();//must follow PeakPickPrecursorData

                //    tandemObject.LoadFragmentationData();
                //}

                //List<TandemObject> precursorList3 = new List<TandemObject>();
                //parameters.Part1Parameters.StartScan = 392;
                //precursorList3 = dataLoad.GetTandemData(newFile, parameters.Part1Parameters.StartScan);

                //foreach (TandemObject tandemObject in precursorList)
                //{
                //    tandemObject.LoadPrecursorData();

                //    tandemObject.PeakPickPrecursorData();
                //    tandemObject.LoadPrecursorMass();//must follow PeakPickPrecursorData

                //    tandemObject.LoadFragmentationData();
                //}

                //List<TandemObject> precursorList4 = new List<TandemObject>();
                //parameters.Part1Parameters.StartScan = 403;
                //precursorList4 = dataLoad.GetTandemData(newFile, parameters.Part1Parameters.StartScan);

                //foreach (TandemObject tandemObject in precursorList)
                //{
                //    tandemObject.LoadPrecursorData();

                //    tandemObject.PeakPickPrecursorData();
                //    tandemObject.LoadPrecursorMass();//must follow PeakPickPrecursorData

                //    tandemObject.LoadFragmentationData();
                //}
            #endregion

                //newMemoryProfilier.printMemory("End");



                Assert.AreEqual(10, precursorList.Count);

                //YAFMS is supposed to have differnet numbers here

                Assert.AreEqual(572.77, precursorList[0].PrecursorMZ);//371
                Assert.AreEqual(572.76634853819996d, precursorList[0].PrecursorMZCorrected);//371

                //YAFMS will have a different number of points for some reason

                Assert.AreEqual(3456, precursorList[0].FragmentationData.Count);//371
                Assert.AreEqual(3937, precursorList[1].FragmentationData.Count);//372
                Assert.AreEqual(9412, precursorList[2].FragmentationData.Count);
                Assert.AreEqual(3840, precursorList[3].FragmentationData.Count);
                Assert.AreEqual(8783, precursorList[4].FragmentationData.Count);
                Assert.AreEqual(4693, precursorList[5].FragmentationData.Count);
                Assert.AreEqual(3903, precursorList[6].FragmentationData.Count);
                Assert.AreEqual(5428, precursorList[7].FragmentationData.Count);
                Assert.AreEqual(8521, precursorList[8].FragmentationData.Count);
                Assert.AreEqual(10672, precursorList[9].FragmentationData.Count);


                //for (int i = 0; i < precursorList.Count; i++)
                //{
                //    TandemObject tandemHold = precursorList[i];
                //    tandemHold.Dispose();
                //    tandemHold = null;
                //    //newMemoryProfilier.printMemory("Delete1");
                //}

                //for (int i = 0; i < precursorList2.Count; i++)
                //{
                //    TandemObject tandemHold = precursorList2[i];
                //    tandemHold.Dispose();
                //    tandemHold = null;
                //    //newMemoryProfilier.printMemory("Delete2");
                //}

                //for (int i = 0; i < precursorList3.Count; i++)
                //{
                //    TandemObject tandemHold = precursorList3[i];
                //    tandemHold.Dispose();
                //    tandemHold = null;
                //    //newMemoryProfilier.printMemory("Delete3");
                //}

                //for (int i = 0; i < precursorList4.Count; i++)
                //{
                //    TandemObject tandemHold = precursorList4[i];
                //    tandemHold.Dispose();
                //    tandemHold = null;
                //    //newMemoryProfilier.printMemory("Delete4");
                //}
            }
            parameters.Dispose();
            //precursorList = null;
            newMemoryProfilier.printMemory("Clear");
            newMemoryProfilier.printMemory("Clear");
            newMemoryProfilier.printMemory("Clear");
            newMemoryProfilier.printMemory("Clear");
            newMemoryProfilier.printMemory("Clear");
            newMemoryProfilier.printMemory("Clear");
            newMemoryProfilier.printMemory("Clear");
        }       

        /// <summary>
        /// this edits the config file on pubs
        /// </summary>
        [Test]
        public void LoadConfigFile()
        {
            StringLoadTextFileLine loadData = new StringLoadTextFileLine();
            List<string> lines = new List<string>();
            string parametersFilePath = @"E:\0_PUBSetup\27_DMS_Programs_PubNames\PubNamesAndLocations.txt";

            lines = loadData.SingleFileByLine(parametersFilePath);

            FileIterator.deliminator textDeliminator;
            textDeliminator = FileIterator.deliminator.Comma;

            ModifyXML loader = new ModifyXML();

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

            string[] wordArray;

            int startline = 1;//0 is the headder

            string folder = lines[0];

            int length = lines.Count;
            for (int i = startline; i < length; i++)//i=0 is the headder
            {
                string line = lines[i];
                wordArray = line.Split(spliter);

                //string configFilePath = @"D:\PNNL\Lab Infrastructure\PNNL Institutational Computerin (PIC)\Batch File Technology\AnalysisManagerProg.exe.config";
                //string pubName = "Pub-11";
                //int pubNumber = 1;
                //int lineNumberToChange = 36;
                string configFilePath = folder + wordArray[0];
                string pubName = wordArray[1];
                int pubNumber = Convert.ToInt32(wordArray[2]);
                int lineNumberToChange = Convert.ToInt32(wordArray[3]);

                List<string> analysisManagerLines = loader.Load(pubName, pubNumber, configFilePath, lineNumberToChange);

                StringListToDisk writer = new StringListToDisk();
                writer.toDiskStringList(configFilePath, analysisManagerLines);             
            }


 
        }

        [Test]
        public void LoadGlycolyzerParameterFileGUI()
        {

            //ImportWrapper.ImportParameters(@"D:\SaveFolder\New Text Document.xml_NLinked_Alditol_PolySA_3.2_Fragment_Neutral.xml");
            //ImportWrapper.ImportParameters(@"D:\PNNL\Projects\Myanna Harris\New Text Document.xml_NLinked_Alditol_PolySA_3.2_Fragment_Neutral.xml");
            //ParameterModelWrapper parameterModel_Input = ImportWrapper.parameterModel_InputWrapper;

            //Import.ImportParameters(@"D:\PNNL\Projects\Myanna Harris\New Text Document.xml_NLinked_Alditol_PolySA_3.2_Fragment_Neutral.xml");
            //GUIImport.ImportParameters(@"D:\Backup not sync\V Drive Copy\GlycolyzerGUITestFolders\SaveLocation\NLinked_Alditol_10_Alditol_Neutral.xml");
            //Import.ImportParameters(@"V:\GlycolyzerGUITestFolders\SaveLocation\NLinked_Alditol_10_Alditol_Neutral.xml");
            //Import.ImportParameters(@"V:\GlycolyzerGUITestFolders\SaveLocation\ParameterFile.xml");
            //GUIImport.ImportParameters(@"C:\GlycolyzerData\SaveLocation\NLinked_Alditol_15_Alditol_Neutral.xml");
            GUIImport.ImportParameters(@"C:\GlycolyzerData\SaveLocation\OmniFinder_27_Alditol_Neutral.xml");
            //GUIImport.ImportParameters(@"C:\GlycolyzerData\SaveLocation\NLinked_Alditol_50_Alditol_Neutral.xml");
            
            ParameterModel parameterModel_Input = GUIImport.parameterModel_Input;

            GlycolyzerParametersGUI parameters = new GlycolyzerParametersGUI();
            parameters.ConvertFromGUI(parameterModel_Input);

            string libraryLocation = @"C:\GlycolyzerData\GlycolyzerLibraryDirectorCDrive.txt";
            //string libraryLocation = @"R:\GlycolyzerData\GlycolyzerLibraryDirectorRDrive.txt";


            GlycolyzerController controller = new GlycolyzerController();
            controller.Glycolyze(parameters, libraryLocation);

        }
    }
}
