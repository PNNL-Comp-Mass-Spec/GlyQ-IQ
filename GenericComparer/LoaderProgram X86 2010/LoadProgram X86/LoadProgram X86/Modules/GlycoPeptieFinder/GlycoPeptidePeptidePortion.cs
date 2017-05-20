using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Constants.ConstantsDataUtilities;

namespace ConsoleApplication1
{
    class GlycoPeptidePeptidePortion
    {
        private int m_LeftLength {get; set;}
        private int m_RightLength {get; set;}
        private double m_GlycanCoreMass {get; set;}


        public void CalculatePeptidePortion(string SiteType, int CenterSiteIndex, peptidePosibilitiesPerSite peptideStorage, GlycoPeptideParameters glycoPeptideParameter)
        {
            int k;
           
	        Console.WriteLine("Find Peptide Part Centered At Site {0}\r", CenterSiteIndex);

            string PeptidePartText = "";
            double PeptidePartMass=0;

            double mAsparagine = AminoAcidStaticLibrary.GetMonoisotopicMass('N');//Asn
            double mSerine = AminoAcidStaticLibrary.GetMonoisotopicMass('S');//Ser
            double mThreonine = AminoAcidStaticLibrary.GetMonoisotopicMass('T');//Thr

	        int CoreToggle=1;//is ther a common core
            CoreToggle = 1;//this is not implemented yet

	        //double CoreMass;  //N-linked Core for example
	        double MassSiteAminoAcid = 0;
	        double MassCurrentAminoAcid = 0;
	        int CounterLeft, CounterRight, AssignmentCounter;
	        
	        double MassCalc;  //Overall Mass Count
	        double MassCalcLeft; //for maxing out the left side
			double MassCalcLeftNext = 0;//one extra amino acid to the left
	        double EndCTermialAminoAcid;
	
	        //string FASTSstr;  //complete string
	        char AminoAcidCurrent;
	        string AminoAcidSite = "";
	        string AminoChain;

			List<string> peptideSequences = new List<string>();
			List<double> peptideMasses = new List<double>();

			double test;//used for testing when we reach the end direction to the right
	        switch(SiteType)
            {
		        case "N":
                {
                    AminoAcidSite = "N";
			        MassSiteAminoAcid=mAsparagine;		//Asn, N
		            Console.WriteLine("--SiteType-N");
		        }
                break;
		        case "S":
                {	
                    AminoAcidSite="S";
			        MassSiteAminoAcid=mSerine;			//Ser, S
			        Console.WriteLine("--SiteType-S");
		        }
                break;
		        case "T":
                {
			        AminoAcidSite="T";
			        MassSiteAminoAcid=mThreonine;		//Thr, T
			        Console.WriteLine("--SiteType-T");
		        }
                break;
		        default:
                {
			        Console.WriteLine("PF Peptide Half");
			        Console.WriteLine("Wrong Site Type");
                }
                break;
	        }  //SiteType


            EndCTermialAminoAcid = glycoPeptideParameter.FASTA.Length;
	        //Console.WriteLine("the peptide is {0} amino acids long\r", EndCTermialAminoAcid);
	        CoreToggle=0;  //Subtract Core from MaxMass

	        CounterLeft=0;
	        CounterRight=0;
	        AssignmentCounter=0;

	        //there are two methods for bounding the number of amino acids to include
	        //1.  user input will bound number of amino acids to the left and right
	        //2.  count till you run our of mass.
		        //1.  you need to know the larget mass in the spectrum.
		        //2.  the max peptide size will be largestmass-core-N glycan for example
		        //or largestmass/number

            //add site block
            //peptideSequences.Add(AminoAcidSite);
            //peptideMasses.Add(MassSiteAminoAcid);


            switch (glycoPeptideParameter.BoundingMethod)
            {
                case "MaxMass":
					//in general you start at the site and then progress right.  Then shift left one, and progress right etc.
                    {
                        Console.WriteLine("Bounded by MaxMass");
                        CounterLeft=0;//reset counter
                        CounterRight=0;//reset counter

                        do
                        {
                            MassCalc=MassSiteAminoAcid; //starting point
                            MassCalcLeft=MassSiteAminoAcid;  //add mass to left.  This is used so you know when you run out of amino acids to the left
                            AminoChain=AminoAcidSite;
                            Console.WriteLine("start mass is", MassCalc, AminoChain);

                            Console.WriteLine(CounterLeft +" "+CenterSiteIndex);
                            if (CounterLeft<=CenterSiteIndex)  //This is here to prevent going over the end to the left
                            {
                                for (k=0;k<CounterLeft;k+=1)
                                {
                                    AminoAcidCurrent = glycoPeptideParameter.FASTA[CenterSiteIndex - k - 1];  //+1 since starting amino acid is already added
                                    MassCurrentAminoAcid = AminoAcidStaticLibrary.GetMonoisotopicMass(AminoAcidCurrent);
        						    
									Console.WriteLine("The Current Left amino acid is {0} with a mass of {1}", AminoAcidCurrent, MassCurrentAminoAcid);

                                    AminoChain = AminoAcidCurrent + AminoChain;//reverse order since we are going left
                                    MassCalcLeft+=MassCurrentAminoAcid; 
                                }

                                if (CenterSiteIndex - k - 1 > 0)//end condition
                                {
                                    AminoAcidCurrent = glycoPeptideParameter.FASTA[CenterSiteIndex - k - 1];  //+1 since starting amino acid is already added
                                    MassCalcLeftNext = AminoAcidStaticLibrary.GetMonoisotopicMass(AminoAcidCurrent);
                                }
                                else
                                {
                                    MassCalcLeftNext = 0; 
                                }
                                
								//Assign
                                if (MassCalcLeft < glycoPeptideParameter.MaxMass - CoreToggle * glycoPeptideParameter.CoreMass)
                                {
                                    PeptidePartText = AminoChain + PeptidePartText;//reverse order
                                    PeptidePartMass += MassCalcLeft;
                                    AssignmentCounter+=1;
                                    //new part 8-2-10
                                    peptideSequences.Add(PeptidePartText);
                                    peptideMasses.Add(PeptidePartMass);
                                }
                            }
                        
                            CounterRight=0;
                            
							do
                            {
                                AminoAcidCurrent = glycoPeptideParameter.FASTA[CenterSiteIndex + CounterRight + 1];  //+1 since starting amino acid is already added
                                MassCurrentAminoAcid = AminoAcidStaticLibrary.GetMonoisotopicMass(AminoAcidCurrent);
                                
        					    Console.WriteLine("The Current Right amino acid is {0} with a mass of {1}\r", AminoAcidCurrent, MassCurrentAminoAcid);
                                
								CounterRight+=1;//increment right

                                MassCalc+=MassCurrentAminoAcid;
                                test = PeptidePartMass + MassCurrentAminoAcid;
                                if (test < glycoPeptideParameter.MaxMass - CoreToggle * glycoPeptideParameter.CoreMass)	
                                {
                                    //Assign
                                    PeptidePartText += AminoAcidCurrent;
                                    PeptidePartMass += MassCurrentAminoAcid;
                                    peptideSequences.Add(PeptidePartText);
                                    peptideMasses.Add(PeptidePartMass);
                                    AssignmentCounter += 1;
                                }
                                
                                //Console.WriteLine(AssignmentCounter, EndCTermialAminoAcid, CounterRight, CounterLeft, CounterRight+CounterLeft, CenterSiteIndex-CounterLeft, EndCTermialAminoAcid-(CounterRight+CenterSiteIndex)-1 //-CounterRight
                            //While (MassCalc<MaxMassIn-CoreToggle*CoreMass)
                            }
                            while (PeptidePartMass + MassCurrentAminoAcid < glycoPeptideParameter.MaxMass - CoreToggle * glycoPeptideParameter.CoreMass && EndCTermialAminoAcid - (CounterRight + CenterSiteIndex) - 1 > 0);  //This is to prevent going over the end to the right
                            
                            CounterLeft+=1;  //increment left

            				Console.WriteLine("The final chain is {0} at mass {0}\r", AminoChain, MassCalc);
                            PeptidePartText="";
                            PeptidePartMass = 0;	
                        }
                        while (MassCalcLeft + MassCalcLeftNext < glycoPeptideParameter.MaxMass - CoreToggle * glycoPeptideParameter.CoreMass && CounterLeft <= CenterSiteIndex);  //increment one left and then test all right
                        
                        //Console.WriteLine("last mass is:  " + PeptidePartMass[AssignmentCounter-1]);
                    }//maxmass
                    break;

                case "User":  //same as unbounded with the substitution of for statements for the do-whiles
                    {

                        Console.WriteLine("Bounded by User Imput");
                        CounterLeft = 0;//reset counter
                        CounterRight = 0;//reset counter

                        do
                        {
                            MassCalc = MassSiteAminoAcid; //starting point
                            MassCalcLeft = MassSiteAminoAcid;  //add mass to left.  This is used so you know when you run out of amino acids to the left
                            AminoChain = AminoAcidSite;
                            Console.WriteLine("start mass is", MassCalc, AminoChain);

                            Console.WriteLine(CounterLeft + " " + CenterSiteIndex);
                            if (CounterLeft <= CenterSiteIndex)  //This is here to prevent going over the end to the left
                            {
                                for (k = 0; k < CounterLeft; k += 1)
                                {
                                    AminoAcidCurrent = glycoPeptideParameter.FASTA[CenterSiteIndex - k - 1];  //+1 since starting amino acid is already added
                                    MassCurrentAminoAcid = AminoAcidStaticLibrary.GetMonoisotopicMass(AminoAcidCurrent);

                                    Console.WriteLine("The Current Left amino acid is {0} with a mass of {1}", AminoAcidCurrent, MassCurrentAminoAcid);

                                    AminoChain = AminoAcidCurrent + AminoChain;//reverse order since we are going left
                                    MassCalcLeft += MassCurrentAminoAcid;
                                }

                                if (CenterSiteIndex - k - 1 > 0)//end condition
                                {
                                    AminoAcidCurrent = glycoPeptideParameter.FASTA[CenterSiteIndex - k - 1];  //+1 since starting amino acid is already added
                                    MassCalcLeftNext = AminoAcidStaticLibrary.GetMonoisotopicMass(AminoAcidCurrent);
                                }
                                else
                                {
                                    MassCalcLeftNext = 0;
                                }

                                //Assign
                                if (MassCalcLeft < glycoPeptideParameter.MaxMass - CoreToggle * glycoPeptideParameter.CoreMass)
                                {
                                    PeptidePartText = AminoChain + PeptidePartText;//reverse order
                                    PeptidePartMass += MassCalcLeft;
                                    AssignmentCounter += 1;
                                    //new part 8-2-10
                                    peptideSequences.Add(PeptidePartText);
                                    peptideMasses.Add(PeptidePartMass);
                                }
                            }

                            CounterRight = 0;

                            do
                            {
                                
                                AminoAcidCurrent = glycoPeptideParameter.FASTA[CenterSiteIndex + CounterRight + 1];  //+1 since starting amino acid is already added
                                MassCurrentAminoAcid = AminoAcidStaticLibrary.GetMonoisotopicMass
                                    (AminoAcidCurrent);

                                Console.WriteLine("The Current Right amino acid is {0} with a mass of {1}\r", AminoAcidCurrent, MassCurrentAminoAcid);

                                CounterRight += 1;//increment right

                                MassCalc += MassCurrentAminoAcid;
                                test = PeptidePartMass + MassCurrentAminoAcid;
                                if (test < glycoPeptideParameter.MaxMass - CoreToggle * glycoPeptideParameter.CoreMass)
                                {
                                    //Assign
                                    PeptidePartText += AminoAcidCurrent;
                                    PeptidePartMass += MassCurrentAminoAcid;
                                    peptideSequences.Add(PeptidePartText);
                                    peptideMasses.Add(PeptidePartMass);
                                    AssignmentCounter += 1;
                                }

                                //Console.WriteLine(AssignmentCounter, EndCTermialAminoAcid, CounterRight, CounterLeft, CounterRight+CounterLeft, CenterSiteIndex-CounterLeft, EndCTermialAminoAcid-(CounterRight+CenterSiteIndex)-1 //-CounterRight
                                //While (MassCalc<MaxMassIn-CoreToggle*CoreMass)
                            }
                            while (CounterRight < glycoPeptideParameter.AminoAcidsToRight && PeptidePartMass + MassCurrentAminoAcid < glycoPeptideParameter.MaxMass - CoreToggle * glycoPeptideParameter.CoreMass && EndCTermialAminoAcid - (CounterRight + CenterSiteIndex) - 1 > 0);  //This is to prevent going over the end to the right

                            CounterLeft += 1;  //increment left

                            Console.WriteLine("The final chain is {0} at mass {0}\r", AminoChain, MassCalc);
                            PeptidePartText = "";
                            PeptidePartMass = 0;
                        }
                        while (CounterLeft < glycoPeptideParameter.AminoAcidsToLeft + 1 && MassCalcLeft + MassCalcLeftNext < glycoPeptideParameter.MaxMass - CoreToggle * glycoPeptideParameter.CoreMass && CounterLeft <= CenterSiteIndex);  //increment one left and then test all right
                        
                    }
                    break;
            }// End switch BoundingMethod\

            peptideStorage.SetSiteLocation(CenterSiteIndex);
            peptideStorage.AddPeptidePossibiities(peptideSequences);
            peptideStorage.AddPeptideMassPossibiities(peptideMasses);
			peptideStorage.SetSiteType(SiteType);
        }
    }
}