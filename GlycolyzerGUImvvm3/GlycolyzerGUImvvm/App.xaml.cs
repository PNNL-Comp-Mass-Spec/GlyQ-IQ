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
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using GlycolyzerGUImvvm.Views;
using GlycolyzerGUImvvm.ViewModels;
using System.Windows.Resources;
using System.Windows.Controls;
using GlycolyzerGUImvvm.Models;
using System.IO;
using System.ComponentModel;
using System.Threading;

namespace GlycolyzerGUImvvm
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static InitializingFlagsModel initializingFlagsModel;

        public static ParameterPage parameterPage;
        public static OmniFinderGMPage omniFinderGMPage;
        public static OmniFinderGMRangesPage omniFinderGMRangesPage;

        public static OmniFinderGMModel omniFinderGMModel_Save = new OmniFinderGMModel();
        public static GlycanMakerModel glycanMakerModel_Save = new GlycanMakerModel();
        public static RangesModel omniFinderGMRangesModel_Save = new RangesModel();

        public static OmniFinderModel omniFinderModel_Save = new OmniFinderModel();
        public static LibrariesModel librariesModel_Save = new LibrariesModel();
        public static FolderModel folderModel_Save = new FolderModel();
        public static ExtraScienceParameterModel extraScienceParameterModel_Save = new ExtraScienceParameterModel();
        public static RangesModel parameterRangesModel_Save = new RangesModel();

        public static String rangesBGColor;


        protected override void OnStartup(StartupEventArgs e) 
        { 
            base.OnStartup(e);
            initializingFlagsModel = new InitializingFlagsModel();
            GUIImport.ImportXMLParameterFiles("auto");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            GUIExport.ExportXMLParameterFiles("autoSave");
        }
    }
}
