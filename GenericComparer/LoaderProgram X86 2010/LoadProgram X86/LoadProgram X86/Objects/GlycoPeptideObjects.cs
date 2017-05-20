using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;

namespace ConsoleApplication1
{
    public class Protein2 : Molecule
    {
        private string m_sequence;

        public string Sequence
        {
            get { return m_sequence; }
            set { m_sequence = value; }
        }
    }

    class peptidePosibilitiesPerSite
    {
        public List<List<string>> PeptideSitesList = new List<List<string>>();
        public List<List<double>> PeptideMassList = new List<List<double>>();

		public List<List<double>> glycanResidulesMass = new List<List<double>>();
		public List<List<string>> glycanResidulesPeptides = new List<List<string>>();
		public List<List<double>> glycanResidulesPeptideMass = new List<List<double>>();
        public List<List<int>> glycanResidulesGlycoPeptideIndex = new List<List<int>>();//raw data
        public List<List<double>> glycanResidulesGlycoPeptideMass = new List<List<double>>();//raw data

        public List<int> siteLocationList = new List<int>();
		public List<string> siteTypeList = new List<string>();
		

        public void AddPeptidePossibiities(List<string> peptidesIn)
        {
            this.PeptideSitesList.Add(peptidesIn);      
        }

        public void AddPeptideMassPossibiities(List<double> peptidesIn)
        {
            this.PeptideMassList.Add(peptidesIn);
        }

		public void AddglycanResidulesMass(List<double> glycanIn)
		{
			this.glycanResidulesMass.Add(glycanIn);
		}

		public void AddglycanResidulesPeptides(List<string> peptidesIn)
		{
			this.glycanResidulesPeptides.Add(peptidesIn);
		}

		public void AddglycanResidulesPeptideMass(List<double> peptidesIn)
		{
			this.glycanResidulesPeptideMass.Add(peptidesIn);
		}

        public void SetSiteLocation(int siteLocationIn)
        {
            this.siteLocationList.Add(siteLocationIn);
        }

		public void SetSiteType(string siteTypeIn)
		{
			this.siteTypeList.Add(siteTypeIn);
		}

        
    }
    public class GlycoPeptideParameters
    {
        public int AminoAcidsToLeft { get; set; }
        public int AminoAcidsToRight { get; set; }
        public int DivisorValue  { get; set; }//used to divide the max mass criteria to produce smaller peptides
        public int CoreToggle { get; set; }//1=on, 0=off

        public double MaxMass { get; set; }
        public double CoreMass { get; set; }
        public double Tollerance { get; set; }//Da at this point
        
        public string SiteType { get; set; }
        public string FASTA { get; set; }
        public string BoundingMethod  { get; set; }
        
        public List<XYData> glycoPeptideMasses { get; set; }
        public List<XYData> glycanLibraryMasses { get; set; }

        public void SetPeptideRange(int AminoAcidsToLeftIN, int AminoAcidsToRightIN)
        {
            this.AminoAcidsToLeft = AminoAcidsToLeftIN;
            this.AminoAcidsToRight = AminoAcidsToRightIN;
        }
    }
    public class GlycoPeptideResults
    {
        public double Tollerance { get; set; }//error tolerance
        public string FASTA { get; set; }//working FASTA

        //glycopeptide
        public List<XYData> glycoPeptideMassesXYCopy { get; set; }//Masses of interest
        public List<XYData> glycoPeptideMassesHitsXY { get; set; }//Masses of interest
        public List<XYData> glycoPeptideMassesMissesXY { get; set; }//Masses of interest
        public List<int> glycoPeptideHitIndex { get; set; }//Masses of interest
        public List<int> glycoPeptideMissIndex { get; set; }//Masses of interest

        //peptide
        public List<string> PeptideSitesListHits { get; set;}//all Peptide Hits
        public List<double> PeptideSitesMassHits { get; set; }//all Peptide Hits
        
        //glycan
        public List<decimal> glycanResidulesHitsMass { get; set; }//all masses from Hits
        public List<int> glycanLibraryHitsIndex { get; set; }//index of coresponding glycan compositions for each hit
        public List<double> glycanLibraryHitsExactMass { get; set; }//exact mass from library

        //site info
        public List<int> PeptideSiteLocation { get; set; }//amino acid number
    }
}
