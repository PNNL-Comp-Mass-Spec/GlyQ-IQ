using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using System.Data;
using PNNLOmics.Data.Constants.ConstantsDataUtilities;

namespace ConsoleApplication1
{
    public class GlycoPeptideController
    {
        
        
        //public void runGlycoPeptideFinder(string siteType, string BoundingMethod, string FASTA, List<XYData> glycoPeptideData, List<XYData> glycanLibrary)
        public void runGlycoPeptideFinder(GlycoPeptideParameters glycoPeptideParameter, GlycoPeptideResults glycopeptideSitesResults, string glycoPeptideFileLocation)
        {
            double AdductToFinalizeEndsOfPeptide = ElementStaticLibrary.GetMonoisotopicMass("H");
            double AdductToConvertAldehydeToLinkedGlycan = ElementStaticLibrary.GetMonoisotopicMass("O") + ElementStaticLibrary.GetMonoisotopicMass("H");

            GlycoPeptideFindSites glyco = new GlycoPeptideFindSites();
            List<int> siteLocations = new List<int>();
            int numberOfSites = glyco.findGlycosylationSites(glycoPeptideParameter.FASTA, glycoPeptideParameter.SiteType, siteLocations);

            GlycoPeptidePeptidePortion peptideHalf = new GlycoPeptidePeptidePortion();

            //find max value  //perhaps glycoPeptideData[glycoPeptideData.Count].X will work if sorted
            glycoPeptideParameter.MaxMass = glycoPeptideParameter.glycoPeptideMasses[0].X;//largest mass in the data set
            for (int i = 1; i < glycoPeptideParameter.glycoPeptideMasses.Count; i++)
            {
                if (glycoPeptideParameter.glycoPeptideMasses[i].X > glycoPeptideParameter.MaxMass)
                {
                    glycoPeptideParameter.MaxMass = glycoPeptideParameter.glycoPeptideMasses[i].X;
                }
            }
            glycoPeptideParameter.MaxMass = glycoPeptideParameter.MaxMass;
            
            peptidePosibilitiesPerSite peptideStorage = new peptidePosibilitiesPerSite();

            for (int i = 0; i < numberOfSites; i++)
            {
                Console.WriteLine("Site#{0}", i);
                Console.Write("the largest mass is {0} but we will limit it by a factor of {1}\r", glycoPeptideParameter.MaxMass, glycoPeptideParameter.DivisorValue);
                glycoPeptideParameter.MaxMass = glycoPeptideParameter.MaxMass / glycoPeptideParameter.DivisorValue;  ///makes the peptide part shorter for fater testing


                #region Calculate peptide part.  List of amino acid pieces that could exist
                    if (glycoPeptideParameter.SiteType.Equals("ST", StringComparison.OrdinalIgnoreCase))  //for combined sites
                    {
			            Console.WriteLine("O-Linked");
                        //Print "the problem is here in that the T will not find T sites"

                        switch (glycoPeptideParameter.FASTA[siteLocations[i]])
					    {
						    case 'T':
						    {
                                peptideHalf.CalculatePeptidePortion("T", siteLocations[i], peptideStorage, glycoPeptideParameter);
						    }
						    break;
						    case 'S':
						    {
                                peptideHalf.CalculatePeptidePortion("S", siteLocations[i], peptideStorage, glycoPeptideParameter);
						    }
						    break;
						    default:
						    {
							    Console.WriteLine("Missed Site");
						    }
						    break;
					    }
                    }
		            else //Normal for a single site type
                    {
                        Console.WriteLine("N-Linked or S or T processing with {0} siteType, MaxMass {1} and BoundingMethod: {2}\r", glycoPeptideParameter.SiteType, glycoPeptideParameter.MaxMass, glycoPeptideParameter.BoundingMethod);
                        peptideHalf.CalculatePeptidePortion(glycoPeptideParameter.SiteType, siteLocations[i], peptideStorage, glycoPeptideParameter);//<-------------------------------------send index of site//Data is stored in PeptidePartMass, PeptidepartText
                    }

                #endregion

                #region Find residules.  Modify masses so that we can use the glycan aldehyde neutral libraries.  Charge adducts are not included yet

                //modify masses so it is consistant with peptides and glycans
                    IAddAdduct addAdduct = new GlycoPeptideAddAdduct();
                    addAdduct.ToPeptide(i, AdductToFinalizeEndsOfPeptide, peptideStorage);

                    //double CoreModMass = AdductToFinalizeEndsOfPeptide + AdductToConvertAldehydeToLinkedGlycan - ElementsConstantTable.GetExactMass("H");
                    double buffer = 0.1;//a larger list is better than a smaller list.  this is here because doubles will not add properly
                    double CoreModMass = AdductToConvertAldehydeToLinkedGlycan + buffer;//the buffer is add on to provide a few extra in the case of round off error for doubles
                    
                    addAdduct.ToCoreRemove(CoreModMass, glycoPeptideParameter);//used to filter the glycan residules to include a core

                    GlycoPeptideGlycanResidules.FindGlycanResidules(i, glycoPeptideParameter.glycoPeptideMasses, peptideStorage, glycoPeptideParameter);

                    addAdduct.ToCoreAdd(CoreModMass, glycoPeptideParameter);

                    addAdduct.ToGlycanResidules(i, AdductToConvertAldehydeToLinkedGlycan, peptideStorage);

                #endregion

                #region Convert data from XYData to decimals so we can sort and compare
                    //convert lists to decimals for converter
                
                    IConvert letsConvert = new Converter();
                    List<Double> DoubleGlycanLibrary = new List<double>();//destination

                    letsConvert.XYDataToMass(glycoPeptideParameter.glycanLibraryMasses, DoubleGlycanLibrary); 
                
                    List<decimal> LibraryMassesDecimal = DoubleGlycanLibrary.ConvertAll<decimal>(delegate(double str)
                    {
                        return (decimal)str;
                    });

                    List<double> dataGlycoPeptideResidualMassesDouble = peptideStorage.glycanResidulesMass[i];
                    List<decimal> glycoPeptideResidualMassesDecimal = dataGlycoPeptideResidualMassesDouble.ConvertAll<decimal>(delegate(double str)
                    {
                        return (decimal)str;
                    });

                #endregion

                #region Copy and sort the glycan residules.  Then compare/contrast them with the glycan library.  Then remap the indexes to the unsorted data to maintain the structure
                    IRapidCompare compareResidulesToGlycan = new CompareContrast2();
                    CompareResults compareResults = new CompareResults();
                
                    //sort stage    
                    //create new library index so we can sort and unsort the results
                    List<int> indexListForGlycoPeptideSort = new List<int>();
                    for (int j = 0; j < glycoPeptideResidualMassesDecimal.Count; j++)
                    {
                        indexListForGlycoPeptideSort.Add(j);
                    }
                    //send copy to sort so we don't need to remap the masses after the sort
                    List<decimal> glycoPeptideResidulesDecimalCopy= new List<decimal>(glycoPeptideResidualMassesDecimal);
                    
                    IQuickSortIndex sortPriorToCompare = new QuickSort();
                    sortPriorToCompare.SortDecimal(ref glycoPeptideResidulesDecimalCopy, ref indexListForGlycoPeptideSort);

                    compareResidulesToGlycan.CompareContrast(glycoPeptideResidulesDecimalCopy, LibraryMassesDecimal, compareResults, glycoPeptideParameter.Tollerance);

                    //remap indexes after sorted compare
                    List<int> holdIndexListMatchB = new List<int>();
                    for(int j=0;j<compareResults.IndexListBMatch.Count;j++)
                    {
                        holdIndexListMatchB.Add(indexListForGlycoPeptideSort[compareResults.IndexListBMatch[j]]);
                    }

                    List<int> holdIndexListMatchBnotA = new List<int>();
                    for (int j = 0; j < compareResults.IndexListBandNotA.Count; j++)
                    {
                        holdIndexListMatchBnotA.Add(indexListForGlycoPeptideSort[compareResults.IndexListBandNotA[j]]);
                    }

                    //copy back
                    for (int j = 0; j < compareResults.IndexListBMatch.Count; j++)
                    {
                        compareResults.IndexListBMatch[j]=holdIndexListMatchB[j];
                    }

                    for (int j = 0; j < compareResults.IndexListBandNotA.Count; j++)
                    {
                        compareResults.IndexListBandNotA[j] = holdIndexListMatchBnotA[j];
                    }
                    //end remaped sorted indexes
            #endregion

                #region Bring Data Together to set up the results file
                    
                    List<string> PeptideSequenceHits = new List<string>();
                    List<double> PeptideMassHits = new List<double>();
                    List<double> glycoMassesExactHits = new List<double>();//hits of residules translated back to intact masses
                    for (int j = 0; j < compareResults.IndexListAMatch.Count; j++)
                    {
                        glycoMassesExactHits.Add(DoubleGlycanLibrary[compareResults.IndexListAMatch[j]]);
                        PeptideSequenceHits.Add(peptideStorage.glycanResidulesPeptides[i][compareResults.IndexListBMatch[j]]);
                        PeptideMassHits.Add(peptideStorage.glycanResidulesPeptideMass[i][compareResults.IndexListBMatch[j]]);
                    }    
                
                    List<int> glycoPeptideHitsIndex = new List<int>();//hits of residules translated back to intact index
                    List<double> glycoPeptideHitsMass = new List<double>();//hits of residules translated back to intact masses    
                    List<int> SiteList = new List<int>();//hits of residules translated back to intact masses
                    List<decimal> glycoResidulesMassesHits = new List<decimal>();//hits of residules translated back to intact masses
                    
                    for (int j = 0; j < compareResults.IndexListBMatch.Count; j++)
                    {
                        SiteList.Add(siteLocations[i]);
                        glycoResidulesMassesHits.Add(glycoPeptideResidualMassesDecimal[compareResults.IndexListBMatch[j]]);
                        glycoPeptideHitsIndex.Add(peptideStorage.glycanResidulesGlycoPeptideIndex[i][compareResults.IndexListBMatch[j]]);
                        glycoPeptideHitsMass.Add(peptideStorage.glycanResidulesGlycoPeptideMass[i][compareResults.IndexListBMatch[j]]);
                    }

                    //deal with glycopetpdie hits and misses.  check this on larger data
                    List<int> glycoPeptideMissIndex = new List<int>();//misses of residules translated back to intact index
                    List<XYData> glycoPeptidesMisses = new List<XYData>();
                    
                    //we need a sorted version of the index so that we can iterate through it looking for hits and missies
                    List<int> tempGlycoPeptideHitIndex = new List<int>(glycoPeptideHitsIndex);
                    //int[] intArray = tempGlycoPeptideHitIndex.ToArray<int>();
                    tempGlycoPeptideHitIndex.Sort();
                
                    //find out which ones were not assigned and save as misses
                    int assigned = 0;
                    for(int j=0;j<glycoPeptideParameter.glycoPeptideMasses.Count;j++)
                    {
                        if (tempGlycoPeptideHitIndex[assigned] == j)
                        {
                            if (assigned < tempGlycoPeptideHitIndex.Count - 1)//this will assign the last one and then not match any more
                            {
                                assigned++;
                            }
                        }
                        else
                        {
                            XYData newPoint = new XYData();
                            newPoint.X = glycoPeptideParameter.glycoPeptideMasses[j].X;
                            newPoint.Y = glycoPeptideParameter.glycoPeptideMasses[j].Y;
                            glycoPeptidesMisses.Add(newPoint);
                            glycoPeptideMissIndex.Add(j);
                        }
                    }
                    
                    //hits
                    List<XYData> glycoPeptidesHits = new List<XYData>();
                    for (int j = 0; j < glycoPeptideHitsIndex.Count; j++)
                    {
                        XYData newPoint = new XYData();
                        newPoint.X = glycoPeptideParameter.glycoPeptideMasses[glycoPeptideHitsIndex[j]].X;
                        newPoint.Y = glycoPeptideParameter.glycoPeptideMasses[glycoPeptideHitsIndex[j]].Y;
                        glycoPeptidesHits.Add(newPoint);
                    }

                    //final load to results file
                    glycopeptideSitesResults.FASTA = glycoPeptideParameter.FASTA;//sequence
                    glycopeptideSitesResults.Tollerance = glycoPeptideParameter.Tollerance;//mass tollerance
                
                    glycopeptideSitesResults.glycanLibraryHitsIndex = compareResults.IndexListAMatch;//index in library that will map back to a glycan structure
                    glycopeptideSitesResults.glycanResidulesHitsMass = glycoResidulesMassesHits;//mass that hit in library
                    glycopeptideSitesResults.glycanLibraryHitsExactMass = glycoMassesExactHits;///exact glycan mass

                    //glycopeptides hits and misses
                    glycopeptideSitesResults.glycoPeptideMassesXYCopy = glycoPeptideParameter.glycoPeptideMasses;

                    glycopeptideSitesResults.glycoPeptideMassesHitsXY = glycoPeptidesHits;
                    glycopeptideSitesResults.glycoPeptideHitIndex = glycoPeptideHitsIndex;
                    glycopeptideSitesResults.glycoPeptideMassesMissesXY = glycoPeptidesMisses;
                    glycopeptideSitesResults.glycoPeptideMissIndex = glycoPeptideMissIndex;

                    //peptides
                    glycopeptideSitesResults.PeptideSitesListHits = PeptideSequenceHits;
                    glycopeptideSitesResults.PeptideSitesMassHits = PeptideMassHits;
                    glycopeptideSitesResults.PeptideSiteLocation = SiteList;
                #endregion

                    GlycoPeptideResultsToTable ResultsFormat = new GlycoPeptideResultsToTable();

                    DataSet GPDataSet = new DataSet();


                    DataTable GPtableHits;
                    DataTable GPtableMisses;
                    DataTable GPtableOther;

                    ResultsFormat.ConvertToTable(glycopeptideSitesResults, out GPtableHits, out GPtableMisses, out GPtableOther);

                    GPDataSet.Tables.Add(GPtableHits);
                    GPDataSet.Tables.Add(GPtableMisses);
                    GPDataSet.Tables.Add(GPtableOther);

                    GPtableHits.TableName = ("GlycoPeptideHits");
                    GPtableMisses.TableName = ("GlycoPeptideMisses");
                    GPtableOther.TableName = ("GlycoPeptideOther");
                    
                    try
                    {


                        GPDataSet.WriteXml(glycoPeptideFileLocation, XmlWriteMode.WriteSchema);
                        //GPtable.WriteXml(glycoPeptideFileLocation, XmlWriteMode.WriteSchema);
                        
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + "\n" + @"No source path included" + "\n");
                        return;
                    }
                 
                    
                    i = i * 1;

            }
        }//end
    }
}
