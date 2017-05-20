//Scott Kronewitter, 3-17-2010
//Loads a fixed index file containing several paths to spectra text files. 
//Each spectra is then loaded to a list array either be line or entire file.

using System;
using System.Text;
using System.Diagnostics;

using System.Collections.Generic;
using System.IO;
using PNNLOmics.Data;
using ProteinFileReader;

using YafmsLibrary;

namespace ConsoleApplication1
{
    class Program
    {
        #region 0. Hardcoded Constants.  Will come from GUI in future.

        
            //Hardcode location of file table of contents
        const string indexFileSource1 = @"d:\Csharp\0_FileLoadIndex1.txt";//program 1
        const string indexFileSource2 = @"d:\Csharp\0_FileLoadIndex2.txt";//program 2
        const string indexFileSource3 = @"d:\Csharp\0_FileLoadIndex3.txt";//program 3
        const string indexFileSource4 = @"d:\Csharp\0_FileLoadIndex4.txt";//program 4

        const string parameterFileSource = @"d:\Csharp\1_SS_LoadFile.txt";
        const string outputFileDestination = @"d:\Csharp\Synthetic.txt";
        const string fastaFileSource = @"d:\Csharp\F_FASTA_RNB.txt";
        const string glycoPeptideTable = @"d:\Csharp\GP_Table.xml";
        const string YAFMSFileSource = @"d:\Csharp\YAFMS\FirstYafy.yafms";

        //const string indexFileSource1 = @"g:\PNNL Files\Csharp\0_HomeFileLoadIndex1.txt";//program 1
        //const string indexFileSource2 = @"g:\PNNL Files\Csharp\0_HomeFileLoadIndex2.txt";//program 2
        //const string indexFileSource3 = @"g:\PNNL Files\Csharp\0_HomeFileLoadIndex3.txt";//program 3
        //const string indexFileSource4 = @"g:\PNNL Files\Csharp\0_HomeFileLoadIndex4.txt";//program 4

        //const string parameterFileSource = @"g:\PNNL Files\Csharp\1_SS_LoadFile.txt";
        //const string outputFileDestination = @"g:\PNNL Files\Csharp\Synthetic.txt";
        //const string fastaFileSource = @"g:\PNNL Files\Csharp\F_FASTA_RNB.txt";
        //const string glycoPeptideTable = @"g:\PNNL Files\Csharp\GP_Table.xml";
        //const string YAFMSFileSource = @"g:\PNNL Files\Csharp\YAFMS\FirstYafy.yafms";

        const string loadMethod = "line";//load the file at once or line by line
        //const string loadMethod = "full";//load the file at once or line by line

        //const string isotopeType = "peptide";
        const string isotopeType = "glycan";
        #endregion

        static void Main(string[] args)
        {
            //set up print buffer
            PrintBuffer printer = new PrintBuffer();

			//switch to change what program does
            //1=synthetic spectra
                //d:\Csharp\CSV Glycan Test3 10K.csv
                //d:\Csharp\CSV FTICR 9T.csv
            //2=comparecontrast
                //d:\Csharp\CSV FileA2.csv
                //d:\Csharp\CSV FTICR 9T.csv
                //d:\Csharp\CSV FileA2 Library.csv
            //3=peptide
				//d:\Csharp\CSV FileA2.csv
				//d:\Csharp\CSV FileA2 Library.csv
            //4=YAFMS
                //d:\Csharp\CSV FileA2.csv
                //d:\Csharp\CSV FileA2 Library.csv

            int programNumber = 4;

            #region start timer
                System.Timers.Timer aTimer = new System.Timers.Timer();
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
            #endregion

            #region 1. Load in mass list and noise data
				string indexFileSource;	
				switch(programNumber)
				{
					case 1:
					{
						indexFileSource=indexFileSource1;
					}
					break;
					case 2:
					{
						indexFileSource=indexFileSource2;
					}
					break;
					case 3:
					{
						indexFileSource=indexFileSource3;
					}
                    break;
                    case 4:
                    {
                        indexFileSource = indexFileSource4;
                    }
					break;
					default:
						indexFileSource = indexFileSource1;
					break;
				}
				 
                //Set up List array to hold files to be inported
                List<String> fileslist = new List<String>();

                //New Listimporter  //creates an instance of Importer to work with
                FileListImporter loadList = new FileListImporter(indexFileSource, printer);

                //Load table of contents file containing other files to load
                loadList.ImportFileList(fileslist, printer);
                //loadList.ImporteFileList(fileslist, sb);

                //New File iterator to load several mass spectra.  the first one is the data, the second one is the noise
                FileIterator iterateList = new FileIterator();
 
                List<DataSetSK> DataProject = new List<DataSetSK>();  //a list of MZ data files
                
                //loads the data from the files listed in fileList and appends them to the DataProject as XYDataLists
                iterateList.IterateFiles(fileslist, loadMethod, DataProject, printer);

                //apply features to project  //adds the mass list as dataProject[0] and noise data as dataProject[1] 
                FileLoadParameters AppendToProject = new FileLoadParameters();
                AppendToProject.registerFeatures(DataProject);
            #endregion

            switch (programNumber)
            {
                case 1:
                    #region A. Load in parameter list
                    //load parameter file
                    List<string> parametersList = new List<string>();
                    FileListImporter loadParameterList = new FileListImporter(parameterFileSource, printer);

                    loadParameterList.ImportFileList(parametersList, printer);

                    //convert loaded text into numbers
                    List<decimal> parameterValueList = new List<decimal>();
                    IConvert convert = new Converter();
                    convert.TextToDecimal(parametersList, parameterValueList);

                    //apply parameters to the dataset object in DataProject
                    int index = 0;//adds the parameter list to dataProject[0]
                    AppendToProject.registerParameters(parameterValueList, DataProject, index);
                    #endregion

                    #region B. generate synthetic spectra
                    
					//create synthetic spectra which includes a pure spectra and noise interpolation
					bool withNoise;
					withNoise = true;
					//withNoise = false;
					
					List<PeakDecimal> SyntheticSpectra = new List<PeakDecimal>();

                    ISpectraGenerator CreateSpectra = new SyntheticSpectra_Controller();
					if (withNoise)
					{
						SyntheticSpectra = CreateSpectra.WithNoise(DataProject, isotopeType, printer);
					}
					else
					{
						SyntheticSpectra = CreateSpectra.WithoutNoise(DataProject, isotopeType, printer);
					}
					#endregion

                    #region C. printing to Console
                    //more printing.  the rest of this is just cosmetics for printing to the console

                    StringBuilder sb = new StringBuilder();

                    for (int d = 0; d < DataProject.Count; d++)
                    {
                        List<XYData> XYList = DataProject[d].XYList;

                        printer.AddLine("First Mass is: " + XYList[0].X + "\n");
                    }

                    using (StreamWriter writer = new StreamWriter(outputFileDestination))
                    {
                        for (int d = 0; d < SyntheticSpectra.Count; d++)
                        {
                            sb = new StringBuilder();
                            sb.Append(SyntheticSpectra[d].Mass);
                            sb.Append("\t");
                            sb.Append(SyntheticSpectra[d].Intensity);

                            writer.WriteLine(sb.ToString());
                        }

                    }

                    //print print buffer
                    printer.ToScreen();
                    #endregion
                    break;
                
                case 2:
                    #region A. run compare-contrast

                        Accelerate Tester = new Accelerate();
                        Tester.RunGO(DataProject);
                        
                    #endregion
                    break;
                case 3:
                    #region A. load FASTA
                        //FASTA

                        Protein2 theoreticalProtein = new Protein2();
                        double peptideMass;

                        FastaFileReader readFASTA = new FastaFileReader();
                        readFASTA.OpenFile(fastaFileSource);
                        readFASTA.ReadNextProteinEntry();
                        readFASTA.CloseFile();

                        peptideMass = readFASTA.PeptideMass;
                        theoreticalProtein.Name = readFASTA.HeaderLine;
                        theoreticalProtein.Sequence = readFASTA.ProteinSequence;

                        ModifyFASTA modiyFASTAfile = new ModifyFASTA();
                        theoreticalProtein.Name = modiyFASTAfile.HeaderToName(theoreticalProtein.Name);
                        
                        int z = 1;
                        z = z * 1;
                    #endregion

                    #region B. find glycosylation sites

                        GlycoPeptideResults glycoPeptideSitesResults = new GlycoPeptideResults();
                        GlycoPeptideParameters glycoPeptideParameter = new GlycoPeptideParameters();

                        glycoPeptideParameter.AminoAcidsToLeft = 1;
                        glycoPeptideParameter.AminoAcidsToRight = 2;

                        glycoPeptideParameter.glycoPeptideMasses = DataProject[0].XYList;
                        glycoPeptideParameter.glycanLibraryMasses = DataProject[1].XYList;

                        glycoPeptideParameter.SiteType = "N";//N, S, T, ST
                        glycoPeptideParameter.BoundingMethod = "User";
                        //glycoPeptideParameter.BoundingMethod = "MaxMass";
                        glycoPeptideParameter.DivisorValue = 1;
                        glycoPeptideParameter.FASTA = theoreticalProtein.Sequence;
                        //glycoPeptideParameter.FASTA = "NYSNYTTSTSSSNY";

                        glycoPeptideParameter.CoreMass = 910.32778;
                        glycoPeptideParameter.CoreToggle = 1;//0=off, 1=on
                        glycoPeptideParameter.Tollerance = 0.05;
                        //List<XYData> glycoPeptideMasses = DataProject[0].XYList;
						//List<XYData> glycanLibrary = DataProject[1].XYList;

                        GlycoPeptideController GPcontroller = new GlycoPeptideController();
						//GPcontroller.runGlycoPeptideFinder(siteType, BoundingMethod, theoreticalProtein.Sequence, glycoPeptideMasses, glycanLibrary);
                        GPcontroller.runGlycoPeptideFinder(glycoPeptideParameter, glycoPeptideSitesResults, glycoPeptideTable);
                        int i = 1;
                        i = i * 1;
                        
                    #endregion
                    break;
                case 4:
                #region YAFMS
                    
                    List<XYData> sampleList = new List<XYData>();
                    sampleList = DataProject[0].XYList;
                    
                    double[] MZarray = new double[sampleList.Count];
                    float[] IntArray = new float[sampleList.Count];
                    for (int j = 0; j < sampleList.Count;j++ )
                    {
                        MZarray[j] = sampleList[j].X;
                        IntArray[j] = (float)sampleList[j].Y;
                    }
                    
                    YafmsWriter Ywriter = new YafmsWriter();

                    string inputFile = YAFMSFileSource;
                    string outputFile = YAFMSFileSource;  //@"d:\Csharp\YAFMS\FirstYafy.yafms";

               
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
                

                    try
                    {
                        Ywriter.OpenYafms(inputFile);
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine (ex.Message + "\n" + @"No file was found with this reference" + "\n");
                        return;
                    }
                    
                    int spectralID = 1;
                    int scanNumber = 0;
                    float scanTime = 0;//seconds
                    Ywriter.InsertData(spectralID, scanNumber, scanTime, MZarray, IntArray, "+");
                    
                    spectralID = 1;
                    scanNumber = 1;
                    scanTime = 1;
                    Ywriter.InsertData(spectralID, scanNumber, scanTime, MZarray, IntArray, "+"); 
                    Ywriter.CloseYafms();
                   
                    YafmsReader Yreader = new YafmsReader();
                    Yreader.OpenYafms(outputFile);

                    double[] doubleArray = new double[sampleList.Count];
                    float[] floatArray = new float[sampleList.Count];

                    Yreader.GetSpectrum(1, 0, ref doubleArray, ref floatArray);
                    
                    Yreader.CloseYafms();

                    //LIFO  last in is first out
                    BoundedStack<int> bound = new BoundedStack<int>(2);
                    bound.Push(1);
                    bound.Push(2);
                    bound.Push(3);
                    bound.Push(4);

                    

                #endregion
                    break;
                default:
                        Console.WriteLine("Missing Program Number, Program");
                    break;

                    
            }

            #region stop timer + NotePad
                NotePad testcode = new NotePad();
                
            
                stopWatch.Stop();

				testcode.NotePadFX();
                TimeSpan ts = stopWatch.Elapsed;
                Console.WriteLine("This took " + ts + "seconds");
            #endregion

            //Keep console window open
            Console.ReadKey();
        }

        private static void Stopwatch()
        {
            throw new NotImplementedException();
        }

    }
}
