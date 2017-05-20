using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace RegisterDLL
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start DLL Load process...");
            Form1_Load();
            
            
            //string dllToRegister = @"\\picfs\projects\DMS\PIC_HPC\ThermoDLL\InstallFilesToCDrive\XcalDLL\MSFileReader\MSFileReader.XRawfile2.dll";
            //string registerorUnregister = "add";
            
            string dllToRegister = args[0];
            string registerorUnregister = args[1];

            if(args.Length!=2)
            {
                Console.WriteLine("2 args are required.   Dll and add/remove");
                Console.ReadKey();
            }

            registerOrNot addOrRemove = registerOrNot.Register;
            switch(registerorUnregister)
            {
                case "add":
                    addOrRemove = registerOrNot.Register;
                    break;
                case "remove":
                    addOrRemove = registerOrNot.Unregister;
                    break;
                default:
                    Console.WriteLine("need to use (add) or (remove) as second arg");
                    Console.ReadKey();
                    break;
            }

            try
            {
                //’/s’ : Specifies regsvr32 to run silently and to not display any message boxes.
                //string arg_fileinfo = "\\" + dllToRegister + "\\"; // give the path of the dll file here
                string arg_fileinfo = dllToRegister + @" /s";
                switch(addOrRemove)
                {
                    case registerOrNot.Register:
                        {
                            arg_fileinfo = dllToRegister + @" /s";
                        }
                        break;
                        case registerOrNot.Unregister:
                        {
                            arg_fileinfo = "-u " + dllToRegister + @" /s";
                            //arg_fileinfo = "-u " + dllToRegister;
                        }
                        break;
                }
                
                Process reg = new Process();
                //This file registers .dll files as command components in the registry.
                reg.StartInfo.FileName = "regsvr32.exe";
                reg.StartInfo.Arguments = arg_fileinfo;
                reg.StartInfo.UseShellExecute = false;
                reg.StartInfo.CreateNoWindow = true;
                reg.StartInfo.RedirectStandardOutput = true;
                reg.Start();
                reg.WaitForExit();
                reg.Close();
                Console.WriteLine("Success");
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                Console.WriteLine(ex.Message);
            }

        }


        private static void Form1_Load()
        {
            bool isAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent())                    .IsInRole(WindowsBuiltInRole.Administrator) ? true : false;

            if(isAdmin)
            {
                Console.WriteLine("you are an administrator");
            }
            else
            {
                Console.WriteLine("You are not an administrator");
            }
        } 
            //http://www.aneef.net/2009/06/29/request-uac-elevation-for-net-application-managed-code/#sthash.1wzjSuAp.dpuf
    }



    

    internal enum registerOrNot
    {
        Register,
        Unregister,
    }
}
