using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    [Serializable]
    public abstract class BaseData
    {
        int ID { get; set; }
        public abstract void Clear();
    }
}
