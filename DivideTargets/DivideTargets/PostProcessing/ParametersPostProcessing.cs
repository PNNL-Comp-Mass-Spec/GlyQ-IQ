using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PostProcessing
{
    public class ParametersPostProcessing
    {
        public string LocksFolderPath { get; set; }

        public string LocksFolderName { get; set; }

        public string PathToLocksController { get; set; }

        public string launchFolderPath { get; set; }

        public ParametersPostProcessing()
        {
            
        }

        public void ArgsToParameters (string[] args)
        {
            if(args.Length==3)
            {
                LocksFolderPath = args[0];
                LocksFolderName = args[1];
                launchFolderPath = args[2];

                PathToLocksController = LocksFolderPath + @"\" + LocksFolderName;
            }


        }
    }
}
