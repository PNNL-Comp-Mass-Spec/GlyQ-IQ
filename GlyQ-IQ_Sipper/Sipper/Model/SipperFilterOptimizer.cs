using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DeconTools.Backend.Utilities.IqLogger;
using DeconTools.Workflows.Backend.FileIO;
using DeconTools.Workflows.Backend.Results;

namespace Sipper.Model
{
    public class SipperFilterOptimizer
    {

        #region Constructors
        public SipperFilterOptimizer(string unlabeledResultsFilePath, string labeledResultsFilePath)
        {
            UnlabeledResultsFilePath = unlabeledResultsFilePath;
            LabeledResultsFilePath = labeledResultsFilePath;

            LabelFitLower = 0.2;
            LabelFitUpper = 1.1;
            LabelFitStep = 0.2;

            SumOfRatiosLower = 0;
            SumOfRatiosUpper = 10.5;
            SumOfRatiosStep = 1;

            IscoreLower = 0;
            IscoreUpper = 1.0;
            IscoreStep = 0.1;

            ContigScoreLower = 0;
            ContigScoreUpper = 7;
            ContigScoreStep = 1;

            PercentIncorpLower = 0;
            PercentIncorpUpper = 2;
            PercentIncorpStep = 0.5;

            PercentPeptidePopulationLower = 0;
            PercentPeptidePopulationUpper = 2;
            PercentPeptidePopulationStep = 0.5;



        }

        #endregion

        #region Properties

        #endregion

        #region Public Methods

        public int GetNumCombinations()
        {
            int numCombos = (int)((LabelFitUpper - LabelFitLower)/LabelFitStep);
            numCombos *= (int)((SumOfRatiosUpper - SumOfRatiosLower) / SumOfRatiosStep);
            numCombos *= (int)((IscoreUpper - IscoreLower) / IscoreStep);
            numCombos *= (int)((ContigScoreUpper - ContigScoreLower) / ContigScoreStep);
            numCombos *= (int)((PercentIncorpUpper - PercentIncorpLower) / PercentIncorpStep);
            numCombos *= (int)((PercentPeptidePopulationUpper - PercentPeptidePopulationLower) / PercentPeptidePopulationStep);

            return numCombos;
        }


        public List<ParameterOptimizationResult> DoFilterOptimization(string outputFileName = null)
        {
            bool isHeaderWritten = false;

            var parameterOptimizationResults = new List<ParameterOptimizationResult>();


            //load results
            SipperResultFromTextImporter importer = new SipperResultFromTextImporter(UnlabeledResultsFilePath);
            var c12Results = (from SipperLcmsFeatureTargetedResultDTO n in importer.Import().Results select n).ToList();

            importer = new SipperResultFromTextImporter(LabeledResultsFilePath);
            var c13Results = (from SipperLcmsFeatureTargetedResultDTO n in importer.Import().Results select n).ToList();

            int numCombinations = GetNumCombinations();

            IqLogger.Log.Info("Filter optimizer - num combinations to analyze = "+ numCombinations);
            StringBuilder sb = new StringBuilder();

            int comboCounter = 0;

            for (double fitScoreLabelled = LabelFitLower; fitScoreLabelled < LabelFitUpper; fitScoreLabelled = fitScoreLabelled + LabelFitStep)
            {
                sb.Clear();

                var levelOneC13Filter = (from n in c13Results
                                         where n.FitScoreLabeledProfile <= fitScoreLabelled
                                         select n).ToList();

                var levelOneC12Filter = (from n in c12Results
                                         where n.FitScoreLabeledProfile <= fitScoreLabelled
                                         select n).ToList();

                IqLogger.Log.Info("Current filter combo: " + comboCounter +" out of " + numCombinations);
                
                for (double area = SumOfRatiosLower; area < SumOfRatiosUpper; area = area + SumOfRatiosStep)
                {
                    var levelTwoC13Filter = (from n in levelOneC13Filter
                                             where n.AreaUnderRatioCurveRevised <= area
                                             select n).ToList();

                    var levelTwoC12Filter = (from n in levelOneC12Filter
                                             where n.AreaUnderRatioCurveRevised <= area
                                             select n).ToList();


                    for (double iscore = IscoreLower; iscore < IscoreUpper; iscore = iscore + IscoreStep)
                    {

                        var levelThreeC13Filter = (from n in levelTwoC13Filter
                                                 where n.AreaUnderRatioCurveRevised <= iscore
                                                 select n).ToList();

                        var levelThreeC12Filter = (from n in levelTwoC12Filter
                                                 where n.AreaUnderRatioCurveRevised <= iscore
                                                 select n).ToList();



                        for (int contigScore = ContigScoreLower; contigScore <= ContigScoreUpper; contigScore = contigScore + ContigScoreStep)
                        {
                            for (double percentIncorp = PercentIncorpLower; percentIncorp < PercentIncorpUpper; percentIncorp = percentIncorp + PercentIncorpStep)
                            {
                                for (double peptidePop = PercentPeptidePopulationLower; peptidePop < PercentPeptidePopulationUpper; peptidePop = peptidePop + PercentPeptidePopulationStep)
                                {
                                    var c13filteredResults = (from n in levelThreeC13Filter
                                                              where n.ContiguousnessScore >= contigScore
                                                             && n.PercentCarbonsLabelled >= percentIncorp
                                                             && n.PercentPeptideLabelled >= peptidePop
                                                              select n).ToList();

                                    var c12filteredResults = (from n in levelThreeC12Filter
                                                              where n.ContiguousnessScore >= contigScore
                                                             && n.PercentCarbonsLabelled >= percentIncorp
                                                             && n.PercentPeptideLabelled >= peptidePop
                                                              select n).ToList();


                                    ParameterOptimizationResult optimizationResult = new ParameterOptimizationResult();
                                    optimizationResult.FitScoreLabelled = fitScoreLabelled;
                                    optimizationResult.SumOfRatios = area;
                                    optimizationResult.Iscore = iscore;
                                    optimizationResult.ContigScore = contigScore;
                                    optimizationResult.PercentIncorp = percentIncorp;
                                    optimizationResult.PercentPeptidePopulation = peptidePop;

                                    optimizationResult.NumLabeledPassingFilter = c13filteredResults.Count;
                                    optimizationResult.NumUnlabelledPassingFilter = c12filteredResults.Count;

                                    sb.Append(optimizationResult.ToStringWithDetails());
                                    sb.Append(Environment.NewLine);

                                    parameterOptimizationResults.Add(optimizationResult);

                                    comboCounter++;

                                }
                            }


                        }
                    }
                }

                if (!string.IsNullOrEmpty(outputFileName))
                {
                    using (var sw = new StreamWriter(new FileStream(outputFileName, FileMode.Append, FileAccess.Write, FileShare.Read)))
                    {
                        sw.AutoFlush = true;

                        if (!isHeaderWritten)
                        {
                            sw.WriteLine("LabeledFit\tSumOfRatios\tIscore\tContigScore\tPercentIncorp\tPercentPeptideLabeled\tUnlabeledCount\tLabeledCount\tFalsePositiveRate");
                        }
                        sw.Write(sb.ToString());

                    }
                }

            }

            return parameterOptimizationResults;



        }



        #endregion

        #region Private Methods

        #endregion

        public string UnlabeledResultsFilePath { get; set; }

        public string LabeledResultsFilePath { get; set; }

        public double LabelFitLower { get; set; }

        public double LabelFitUpper { get; set; }

        public double LabelFitStep { get; set; }

        public double SumOfRatiosLower { get; set; }

        public double SumOfRatiosUpper { get; set; }

        public double SumOfRatiosStep { get; set; }

        public double IscoreLower { get; set; }

        public double IscoreUpper { get; set; }

        public double IscoreStep { get; set; }

        public int ContigScoreLower { get; set; }

        public int ContigScoreUpper { get; set; }

        public int ContigScoreStep { get; set; }

        public double PercentIncorpLower { get; set; }

        public double PercentIncorpUpper { get; set; }

        public double PercentIncorpStep { get; set; }

        public double PercentPeptidePopulationLower { get; set; }

        public double PercentPeptidePopulationUpper { get; set; }

        public double PercentPeptidePopulationStep { get; set; }
    }
}
