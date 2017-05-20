using DeconTools.Backend.Core;
using DeconTools.Workflows.Backend.Results;

namespace Sipper.Model
{
    public class Project
    {

        #region Constructors

        public Project()
        {
            ResultRepository = new TargetedResultRepository();
            FileInputs = new FileInputsInfo();
        }

        public Project(TargetedResultRepository targetedResultRepository, FileInputsInfo fileInputsInfo)
        {
            ResultRepository = targetedResultRepository;
            FileInputs = fileInputsInfo;
        }


        #endregion

        #region Properties

        public TargetedResultRepository ResultRepository { get; set; }

        public FileInputsInfo FileInputs { get; set; }

        public Run Run { get; set; }

        #endregion

    

    }
}
