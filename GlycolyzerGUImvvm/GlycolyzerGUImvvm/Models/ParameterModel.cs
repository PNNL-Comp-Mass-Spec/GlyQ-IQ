using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlycolyzerGUImvvm.Models
{
    public class ParameterModel : ObservableObject
    {
        private OmniFinderModel omniFinderModel_Save = new OmniFinderModel();
        private LibrariesModel librariesModel_Save = new LibrariesModel();
        private FolderModel folderModel_Save = new FolderModel();
        private ExtraScienceParameterModel extraScienceParameterModel_Save = new ExtraScienceParameterModel();
        private RangesModel parameterRangesModel_Save = new RangesModel();

        public OmniFinderModel OmniFinderModel_Save
        {
            get { return omniFinderModel_Save; }
            set { if (value != omniFinderModel_Save) { omniFinderModel_Save = value; OnPropertyChanged("omniFinderModel_Save"); } }
        }

        public LibrariesModel LibrariesModel_Save
        {
            get { return librariesModel_Save; }
            set { if (value != librariesModel_Save) { librariesModel_Save = value; OnPropertyChanged("librariesModel_Save"); } }
        }

        public FolderModel FolderModel_Save
        {
            get { return folderModel_Save; }
            set { if (value != folderModel_Save) { folderModel_Save = value; OnPropertyChanged("folderModel_Save"); } }
        }

        public ExtraScienceParameterModel ExtraScienceParameterModel_Save
        {
            get { return extraScienceParameterModel_Save; }
            set { if (value != extraScienceParameterModel_Save) { extraScienceParameterModel_Save = value; OnPropertyChanged("extraScienceParameterModel_Save"); } }
        }

        public RangesModel ParameterRangesModel_Save
        {
            get { return parameterRangesModel_Save; }
            set { if (value != parameterRangesModel_Save) { parameterRangesModel_Save = value; OnPropertyChanged("parameterRangesModel_Save"); } }
        }

        public ParameterModel()
        {
        }

        public ParameterModel(OmniFinderModel omniFinderModel_SaveP, LibrariesModel librariesModel_SaveP, 
            FolderModel folderModel_SaveP, ExtraScienceParameterModel extraScienceParameterModel_SaveP, RangesModel parameterRangesModel_SaveP)
        {
            OmniFinderModel_Save = omniFinderModel_SaveP;
            LibrariesModel_Save = librariesModel_SaveP;
            FolderModel_Save = folderModel_SaveP;
            ExtraScienceParameterModel_Save = extraScienceParameterModel_SaveP;
            ParameterRangesModel_Save = parameterRangesModel_SaveP;
        }
    }
}
