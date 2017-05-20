using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlycolyzerGUImvvm.Models
{
    public class LibrariesModel : ObservableObject
    {
        private String chosenDefaultLibrary_String = "No Library Selected";
        private String addressDefaultLibrary_String = "";
        private String chosenCustomLibrary_String = "";


        public String ChosenDefaultLibrary_String
        {
            get { return chosenDefaultLibrary_String; }
            set { if (value != chosenDefaultLibrary_String) { chosenDefaultLibrary_String = value; OnPropertyChanged("chosenDefaultLibrary_String"); } }
        }

        public String AddressDefaultLibrary_String
        {
            get { return addressDefaultLibrary_String; }
            set { if (value != addressDefaultLibrary_String) { addressDefaultLibrary_String = value; OnPropertyChanged("addressDefaultLibrary_String"); } }
        }

        public String ChosenCustomLibrary_String
        {
            get { return chosenCustomLibrary_String; }
            set { if (value != chosenCustomLibrary_String) { chosenCustomLibrary_String = value; OnPropertyChanged("chosenCustomLibrary_String"); } }
        }
    }
}
