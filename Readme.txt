== Overview ==

GlyQ-IQ is a program that performs a targeted, chromatographic centric 
search of mass spectral data for glycans.  It is designed for 
high resolution LC-MS datasets.  The software uses a list of glycan 
targets to search for expected features in MS1 spectra. Features are 
characterized by monoisotopic mass, elution time, and isotopic fit score. 
Features are annotated by glycan family relationships and optionally
by in-source fragmentation patterns.

At present, GlyQ-IQ only supports data acquired on 
Thermo instruments (.raw files).

IQGlyQ_Console.exe is the main application.  

Source code for the 64-bit version is located at GetPeaks3\IQGlyQ_Console
though the master solution file is GetPeaks3\GetPeaks3.sln

There is also a 32-bit version, but the code-base may be a bit out of date.
That solution file is at GetPeaks2\GetPeaks2.sln


== Usage ==

The IQGlyQ_Console.exe program requires several input files, including:
- Thermo .Raw file
- Targets file
- Several parameter files

=== Peak Detection ===

The first step is to create the Peaks.txt file using HammerOrDeconSimplePeakDetector.exe
which you can find at bin\DeconPeakDetector

This file contains peak lists for each mass spectrum, i.e. the mass and intensity values 
for each isotopic species above a computed noise threshold.

The program syntax is:

HammerOrDeconSimplePeakDetector.exe Dataset.raw /P:ParameterFile.txt /O:OutputFolderPath


The default parameter file is HPC_SpinExactive_PeakDetector_Parameters.txt
which you can find at bin\ParameterFiles\PeakDetection


=== Parameter File Configuration ===

The next step is to customize the parameter files in the Working Directory 
and the WorkingParameters folder.  There are quite a few folders and files 
to customize; for a model layout, see bin\WorkDir\WorkingParametersCore1
For the example files to work-as is, copy the WorkDir folder to
C:\Temp\WorkDir


Place the following files in folder WorkingParametersCore1

- FragmentedParameters_Velos_7ppm_H.txt or FragmentedParameters_Velos_7ppm_DH.txt
	- Use the DH file if the sample includes a spike-in of 
	  deuterium labeled peptides, which provides a better chance 
	  of identifying the glycans.  These labeled peptides should
	  be similar to your sample of interest, but labeled with deuterium
	- If the sample does not contain labeled peptides, use the _H.txt file
	- Both of these parameter files have a MS1 ion tolerance of +/-7 ppm.  
	  Optionally adjust this tolerance wider if necessary

- Factors_L10.txt

- AlignmentParameters.xml

- Targets file (glycans to search for)
	- Use L_13_HighMannose_TargetsFirstAll.txt for testing
	- Use L_13_Alditol_No_PSA.txt              for production

- Runtime parameters file
	- Suggested name GlyQIQ_Params_DatasetName.txt
	- Example contents:

ResultsFolderPath,C:\Temp\WorkDir\Results
LoggingFolderPath,C:\Temp\WorkDir\Results
FactorsFile,Factors_L10.txt
ExecutorParameterFile,ExecutorParametersSK.xml
XYDataFolder,XYDataWriter
WorkflowParametersFile,FragmentedParameters_Velos_7ppm_DH.txt
Alignment,C:\Temp\WorkDir\WorkingParametersCore1\AlignmentParameters.xml
BasicTargetedParameters,C:\Temp\WorkDir\WorkingParametersCore1\BasicTargetedWorkflowParameters.xml


- ExecutorParameters file
	- The file paths in this file don't matter
	- Thus, just use the default Executor file, ExecutorParametersSK.xml


=== Start IQGlyQ_Console.exe ===

Program syntax / paths (these will all be on one line when you actually run it):

IQGlyQ_Console.exe "%WorkDirPath%"
   "%DatasetName%"
   "raw"
   "L_13_HighMannose_TargetsFirstAll_Part1.txt"
   "GlyQIQ_Params_DatasetName.txt"
   "%WorkDirPath%\WorkingParametersCore1"
   "Lock_1"
   "%WorkDirPath%\Results"
   "1"

It is probably best to include all of the double quotes on these parameters, 
even if they don't have spaces.  For an example command line with full paths, 
see file bin\WorkDir\StartProgram_Core1.bat


== Results File ==

The results file has 85 columns with numerous details on 
putatively identified glycans.  The first thing to do with the 
result file is open with Microsof Excel (or similar) and sort
the data on the FinalDecision column (in Excel this is column AM).
Results with "Future Target" are low quality results and can likely
be ignored.  The important results say "CorrectGlycan".


== Multithreading ==

When searching for a large number of targets, you can split the targets file
into multiple parts then launch multiple copies of IQGlyQ_Console.exe
When doing this, make duplicate copies of the WorkingParametersCore1
folder, for example:
	WorkingParametersCore1
	WorkingParametersCore2
	WorkingParametersCore3

The parameter files in each one will be nearly identical, 
but each one will contain a different section of the targets file.

In addition, update the paths for these two entries in the
Runtime parameters file:

Alignment,C:\Temp\WorkDir\WorkingParametersCore2\AlignmentParameters.xml
BasicTargetedParameters,C:\Temp\WorkDir\WorkingParametersCore2\BasicTargetedWorkflowParameters.xml

You then make a series of batch files, with each one pointing 
to a different WorkingParametersCore folder. In addition, update 
the LockFile name and the Core ID.  For example:

C:\GlyQ-IQ\IQGlyQ_Console.exe "C:\Temp\WorkDir" "Gly09_Velos3_Jaguar_230nL30_C14_DB10" "raw" "L_13_Alditol_No_PSA_Part1.txt" "GlyQIQ_Params_Gly09_Velos3_Jaguar_230nL30_C14_DB10.txt" "C:\Temp\WorkDir\WorkingParametersCore1" "Lock_1" "C:\Temp\WorkDir\Results" "1"
C:\GlyQ-IQ\IQGlyQ_Console.exe "C:\Temp\WorkDir" "Gly09_Velos3_Jaguar_230nL30_C14_DB10" "raw" "L_13_Alditol_No_PSA_Part2.txt" "GlyQIQ_Params_Gly09_Velos3_Jaguar_230nL30_C14_DB10.txt" "C:\Temp\WorkDir\WorkingParametersCore2" "Lock_2" "C:\Temp\WorkDir\Results" "2"
C:\GlyQ-IQ\IQGlyQ_Console.exe "C:\Temp\WorkDir" "Gly09_Velos3_Jaguar_230nL30_C14_DB10" "raw" "L_13_Alditol_No_PSA_Part3.txt" "GlyQIQ_Params_Gly09_Velos3_Jaguar_230nL30_C14_DB10.txt" "C:\Temp\WorkDir\WorkingParametersCore3" "Lock_3" "C:\Temp\WorkDir\Results" "3"
C:\GlyQ-IQ\IQGlyQ_Console.exe "C:\Temp\WorkDir" "Gly09_Velos3_Jaguar_230nL30_C14_DB10" "raw" "L_13_Alditol_No_PSA_Part4.txt" "GlyQIQ_Params_Gly09_Velos3_Jaguar_230nL30_C14_DB10.txt" "C:\Temp\WorkDir\WorkingParametersCore4" "Lock_4" "C:\Temp\WorkDir\Results" "4"


-------------------------------------------------------------------------------
Written by Scott Kronewitter for the Department of Energy (PNNL, Richland, WA)
Copyright 2014, Battelle Memorial Institute.  All Rights Reserved.

E-mail: matthew.monroe@pnnl.gov or proteomics@pnnl.gov
Website: http://omics.pnl.gov/  or http://panomics.pnnl.gov/ 
-------------------------------------------------------------------------------

Licensed under the Apache License, Version 2.0; you may not use this file except 
in compliance with the License.  You may obtain a copy of the License at 
http://www.apache.org/licenses/LICENSE-2.0

All publications that utilize this software should provide appropriate 
acknowledgement to PNNL website. However, if the software is extended or modified, 
then any subsequent publications should include a more extensive statement, 
using this text or a similar variant: 
 Portions of this research were supported by the NIH National Center for 
 Research Resources (Grant RR018522), the W.R. Wiley Environmental Molecular 
 Science Laboratory (a national scientific user facility sponsored by the U.S. 
 Department of Energy's Office of Biological and Environmental Research and 
 located at PNNL), and the National Institute of Allergy and Infectious Diseases 
 (NIH/DHHS through interagency agreement Y1-AI-4894-01). PNNL is operated by 
 Battelle Memorial Institute for the U.S. Department of Energy under 
 contract DE-AC05-76RL0 1830. 

Notice: This computer software was prepared by Battelle Memorial Institute, 
hereinafter the Contractor, under Contract No. DE-AC05-76RL0 1830 with the 
Department of Energy (DOE).  All rights in the computer software are reserved 
by DOE on behalf of the United States Government and the Contractor as 
provided in the Contract.  NEITHER THE GOVERNMENT NOR THE CONTRACTOR MAKES ANY 
WARRANTY, EXPRESS OR IMPLIED, OR ASSUMES ANY LIABILITY FOR THE USE OF THIS 
SOFTWARE.  This notice including this sentence must appear on any copies of 
this computer software.

