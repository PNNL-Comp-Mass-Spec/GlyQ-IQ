using System;
using System.Collections.Generic;
using IQ_X64.Workflows.Core;

namespace IQGlyQ.Results
{
    [Serializable]
    public class IqGlyQResult : IqResult
    {
        public IqGlyQResult(IqTarget target)
            : base(target)
        {
            FutureTargets = new List<FragmentResultsObjectHolderIq>();
            TargetAddOns = new List<FragmentResultsObjectHolderIq>();
            FragmentIQTarget convertedTarget = new FragmentIQTarget(target);
            ToChild = new FragmentResultsObjectHolderIq(convertedTarget);
            DidThisWork = false;
        }

        /// <summary>
        /// bool for did this work
        /// </summary>
        public bool DidThisWork { get; set; }

        /// <summary>
        /// list of successfull hits
        /// </summary>
        public List<FragmentResultsObjectHolderIq> TargetAddOns { get; set; }

        /// <summary>
        /// list of future targets
        /// </summary>
        public List<FragmentResultsObjectHolderIq> FutureTargets { get; set; }

        /// <summary>
        /// location for output data sent to the printer
        /// </summary>
        public FragmentResultsObjectHolderIq ToChild { get; set; }

        /// <summary>
        /// error name
        /// </summary>
        public string Error { get; set; }
    }
}
