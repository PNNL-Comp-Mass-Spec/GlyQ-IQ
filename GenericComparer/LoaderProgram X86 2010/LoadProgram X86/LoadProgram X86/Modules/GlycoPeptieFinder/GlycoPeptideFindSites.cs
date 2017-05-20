using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class GlycoPeptideFindSites
    {
        public int findGlycosylationSites(string FASTAstr, string SiteType, List<int> GlycanSites)
        {
            Console.WriteLine("-Count Sites ");

	        string SearchFor="X";  //letter to search for
	        string TempLetter;
	        int LengthSequence;
	        int SiteCount=0;
	        int TempSite;
	
	        switch(SiteType)
            {
                case "N":
                    SearchFor="N";  //N-linked
		        break;
		        case  "ST":
			        SearchFor="ST";  //O-Linkned
		        break;
		        case  "S":
			        SearchFor="S";  //S only
		        break;
		        case  "T":
			        SearchFor="T";  //T only
		        break;
                default:
                    Console.WriteLine("Missing 'SiteType', GlycoPeptideFindSites");
                break;
	        }

	        switch (SearchFor)
	        {	
                case "N":
			        Console.WriteLine("   Look for N-Linked N/S or N/T");
                    LengthSequence=FASTAstr.Length;
		            SiteCount=0;
			        TempSite=0;
			        do
                    {
            	        TempSite=FASTAstr.IndexOf('N', TempSite);//provides the index of a possible N site
				
				        //check for N at end of sequence and S and T being +2 off	
					        if(TempSite+2<LengthSequence) //check for end of sequence
                            {
                                TempLetter = FASTAstr.ElementAt(TempSite+2).ToString();
                                //TempLetter=FASTAstr[TempSite+2];
						        //printf "test >%s<\r", TempLetter
						        if(TempLetter.Equals
                                    ("T", StringComparison.OrdinalIgnoreCase) || TempLetter.Equals("S", StringComparison.OrdinalIgnoreCase))
                                //if (cmpstr("T", TempLetter)==0 || cmpstr("S", TempLetter)==0)  //check for N?T or N?S
                                { 
                                    Console.WriteLine("   Add Site N "+ TempSite + " linked with a " + TempLetter);
                                    
                                    GlycanSites.Add(TempSite);
                                    SiteCount+=1;
						        }
                                else
						        {
                                    Console.WriteLine("   N "+ TempSite + " exists but is missing the S or T");
						        }
					        }
                            else 
                            {
                                Console.WriteLine("   N is located at end of sequence");
                            }
					
				        TempSite+=1;  //increment +1 aminoacid so we can find the next site
			        }
                    while (TempSite>0);//TempSite=-1 when there are no more N in the data
			        //SiteCount-=1  //remove last extra increment
		        break;

		        case "ST":
			        Console.WriteLine("look for O-Linked S or T");
			        SiteCount=0;
			        TempSite=0;
			        do
				    {
                        //TempSite=strsearch(FASTSstr,"S",TempSite)  //provides the index of a site
				        TempSite=FASTAstr.IndexOf('S',TempSite);
                        if (TempSite >= 0)//this ensures it exists
                        {
                            Console.WriteLine("   Add Site S " + TempSite);
                            GlycanSites.Add(TempSite);
                            SiteCount += 1;
                            TempSite += 1;
                        }
				        
			        }
                    while (TempSite>0);
			        
                    //SiteCount-=1;  //remove last extra increment
			        TempSite=0;

			        do
				    {
                        TempSite=FASTAstr.IndexOf('T',TempSite);
                        //TempSite=strsearch(FASTSstr,"T",TempSite)  //provides the index of a site
                        if (TempSite >= 0)//this ensures it exists
                        {
                            Console.WriteLine("   Add Site T " + TempSite);
                            GlycanSites.Add(TempSite);
                            SiteCount += 1;
                            TempSite += 1;
                        }  
			        }
                    while (TempSite>0);
			        
                    //SiteCount-=1;  //remove last extra increment
		        break;

		        case "S":
			        Console.WriteLine("look for S sites");
			        SiteCount=0;
			        TempSite=0;
                    do
                    {
                        //TempSite=strsearch(FASTSstr,"S",TempSite)  //provides the index of a site
                        TempSite = FASTAstr.IndexOf('S', TempSite);
                        if (TempSite >= 0)//this ensures it exists
                        {
                            Console.WriteLine("   Add Site S " + TempSite);
                            GlycanSites.Add(TempSite);
                            SiteCount += 1;
                            TempSite += 1;
                        }
                    }
                    while (TempSite>0);
			        
                    //SiteCount-=1;  //remove last extra increment
		        break;
		        case "T":
			        Console.WriteLine("look for T sites");
			        SiteCount=0;
			        TempSite=0;

                    do
                    {
                        //TempSite=strsearch(FASTSstr,"T",TempSite)  //provides the index of a site
                        TempSite = FASTAstr.IndexOf('T', TempSite);
                        if (TempSite >= 0)//this ensures it exists
                        {
                            Console.WriteLine("   Add Site T " + TempSite);
                            GlycanSites.Add(TempSite);
                            SiteCount += 1;
                            TempSite += 1;
                        }
                    }
                    while (TempSite>0);
			        
                    //SiteCount-=1;  //remove last extra increment
		        break;
                default:
                    Console.WriteLine("Missing 'SearchFor', GlycoPeptideFindSites");
                break;
            }
	    
            Console.WriteLine(FASTAstr);
            Console.WriteLine("There are " + SiteCount + " sites");

	        return SiteCount;
        }
    }
}
