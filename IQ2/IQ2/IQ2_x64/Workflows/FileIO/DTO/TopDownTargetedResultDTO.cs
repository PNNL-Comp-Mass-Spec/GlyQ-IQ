using System.Collections.Generic;

namespace IQ_X64.Workflows.FileIO.DTO
{
	public class TopDownTargetedResultDTO : UnlabelledTargetedResultDTO
	{
		public HashSet<int> PrsmList { get; set; }
		public List<int> ChargeStateList { get; set; }
		public float Quantitation { get; set; }

		public string ProteinName { get; set; }
		public double ProteinMass { get; set; }
		public string PeptideSequence { get; set; }
		public int MostAbundantChargeState { get; set; }
		public float ChromPeakSelectedHeight { get; set; }
	}
}
