using IQ_X64.Workflows.WorkFlowPile;

namespace IQ_X64.Workflows.Core
{
    public class IqChargeStateTarget : IqTarget
    {
        public IqChargeStateTarget()
        {
            
        }

        public IqChargeStateTarget(IqWorkflow workflow) : base(workflow)
        {

        }

		/// <summary>
		/// Strictly used during the data importing process. Parent uses this value to calculate overall NET for the sequence. Once a NET value is calculated, this value is not needed anymore.
		/// </summary>
	    public int ObservedScan;
    }
}
