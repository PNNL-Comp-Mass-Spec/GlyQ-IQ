using System.Collections.Generic;
using IQ.Workflows.FileIO.DTO;
using Run32.Backend.Core.Results;

namespace IQ.Workflows.FileIO
{
    public class TargetedResultRepository
    {

        #region Constructors
        public TargetedResultRepository()
        {
            this.Results = new List<TargetedResultDTO>();
        }
        #endregion

        #region Properties
        public List<TargetedResultDTO> Results { get; set; }


        #endregion

        public void AddResult(TargetedResultBase resultToConvert)
        {
            TargetedResultDTO result = ResultDTOFactory.CreateTargetedResult(resultToConvert);
            this.Results.Add(result);
        }



        public void AddResults(List<TargetedResultDTO> featuresToAlign)
        {
            this.Results.AddRange(featuresToAlign);
        }

        public void AddResults(List<TargetedResultBase> resultsToConvert)
        {
            foreach (var item in resultsToConvert)
            {
                TargetedResultDTO result = ResultDTOFactory.CreateTargetedResult(item);
                this.Results.Add(result);
            }
        }


        public void Clear()
        {
            this.Results.Clear();
        }



        public bool HasResults
        {
            get
            { return (this.Results != null && this.Results.Count > 0); }
        }
    }
}
