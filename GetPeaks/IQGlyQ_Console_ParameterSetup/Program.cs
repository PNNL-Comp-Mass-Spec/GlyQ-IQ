using IQGlyQ;
using IQGlyQ.UnitTesting;
using System;

namespace IQGlyQ_Console_ParameterSetup
{
    //argsPIC  "L_10_IQ_TargetsFirstAll.txt" "FragmentedTargetedWorkflowParameters_Velos_DH.txt" "Parameters_DivideTargetsPIC.txt" "GlyQIQ_Diabetes_Parameters_PIC.txt" "Factors_L10.txt" 
    //old args "L_10_IQ_TargetsFirstAll_R.txt" "FragmentedTargetedWorkflowParameters_Velos_DH.txt" "Parameters_DivideTargetsPIC.txt" "GlyQIQ_Diabetes_Parameters_PIC.txt" "Factors_L10.txt" "F:\ScottK\ToPic"
    //new args "F:\ScottK\ToPIC\WorkingParameters\ApplicationSetupParameters.txt"
    class Program
    {
        static void Main(string[] args)
        {
            //string targets = args[0];
            //string processingParameters = args[1];
            //string divideTargetsParameters = args[2];
            //string specificParameters = args[3];
            //string factors = args[4];
            //string writeFolder = args[5];
            

            Console.WriteLine("Running Parameter Setup via GetPeaks");

            IQGlyQSetupParameters parameters = new IQGlyQSetupParameters();
            parameters.SetParameters(args[0]);

            string targets = parameters.Targets;
            string processingParameters = parameters.ProcessingParameters;
            string divideTargetsParameters = parameters.DivideTargetsParameters;
            string dividetargetsFolder = parameters.DivideTargetsParametersFolder;
            string specificParameters = parameters.DataSpecificParameters;
            string factors = parameters.FactorsForTargets;
            string masterLocation = parameters.MasterLocation;
            string workerLocation = parameters.WorkerLocation;
            //string masterComputerName = "Pub-5000";
            string masterComputerName = parameters.MasterComputerName;
            
            SetupMasterAndWorker setupParameters = new SetupMasterAndWorker();
            setupParameters.GeneralSetupRunFirst(targets, processingParameters, divideTargetsParameters, dividetargetsFolder, specificParameters, factors, masterLocation, workerLocation, masterComputerName);

            //IQ_UnitTests tester = new IQ_UnitTests();
            //tester.writeParameterFilesForPic(targets, processingParameters, divideTargetsParameters, dividetargetsFolder, specificParameters, factors, masterLocation, workerLocation);

            FragmentIQTarget newTarget = new FragmentIQTarget();
            Console.WriteLine("Done");
            //Console.ReadKey();
        }
    }
}
