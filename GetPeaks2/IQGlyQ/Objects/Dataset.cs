using System.Collections.Generic;
using IQGlyQ.FIFO;

namespace IQGlyQ.Objects
{
    public class Dataset
    {
        public string DataSetName { get; set; }

        public string headerToResults { get; set; }

        public List<GlyQIqResult> results { get; set; }
    }
}
