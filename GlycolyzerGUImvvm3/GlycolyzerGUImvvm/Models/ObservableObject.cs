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
using System.ComponentModel;
using GlycolyzerGUImvvm.ViewModels;

namespace GlycolyzerGUImvvm.Models
{
    public class ObservableObject : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members
        /// <summary>  
        /// Raised when a property on this object has a new value.  
        /// </summary>  
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>  
        /// Raises this object's PropertyChanged event.  
        /// </summary>  
        /// <param name="propertyName">The property that has a new value.</param>  
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);

                if (e.PropertyName == "inputDataFile_String" || e.PropertyName == "chosenDefaultLibrary_String" ||
                    e.PropertyName == "mzToleranceExtraParameter_Double" || e.PropertyName == "carbohydrateTypeExtraParameter_String" ||
                    e.PropertyName == "chargeCarryingSpeciesExtraParameter_String")
                {
                    ParameterViewModel.RunChangedAction("AutoFileName");
                }
                else if (e.PropertyName == "selectedOption_String")
                {
                    if (App.initializingFlagsModel.ParameterRangesSave_InitializeFlag)
                    {
                        OmniFinderViewModel.SelectedOption_Changed();
                    }
                    else if (App.initializingFlagsModel.OmniFinderGMRangesSave_InitializeFlag)
                    {
                        OmniFinderGMViewModel.SelectedOption_Changed();
                    }
                }
                else if (e.PropertyName == "chosenDefaultLibrary_String")
                {
                    ParameterViewModel.RunChangedAction("GetDefaultAddress");
                }
                else if (e.PropertyName == "inputDataFileType_String")
                {
                    ParameterViewModel.RunChangedAction("ClearInputDataString");
                }

                this.PropertyChanged(this, e);
            }
        }
        #endregion // INotifyPropertyChanged Members
    }
}
