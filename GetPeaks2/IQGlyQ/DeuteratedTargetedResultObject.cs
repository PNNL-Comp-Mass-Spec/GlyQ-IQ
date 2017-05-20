using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;

namespace IQGlyQ
{
    public class DeuteratedTargetedResultObject : TargetedResultBase
    {
        #region Constructors
        public DeuteratedTargetedResultObject() : base() { }

        public DeuteratedTargetedResultObject(TargetBase target) : base(target) { }
        #endregion

        #region Properties

        public double RatioDH { get; set; }
        public double IntensityI0HydrogenMono { get; set; }
        #endregion

       
    }
}
