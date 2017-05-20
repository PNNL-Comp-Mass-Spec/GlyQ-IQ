using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend;
using DeconTools.Backend.Core;

namespace GetPeaks_DLL.Go_Decon_Modules
{
    public class GoResults:IDisposable
    {
        public List<StandardIsosResult> IsosResultList { get; set; }
        //public List<IsosResult> IsosResultList { get; set; }

        public GoResults()
        {
            IsosResultList = new List<StandardIsosResult>();
            //IsosResultList = new List<IsosResult>();
        }

        #region IDisposable Members

        public void  Dispose()
        {
            foreach (StandardIsosResult isos in IsosResultList)
            {
                isos.IsotopicProfile.Peaklist.Clear();
                isos.IsotopicProfile.Peaklist = null;
                //isos.ScanSet.IndexValues = null;
            }
            foreach (StandardIsosResult isos in IsosResultList)
            {
                isos.IsotopicProfile = null;
                isos.ScanSet = null;
                isos.Run = null;
            }
            this.IsosResultList = null;

        }

        #endregion

    }
}
