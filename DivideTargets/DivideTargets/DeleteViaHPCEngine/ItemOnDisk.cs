using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeleteViaHPCEngine
{
    public class ItemOnDisk
    {
        public itemType Type { get; set; }
        public string path { get; set; }
    }

    public enum itemType
    {
        File,
        Directory
    }
}
