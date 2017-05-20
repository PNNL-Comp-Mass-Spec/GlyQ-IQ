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

namespace GridView4D
{
    class PopUpWorker
    {
        private volatile bool _shouldStop;

        public void DoLoadWork()
        {
            LoadingPopUp loadingPopUp = new LoadingPopUp();
            while (!_shouldStop)
            {
                loadingPopUp.Show();
            }
            loadingPopUp.Close();
        }

        public void DoSaveWork()
        {
            SavingPopUp savingPopUp = new SavingPopUp();
            while (!_shouldStop)
            {
                savingPopUp.Show();
            }
            savingPopUp.Close();
        }

        public void RequestStop()
        {
            _shouldStop = true;
        }

        public void RequestStart()
        {
            _shouldStop = false;
        }
    }
}
