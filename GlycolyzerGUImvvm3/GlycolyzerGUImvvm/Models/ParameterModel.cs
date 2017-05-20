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
