using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;
using DeconTools.Backend.Data;
using DeconTools.Backend.Runs;
using DeconTools.Backend.Utilities;
using DeconTools.Backend.Utilities.IsotopeDistributionCalculation;
using DeconTools.Workflows.Backend.Core;
using DeconTools.Workflows.Backend.FileIO;

namespace IQGlyQ.Executor
{
    public class GlyQIQExecutor2
    {

        private BackgroundWorker _backgroundWorker;

        private readonly IqResultUtilities _iqResultUtilities = new IqResultUtilities();
        private readonly IqTargetUtilities _targetUtilities = new IqTargetUtilities();
        private RunFactory _runFactory = new RunFactory();

        #region Constructors

        public GlyQIQExecutor2()
        {
            Parameters = new BasicTargetedWorkflowExecutorParameters();
            Results = new List<IqResult>();
            IsDataExported = true;
            DisposeResultDetails = true;
        }

        public GlyQIQExecutor2(WorkflowExecutorBaseParameters parameters)
            : this()
        {
            Parameters = parameters;
        }

        protected WorkflowExecutorBaseParameters Parameters { get; set; }

        protected bool IsDataExported { get; set; }

        #endregion

        #region Properties

        public bool DisposeResultDetails { get; set; }


        public IqTargetImporter TargetImporter { get; set; }



        #endregion


        #region Public Methods


        public string ChromSourceDataFilePath { get; set; }

        public List<IqResult> Results { get; set; }

        public List<IqTarget> Targets { get; set; }


        protected ResultExporter ResultExporter { get; set; }

        public void Execute()
        {
            Execute(Targets);
        }



        public void SetRun(string datasetPath)
        {
            Run = _runFactory.CreateRun(datasetPath);
        }


        public void SetRun(Run run)
        {
            Run = run;
        }


        public void Execute(IEnumerable<IqTarget> targets)
        {
            foreach (var target in targets)
            {
                Run = target.GetRun();

                if (!ChromDataIsLoaded)
                {
                    LoadChromData(Run);
                }

                target.DoWorkflow();
                var result = target.GetResult();

                if (IsDataExported)
                {
                    ExportResults(result);
                }

                Results.Add(result);

                if (DisposeResultDetails)
                {
                    result.Dispose();
                }
            }

        }




        public virtual void LoadAndInitializeTargets()
        {
            LoadAndInitializeTargets(Parameters.TargetsFilePath);
        }

        public virtual void LoadAndInitializeTargets(string targetsFilePath)
        {
            if (TargetImporter == null)
            {
                TargetImporter = new BasicIqTargetImporter(targetsFilePath);
            }

            Targets = TargetImporter.Import();

            //_targetUtilities.CreateChildTargets(Targets);
            CreateChildTargets(Targets);
        }

       

        #region added stuff

        public void CreateChildTargets(List<IqTarget> targets)
        {
            foreach (IqTarget iqTarget in targets)
            {
                UpdateTargetMissingInfo(iqTarget);
                //TODO make a parameter
                var childTargets = CreateChargeStateTargets(iqTarget, 400, 2000);
                iqTarget.AddTargetRange(childTargets);
            }
        }

        public List<IqTarget> CreateChargeStateTargets(IqTarget iqTarget, double minMZObs = 400, double maxMZObserved = 1500)
        {
            int minCharge = 1;
            int maxCharge = 100;

            UpdateTargetMissingInfo(iqTarget);

            List<IqTarget> targetList = new List<IqTarget>();

            for (int charge = minCharge; charge <= maxCharge; charge++)
            {
                double mz = iqTarget.MonoMassTheor / charge + DeconTools.Backend.Globals.PROTON_MASS;

                if (mz < maxMZObserved)
                {

                    if (mz < minMZObs)
                    {
                        break;
                    }

                    IqTarget chargeStateTarget = new IqChargeStateTarget();


                    CopyTargetProperties(iqTarget, chargeStateTarget);

                    //Note - make sure this step is done after the 'CopyTargetProperties'
                    chargeStateTarget.ChargeState = charge;


                    //adjust isotope profile to reflect new charge state
                    if (chargeStateTarget.TheorIsotopicProfile != null && iqTarget.TheorIsotopicProfile != null)
                    {
                        chargeStateTarget.TheorIsotopicProfile = AdjustIsotopicProfileMassesFromChargeState(chargeStateTarget.TheorIsotopicProfile, iqTarget.TheorIsotopicProfile.ChargeState, charge);
                    }

                    chargeStateTarget.MZTheor = chargeStateTarget.MonoMassTheor / chargeStateTarget.ChargeState +
                                                DeconTools.Backend.Globals.PROTON_MASS;

                    targetList.Add(chargeStateTarget);
                }
            }

            return targetList;
        }

        public void CopyTargetProperties(IqTarget sourceTarget, IqTarget targetForUpdate)
        {
            targetForUpdate.ID = sourceTarget.ID;
            targetForUpdate.EmpiricalFormula = sourceTarget.EmpiricalFormula;
            targetForUpdate.Code = sourceTarget.Code;
            targetForUpdate.MonoMassTheor = sourceTarget.MonoMassTheor;
            targetForUpdate.ChargeState = sourceTarget.ChargeState;
            targetForUpdate.MZTheor = sourceTarget.MZTheor;
            targetForUpdate.ElutionTimeTheor = sourceTarget.ElutionTimeTheor;

            targetForUpdate.TheorIsotopicProfile = sourceTarget.TheorIsotopicProfile == null
                             ? null
                             : sourceTarget.TheorIsotopicProfile.CloneIsotopicProfile();

            targetForUpdate.Workflow = sourceTarget.Workflow;


        }

        private IsotopicProfile AdjustIsotopicProfileMassesFromChargeState(IsotopicProfile iso, int existingCharge, int chargeNew)
        {
            if (iso != null)
            {
                //step 1, scale origional iso from mz to mono incease the input target is allready charged
                double massProton = DeconTools.Backend.Globals.PROTON_MASS;

                if (existingCharge > 0)
                {
                    foreach (MSPeak peak in iso.Peaklist)
                    {
                        peak.XValue = (peak.XValue * existingCharge) - massProton * existingCharge; //gives us a monoisotopic mass
                    }
                }

                //step 2, scale to mono back to mz using new charge 
                iso.ChargeState = chargeNew;
                foreach (MSPeak peak in iso.Peaklist)
                {
                    peak.XValue = (peak.XValue + chargeNew * massProton) / chargeNew;//gives us m/z
                }

                return iso;
            }
            return null;
        }

        public virtual void UpdateTargetMissingInfo(IqTarget target, bool calcAveragineForMissingEmpiricalFormula = true)
        {
            bool isMissingMonoMass = target.MonoMassTheor <= 0;

            if (String.IsNullOrEmpty(target.EmpiricalFormula))
            {
                if (!String.IsNullOrEmpty(target.Code))
                {
                    //Create empirical formula based on code. Assume it is an unmodified peptide
                    //target.EmpiricalFormula = _peptideUtils.GetEmpiricalFormulaForPeptideSequence(target.Code);
                }
                else
                {
                    if (isMissingMonoMass)
                    {
                        throw new ApplicationException(
                            "Trying to fill in missing data on target, but Target is missing both the 'Code' and the Monoisotopic Mass. One or the other is needed.");
                    }
                    target.Code = "AVERAGINE";
                    //target.EmpiricalFormula = IsotopicDistributionCalculator.GetAveragineFormulaAsString(target.MonoMassTheor);
                }
            }

            if (isMissingMonoMass)
            {
                target.MonoMassTheor =
                    EmpiricalFormulaUtilities.GetMonoisotopicMassFromEmpiricalFormula(target.EmpiricalFormula);

                if (target.ChargeState != 0)
                {
                    target.MZTheor = target.MonoMassTheor / target.ChargeState + DeconTools.Backend.Globals.PROTON_MASS;
                }
            }
        }

        #endregion

        protected TargetedWorkflowParameters IqWorkflowParameters { get; set; }

        protected virtual void ExportResults(IqResult iqResult)
        {
            List<IqResult> resultsForExport = _iqResultUtilities.FlattenOutResultTree(iqResult);

            var orderedResults = resultsForExport.OrderBy(p => p.Target.ChargeState).ToList();


            if (ResultExporter == null)
            {
                ResultExporter = iqResult.Target.Workflow.CreateExporter();
            }


            ResultExporter.WriteOutResults(Parameters.ResultsFolder + Path.DirectorySeparatorChar + Run.DatasetName + "_iqResults.txt", orderedResults);
        }



        private string CreatePeaksForChromSourceData()
        {
            var parameters = new PeakDetectAndExportWorkflowParameters();

            parameters.PeakBR = Parameters.ChromGenSourceDataPeakBR;
            parameters.PeakFitType = DeconTools.Backend.Globals.PeakFitType.QUADRATIC;
            parameters.SigNoiseThreshold = Parameters.ChromGenSourceDataSigNoise;
            parameters.ProcessMSMS = Parameters.ChromGenSourceDataProcessMsMs;
            parameters.IsDataThresholded = Parameters.ChromGenSourceDataIsThresholded;

            var peakCreator = new PeakDetectAndExportWorkflow(this.Run, parameters, _backgroundWorker);
            peakCreator.Execute();

            var peaksFilename = this.Run.DataSetPath + "\\" + this.Run.DatasetName + "_peaks.txt";
            return peaksFilename;

        }


        private string GetPossiblePeaksFile()
        {
            string baseFileName;
            baseFileName = this.Run.DataSetPath + "\\" + this.Run.DatasetName;

            string possibleFilename1 = baseFileName + "_peaks.txt";

            if (File.Exists(possibleFilename1))
            {
                return possibleFilename1;
            }
            else
            {
                return string.Empty;
            }
        }



        private void LoadChromData(Run run)
        {
            if (string.IsNullOrEmpty(ChromSourceDataFilePath))
            {
                ChromSourceDataFilePath = GetPossiblePeaksFile();
            }

            if (string.IsNullOrEmpty(ChromSourceDataFilePath))
            {
                //ReportGeneralProgress("Creating _Peaks.txt file for extracted ion chromatogram (XIC) source data ... takes 1-5 minutes");
                ChromSourceDataFilePath = CreatePeaksForChromSourceData();

            }
            else
            {
                //ReportGeneralProgress("Using existing _Peaks.txt file");
            }

            //ReportGeneralProgress("Peak loading started...");

            PeakImporterFromText peakImporter = new PeakImporterFromText(ChromSourceDataFilePath, _backgroundWorker);
            peakImporter.ImportPeaks(this.Run.ResultCollection.MSPeakResultList);


        }

        protected bool ChromDataIsLoaded
        {
            get
            {
                if (Run != null)
                {
                    return Run.ResultCollection.MSPeakResultList.Count > 0;
                }

                return false;
            }
        }

        protected bool RunIsInitialized
        {
            get { throw new NotImplementedException(); }
        }

        protected Run Run
        {
            get;
            set;
        }

        #endregion

        #region Private Methods

        #endregion

    }
}
