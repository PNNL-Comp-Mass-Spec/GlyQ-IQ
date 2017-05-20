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
    public class LibrariesModel : ObservableObject
    {
        private String chosenDefaultLibrary_String = "No Library Selected";
        private String chosenCustomLibrary_String = "";


        public String ChosenDefaultLibrary_String
        {
            get { return chosenDefaultLibrary_String; }
            set { if (value != chosenDefaultLibrary_String) { chosenDefaultLibrary_String = value; OnPropertyChanged("chosenDefaultLibrary_String"); } }
        }

        public String ChosenCustomLibrary_String
        {
            get { return chosenCustomLibrary_String; }
            set { if (value != chosenCustomLibrary_String) { chosenCustomLibrary_String = value; OnPropertyChanged("chosenCustomLibrary_String"); } }
        }
    }
}
