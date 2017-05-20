using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ConsoleApplication1
{

    public class GlycoPeptideResultsToTable
    {
        public void ConvertToTable(GlycoPeptideResults IncommingResults, out DataTable GPtableHits, out DataTable GlycopeptideTableUnMatched, out DataTable GlycopeptideTableOtherInfo)
        {
            GPtableHits = new DataTable();
            GPtableHits.Columns.Add("Site", typeof(int));//0
            GPtableHits.Columns.Add("Peptide", typeof(string));//1
            GPtableHits.Columns.Add("GPMassExperimentalHit", typeof(double));//2
            GPtableHits.Columns.Add("GPMassExperimentalHitIndex", typeof(int));//3
            GPtableHits.Columns.Add("GlycanMassExperimentalHit", typeof(double));//4
            GPtableHits.Columns.Add("GlycanMassExperimentalHitIndex", typeof(int));//5

            GlycopeptideTableUnMatched = new DataTable();
            GlycopeptideTableUnMatched.Columns.Add("GPMassExperimentalMisses", typeof(double));
            GlycopeptideTableUnMatched.Columns.Add("GPMassExperimentalMissesIndex", typeof(int));

            GlycopeptideTableOtherInfo = new DataTable();
            GlycopeptideTableOtherInfo.Columns.Add("FASTA", typeof(string));
            GlycopeptideTableOtherInfo.Columns.Add("Tolerance", typeof(string));

            for (int i = 0; i < IncommingResults.glycoPeptideMassesHitsXY.Count; i++)
            {
                GPtableHits.Rows.Add();
                GPtableHits.Rows[i][0] = IncommingResults.PeptideSiteLocation[i];//site location
                GPtableHits.Rows[i][1] = IncommingResults.PeptideSitesListHits[i];//peptide sequence
                GPtableHits.Rows[i][2] = IncommingResults.glycoPeptideMassesHitsXY[i].X;//intact experimental glycopeptide mass
                GPtableHits.Rows[i][3] = IncommingResults.glycoPeptideHitIndex[i];//corresponding index for glycopeptide
                GPtableHits.Rows[i][4] = IncommingResults.glycanResidulesHitsMass[i];//glycan experimental mass after peptide was subtracted
                GPtableHits.Rows[i][5] = IncommingResults.glycanLibraryHitsIndex[i];//corresponding index for glycan
               
            }

            for (int i = 0; i < IncommingResults.glycoPeptideMassesMissesXY.Count; i++)
            {
                GlycopeptideTableUnMatched.Rows.Add();
                GlycopeptideTableUnMatched.Rows[i][0] = IncommingResults.glycoPeptideMassesMissesXY[i].X;//un matched glycopeptide masses
                GlycopeptideTableUnMatched.Rows[i][1] = IncommingResults.glycoPeptideMissIndex[i];//corresponding un matched mass index

            }

            GlycopeptideTableOtherInfo.Rows.Add();
            GlycopeptideTableOtherInfo.Rows[0][0] = IncommingResults.FASTA;
            GlycopeptideTableOtherInfo.Rows[0][1] = IncommingResults.Tollerance;


            int y = 44;
            y = y * 55;

            //return GPtableHits;
        }
    }
}


//GlycopeptideTable.Rows.Add();

        
//            GlycopeptideTable.Rows[0][0] = 3;
//            GlycopeptideTable.Rows[0][1] = 55.5555;
//            GlycopeptideTable.Rows[0][2] = "NRT";

//            GlycopeptideTable.Rows.Add(3,55.55,"NRTL");
//            GlycopeptideTable.Rows.Add(4, 54.33, "NR");