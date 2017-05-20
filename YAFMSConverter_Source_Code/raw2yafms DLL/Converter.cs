using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRAWFILE2Lib;
using YafmsLibrary;
using System.IO;

namespace RawConvertToYafms_DLL
{
    public class RawConvertToYafms
    {
        XRawfile ixraw = new XRawfile();
        YafmsWriter dw = new YafmsWriter();
        string rawfile = null;
        string yafmsfile = null;
        int first_spectrum_num = 0;
        int last_spectrum_num = 0;
        int numSpectra = 0;

        public static void Launch(string fileName)
        {
            if (fileName.Length < 1)
            {
                Console.WriteLine("Usage: RawConvertToYafms rawfile [outfile]\n");
                Console.WriteLine("If outfile is not specified, then outfile = rawfile_root.yafms\n");
                Console.WriteLine("where rawfile_root is rawfile without extension\n");
                Console.WriteLine("Press Return to end this process\n");
                Console.ReadLine();
                return;
            }

            RawConvertToYafms r2y = new RawConvertToYafms();
            r2y.rawfile = fileName;
            //r2y.rawfile = args[0];
            string[] fileroot = fileName.Split(new char[] { '.' });
            //string[] fileroot = args[0].Split(new char[] { '.' });
    //        if (args.Length < 2)
    //        {
                r2y.yafmsfile = fileroot[0] + ".yafms";
    //      }
    //        else
    //        {
    //            r2y.yafmsfile = args[1];
    //        }

            if (!File.Exists(r2y.rawfile))
            {
                Console.WriteLine("Input file " + r2y.rawfile + " does not exist\n");
                Console.WriteLine("Usage: RawConvertToYafms rawfile [outfile]\n");
                Console.WriteLine("If outfile is not specified, then outfile = rawfile_root.yafms\n");
                Console.WriteLine("where rawfile_root is rawfile without extension\n");
                Console.WriteLine("Press Return to end this process\n");
                Console.ReadLine();
                return;
            }

            if (File.Exists(r2y.yafmsfile))
            {
                File.Delete(r2y.yafmsfile);
            }

            Console.WriteLine("Input file: \n    " + r2y.rawfile + "\n");
            Console.WriteLine("Output file: \n    " + r2y.yafmsfile + "\n");

            r2y.OpenRawFile();
            r2y.OpenYafmsFile();
            r2y.Insert_DatasetInfo();
            r2y.Insert_Data();
            r2y.CloseRawFile();
            r2y.CloseYafmsFile();
        } 
          
        void OpenRawFile()
        {
            try
            {
                ixraw.Open(rawfile);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        void CloseRawFile()
        {
            try
            {
                ixraw.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        void OpenYafmsFile()
        {
            //Open a new yafms file and create tables
            try
            {
                dw.OpenYafms(yafmsfile);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        void CloseYafmsFile()
        {
            try
            {
                dw.CloseYafms();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        void Insert_DatasetInfo()
        {
            //Get data from rawfile
            try
            {
                string inst_model = null;
                string inst_SoftwareVersion = null;
                ixraw.SetCurrentController(0, 1);
                ixraw.GetInstModel(ref inst_model);
                ixraw.GetInstSoftwareVersion(ref inst_SoftwareVersion);
                ixraw.GetFirstSpectrumNumber(ref first_spectrum_num);
                ixraw.GetLastSpectrumNumber(ref last_spectrum_num);
                ixraw.GetNumSpectra(ref numSpectra);

                //Insert info into Dataset_Info table
                dw.InsertDataInfo("Source File", rawfile);
                dw.InsertDataInfo("Scan Count", numSpectra.ToString());
                dw.InsertDataInfo("Instrument Vendor", "Thermo Scientific");
                if (inst_model != null)
                {
                    dw.InsertDataInfo("Instrument Model", inst_model);
                }

                string scan_description = null;
                ixraw.GetFilterForScanNum(1, ref scan_description);
                int index = scan_description.IndexOf("NSI");
                
                if (index >= 0)
                {
                    dw.InsertDataInfo("Ionization Method", scan_description.Substring(index, 3));
                }
                else dw.InsertDataInfo("Ionization Method", "Unknown");
                {
                    index = scan_description.IndexOf("FTMS");
                }

                if (index >= 0)
                {
                    dw.InsertDataInfo("Mass Analyzer", scan_description.Substring(index, 4));
                }
                else
                {
                    index = scan_description.IndexOf("ITMS");
                    if (index >= 0)
                    {
                        dw.InsertDataInfo("Mass Analyzer", scan_description.Substring(index, 4));
                    }
                    else
                    {
                        dw.InsertDataInfo("Mass Analyzer", "Unknown");
                    }
                }

                index = scan_description.IndexOf("Full");
                if (index >= 0)
                {
                    dw.InsertDataInfo("Scan Type", scan_description.Substring(index, 4));
                }
                else
                {
                    dw.InsertDataInfo("Scan Type", "Unknown");
                }

                dw.InsertDataInfo("Ion Detector", "Unknown");
                
                if (inst_SoftwareVersion != null) dw.InsertDataInfo("Instrument Software Aquisition", "XCalibur", inst_SoftwareVersion);
                
                dw.InsertDataInfo("Mass Analyer Resolution Method", "Unknown");

                //[YS] Another way to find the fields, but I am not sure if the format of scan_description is fixed in the raw file
                //dw.InsertInfo("Mass Analyzer", scan_description.Substring(0, 4));
                //dw.InsertInfo("Ionization Method", scan_description.Substring(9, 3));
                //dw.InsertInfo("Scan Type", scan_description.Substring(13,4));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        void Insert_Data()
        {
            //Insert data into Spectra_Info and Spectra_Data tables
            try
            {
                //dw.PrepareInsertData(); Aaron: June 14, 2010
                int count = 0;
                int t = 1;
                int PrecursorScanNum = 0;
                int precursorScanNum_next = 0;

                for (int scan_num = first_spectrum_num; scan_num <= last_spectrum_num; scan_num++)
                {
                    int ms_level = GetMSLevel(scan_num);
                    int ms_level_next = 0;

                    if (scan_num + t <= last_spectrum_num)
                    {
                        ms_level_next = GetMSLevel(scan_num + t);
                    }

                    while (scan_num + t < last_spectrum_num && ms_level < ms_level_next)
                    {
                        count++;
                        t++;
                        ms_level_next = GetMSLevel(scan_num + t);

                        if (scan_num + t == last_spectrum_num && ms_level < ms_level_next)
                        {
                            count++;
                        }
                    }

                    //if it has fragments, insert info into Spectra_Info table
                    if (count > 0)
                    {
                        dw.InsertSpectraInfo(1, scan_num, "Fragment Count", count.ToString());
                        count = 0;
                        t = 1;
                        PrecursorScanNum = scan_num;
                        precursorScanNum_next = scan_num + 1;
                    }

                    //Get scan description from rawfile
                    string scan_description = null;
                    ixraw.GetFilterForScanNum(scan_num, ref scan_description);

                    //Get Polarity
                    string polarity = "+";
                    int index = scan_description.IndexOf(polarity);
                    if (index == -1) polarity = "-";

                    //Get scanTime, tic, bpi_mz, bpi
                    double scanTime = 0;
                    int numPackets = 0;
                    double lowMass = 0;
                    double highMass = 0;
                    double tic = 0;
                    double bpi_mz = 0;
                    double bpi = 0;
                    int numChannels = 0;
                    int uniformTime = 0;
                    double frequency = 0;
                    ixraw.GetScanHeaderInfoForScanNum(scan_num, ref numPackets, ref scanTime, ref lowMass, ref highMass,
                        ref tic, ref bpi_mz, ref bpi, ref numChannels, ref uniformTime, ref frequency);

                    //Get PrecusorMZ
                    object pVal = null;
                    ixraw.GetTrailerExtraValueForScanNum(scan_num, "Monoisotopic M/Z:", ref pVal);
                    double PrecursorMZ = (double)pVal;
                    
                    //For Velos data, the PrecursorMZ is not extracted so we go and get it from the string
                    if (PrecursorMZ == 0)//Scott
                    {
                        PrecursorMZ = extractPrecursorMZfromScanDescription(scan_description, PrecursorMZ);
                    }

                    //Get MassList
                    string t1 = null;
                    double peak_width = 0;
                    object MassList = null;
                    object PeakFlags = null;
                    int ArraySize = 0;
                    ixraw.GetMassListFromScanNum(ref scan_num, t1, 0, 0, 0, 0, ref peak_width, ref MassList, ref PeakFlags, ref ArraySize);

                    //Get mz and intensities from Masslist
                    double[,] mz_intensities = (double[,])MassList;
                    double[] mz = new double[ArraySize];
                    float[] intensities = new float[ArraySize];
                    if (ArraySize > 0)
                    {
                        for (int i = 0; i < ArraySize; i++)
                        {
                            intensities[i] = (float)mz_intensities[1, i];
                            mz[i] = mz_intensities[0, i];
                        }
                        //Insert data into Spectra_Data table
                        if (precursorScanNum_next > PrecursorScanNum && PrecursorScanNum != scan_num)
                        {
                            dw.InsertData(1, scan_num, (float)scanTime, mz, intensities, (float)tic, (float)bpi, bpi_mz, polarity, PrecursorScanNum, PrecursorMZ);
                        }
                        else
                        {
                            dw.InsertData(1, scan_num, (float)scanTime, mz, intensities, (float)tic, (float)bpi, bpi_mz, polarity);
                        }
                    }
                }
                //dw.EndInsertData(); Aaron: June 14, 2010
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            #if PRODUCTION
            Console.WriteLine("Finished creating database. Spectra range: " + first_spectrum_num + " to " + last_spectrum_num);
            Console.WriteLine("Press Return to end this process");
            Console.ReadLine();
            #endif

        }

        private static double extractPrecursorMZfromScanDescription(string scan_description, double PrecursorMZ)
        {
            int index = scan_description.IndexOf("@");//If there is a "@" it is a tandem spectra

            if (index > 0)//if "@" was found, it will have a index >0
            {
                int startIndex = scan_description.IndexOf("ms") + 2;//where to start.  +2 takes into account the space
                string decidingValue = scan_description.Substring(startIndex, 1);//MS will have a space (MS_) instead of an integer (MS2)
                int testInt = 0;
                try
                {
                    testInt = Convert.ToInt32(decidingValue);//a space will fail
                }
                catch
                {
                    testInt = 0;
                }

                if (testInt > 0)//we are certain it is a ms2 or higher scan
                {
                    startIndex += 2;//offset text
                    int stopIndex = index - startIndex;
                    string precursorMZstring = scan_description.Substring(startIndex, stopIndex);

                    try
                    {
                        PrecursorMZ = Convert.ToDouble(precursorMZstring);//final string conversion to double
                    }
                    catch
                    {
                        PrecursorMZ = 0;
                    }
                }

            }
            else
            {
                PrecursorMZ = 0;
            }

            return PrecursorMZ;
        }

        private int GetMSLevel(int scan_num)
        {
            try
            {
                //Extract FilterLine from rawfile
                string FilterLine = null;
                ixraw.GetFilterForScanNum(scan_num, ref FilterLine);

                //Get ms_level
                int ms_level = 1;
                int idx = FilterLine.IndexOf("ms");
                string sLevel = FilterLine.Substring(idx + 2, 1);
                if (sLevel == " ")
                {
                    ms_level = 1;
                }
                else
                {
                    ms_level = Convert.ToInt32(sLevel);
                }
                return ms_level;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }   
        }
    }
}
