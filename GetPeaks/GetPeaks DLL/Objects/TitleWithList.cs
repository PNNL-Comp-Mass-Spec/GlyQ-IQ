using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.Objects
{
    public class TitleWithList<T>
    {
        public string Title { get; set; }

        public List<T> Data { get; set; }
 
        public TitleWithList()
        {
            Data = new List<T>();
        }

        public TitleWithList(string title, List<T> data)
        {
            Data = data;
            Title = title;
        }
    }
}
