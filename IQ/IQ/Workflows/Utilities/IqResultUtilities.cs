
using System.Collections.Generic;
using DeconTools.Workflows.Backend.Core;
using IQ.Workflows.Core;

namespace IQ.Workflows.Utilities
{
    public class IqResultUtilities
    {

        #region Constructors
        #endregion

        #region Properties

        #endregion

        #region Public Methods


        public List<IqResult> FlattenOutResultTree(IqResult iqResult)
        {
            List<IqResult> flattenedResults = new List<IqResult>();

            if (iqResult.HasChildren())
            {
                var childresults = iqResult.ChildResults();
                foreach (var childResult in childresults)
                {
                    var moreResults = FlattenOutResultTree(childResult);
                    flattenedResults.AddRange(moreResults);
                }
            }

            flattenedResults.Add(iqResult);


            return flattenedResults;

        }


        #endregion

        #region Private Methods

        #endregion

    }
}
