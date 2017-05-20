/* Written by Myanna Harris
* for the Department of Energy (PNNL, Richland, WA)
* Battelle Memorial Institute
* E-mail: Myanna.Harris@pnnl.gov
* Website: http://omics.pnl.gov/software
* -----------------------------------------------------
* 
 * Notice: This computer software was prepared by Battelle Memorial Institute,
* hereinafter the Contractor, under Contract No. DE-AC05-76RL0 1830 with the
* Department of Energy (DOE).  All rights in the computer software are reserved
* by DOE on behalf of the United States Government and the Contractor as
* provided in the Contract.
* 
 * NEITHER THE GOVERNMENT NOR THE CONTRACTOR MAKES ANY WARRANTY, EXPRESS OR
* IMPLIED, OR ASSUMES ANY LIABILITY FOR THE USE OF THIS SOFTWARE.
* 
 * This notice including this sentence must appear on any copies of this computer
* software.
* -----------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlycolyzerGUImvvm.Models;
using System.IO;
using GlycolyzerGUImvvm.Views;
using System.Threading;
using System.ComponentModel;

namespace GlycolyzerGUImvvm.ViewModels
{
    public class GUIImport
    {
        public static ParameterModel parameterModel_Input = new ParameterModel();

        public static void ImportParameters(string path)
        {
            System.Xml.Serialization.XmlSerializer pReader = new System.Xml.Serialization.XmlSerializer(typeof(ParameterModel));
            System.IO.StreamReader pFile = new System.IO.StreamReader(path);
            parameterModel_Input = (ParameterModel)pReader.Deserialize(pFile);
            pFile.Close();
        }

        public static void ImportXMLParameterFiles(string type)
        {
            string readFolder = "";

            if (type == "auto")
                readFolder = "C:\\GlycolyzerApplicationFolder";
            else if (type == "customLoad")
                readFolder = App.folderModel_Save.SaveAppParametersLocationFolder_String;

            if (Directory.Exists(readFolder))
            {
                PopUpWorker loadWorkerObject = new PopUpWorker();
                Thread loadWorkerThread = new Thread(loadWorkerObject.DoLoadWork);
                loadWorkerObject.RequestStart();

                loadWorkerThread.SetApartmentState(ApartmentState.STA);
                loadWorkerThread.Start();
                while (!loadWorkerThread.IsAlive) ;
                Thread.Sleep(1);

                if (File.Exists(readFolder + "\\OmniFinderGM.xml"))
                {
                    System.Xml.Serialization.XmlSerializer omniFinderGMReader = new System.Xml.Serialization.XmlSerializer(typeof(OmniFinderGMModel));
                    System.IO.StreamReader omniFinderGMFile = new System.IO.StreamReader(readFolder + "\\OmniFinderGM.xml");
                    App.omniFinderGMModel_Save = (OmniFinderGMModel)omniFinderGMReader.Deserialize(omniFinderGMFile);
                    omniFinderGMFile.Close();
                }

                if (File.Exists(readFolder + "\\GlycanMaker.xml"))
                {
                    System.Xml.Serialization.XmlSerializer glycanMakerReader = new System.Xml.Serialization.XmlSerializer(typeof(GlycanMakerModel));
                    System.IO.StreamReader glycanMakerFile = new System.IO.StreamReader(readFolder + "\\GlycanMaker.xml");
                    App.glycanMakerModel_Save = (GlycanMakerModel)glycanMakerReader.Deserialize(glycanMakerFile);
                    glycanMakerFile.Close();
                }

                if (File.Exists(readFolder + "\\OmniFinderGMRanges.xml"))
                {
                    System.Xml.Serialization.XmlSerializer omniFinderGMRangesReader = new System.Xml.Serialization.XmlSerializer(typeof(RangesModel));
                    System.IO.StreamReader omniFinderGMRangesFile = new System.IO.StreamReader(readFolder + "\\OmniFinderGMRanges.xml");
                    App.omniFinderGMRangesModel_Save = (RangesModel)omniFinderGMRangesReader.Deserialize(omniFinderGMRangesFile);
                    omniFinderGMRangesFile.Close();
                }

                if (File.Exists(readFolder + "\\OmniFinderParameter.xml"))
                {
                    System.Xml.Serialization.XmlSerializer omniFinderParameterReader = new System.Xml.Serialization.XmlSerializer(typeof(OmniFinderModel));
                    System.IO.StreamReader omniFinderParameterFile = new System.IO.StreamReader(readFolder + "\\OmniFinderParameter.xml");
                    App.omniFinderModel_Save = (OmniFinderModel)omniFinderParameterReader.Deserialize(omniFinderParameterFile);
                    omniFinderParameterFile.Close();
                }

                if (File.Exists(readFolder + "\\Libraries.xml"))
                {
                    System.Xml.Serialization.XmlSerializer librariesReader = new System.Xml.Serialization.XmlSerializer(typeof(LibrariesModel));
                    System.IO.StreamReader librariesFile = new System.IO.StreamReader(readFolder + "\\Libraries.xml");
                    App.librariesModel_Save = (LibrariesModel)librariesReader.Deserialize(librariesFile);
                    librariesFile.Close();
                }

                if (File.Exists(readFolder + "\\Folders.xml"))
                {
                    System.Xml.Serialization.XmlSerializer folderReader = new System.Xml.Serialization.XmlSerializer(typeof(FolderModel));
                    System.IO.StreamReader folderFile = new System.IO.StreamReader(readFolder + "\\Folders.xml");
                    App.folderModel_Save = (FolderModel)folderReader.Deserialize(folderFile);
                    folderFile.Close();
                }

                if (File.Exists(readFolder + "\\ExtraScienceParameter.xml"))
                {
                    System.Xml.Serialization.XmlSerializer extraScienceParameterReader = new System.Xml.Serialization.XmlSerializer(typeof(ExtraScienceParameterModel));
                    System.IO.StreamReader extraScienceParameterFile = new System.IO.StreamReader(readFolder + "\\ExtraScienceParameter.xml");
                    App.extraScienceParameterModel_Save = (ExtraScienceParameterModel)extraScienceParameterReader.Deserialize(extraScienceParameterFile);
                    extraScienceParameterFile.Close();
                }

                if (File.Exists(readFolder + "\\ParameterRanges.xml"))
                {
                    System.Xml.Serialization.XmlSerializer parameterRangesReader = new System.Xml.Serialization.XmlSerializer(typeof(RangesModel));
                    System.IO.StreamReader parameterRangesFile = new System.IO.StreamReader(readFolder + "\\ParameterRanges.xml");
                    App.parameterRangesModel_Save = (RangesModel)parameterRangesReader.Deserialize(parameterRangesFile);
                    parameterRangesFile.Close();
                }

                loadWorkerObject.RequestStop();

                PopUpWorker loadWorkerObject1 = new PopUpWorker();
                Thread loadWorkerThread1 = new Thread(loadWorkerObject1.FinishLoadWork);
                loadWorkerObject1.RequestStart();

                loadWorkerThread1.SetApartmentState(ApartmentState.STA);
                loadWorkerThread1.Start();
                while (!loadWorkerThread1.IsAlive) ;
                Thread.Sleep(1000);

                loadWorkerObject1.RequestStop();

                App.folderModel_Save.SaveAppParametersLocationFolder_String = "C:\\GlycolyzerApplicationFolder";
            }

            ParameterViewModel.RunChangedAction("AutoFileName");
        }
    }
}
