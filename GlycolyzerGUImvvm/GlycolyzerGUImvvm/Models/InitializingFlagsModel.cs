using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlycolyzerGUImvvm.Models
{
    public class InitializingFlagsModel : ObservableObject
    {
        //Initialize Page Flags
        private Boolean parameterModel_InitializeFlag = true;
        private Boolean omniFinderGMModel_InitializeFlag = true;
        private Boolean parameterRangesSave_InitializeFlag = false;
        private Boolean omniFinderGMRangesSave_InitializeFlag = false;
        private Boolean tab4_ResizeFlag = false;


        public Boolean ParameterModel_InitializeFlag
        {
            get { return parameterModel_InitializeFlag; }
            set { parameterModel_InitializeFlag = value; OnPropertyChanged("parameterModel_InitializeFlag"); }
        }

        public Boolean OmniFinderGMModel_InitializeFlag
        {
            get { return omniFinderGMModel_InitializeFlag; }
            set { omniFinderGMModel_InitializeFlag = value; OnPropertyChanged("omniFinderGMModel_InitializeFlag"); }
        }

        public Boolean ParameterRangesSave_InitializeFlag
        {
            get { return parameterRangesSave_InitializeFlag; }
            set { parameterRangesSave_InitializeFlag = value; OnPropertyChanged("saveParameterRangesModel_InitializeFlag"); }
        }

        public Boolean OmniFinderGMRangesSave_InitializeFlag
        {
            get { return omniFinderGMRangesSave_InitializeFlag; }
            set { omniFinderGMRangesSave_InitializeFlag = value; OnPropertyChanged("saveOmniFinderGMRangesModel_InitializeFlag"); }
        }

        public Boolean Tab4_ResizeFlag
        {
            get { return tab4_ResizeFlag; }
            set { tab4_ResizeFlag = value; OnPropertyChanged("tab4_ResizeFlag"); }
        }
    }
}
