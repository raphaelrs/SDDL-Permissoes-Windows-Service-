using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace PermissaoWindowsService
{
    public class Program
    {
        private static string SDDLString = "(A;;RPWPDTLO;;;BU)";
        private static string CurrentSDDLString = string.Empty;
        private static string command1 = "/c sc sdshow CalculadoraGest";
        private static string command2 = "/c sc sdset CalculadoraGest";

        static void Main(string[] args)
        {        
            //the code below captures the SID from the User who ran this program
            //Get this SID number and chanve the SDDLString like this = "(A;;RPWPDTLO;;;Insert_Here_The_SID_Number)";
            //WindowsIdentity user = WindowsIdentity.GetCurrent();
            //SecurityIdentifier sid = user.User;

            //Here you get the current SDDL string
            CurrentSDDLString = SDDLManager(command1);

            //Here you just create the full command "/c sc sdset CalculadoraGest" + SDDL string
            CreatingSDDLString();

            //Here you just execute the command you've created above
            SDDLManager(command2);

            //If you want to see the cmd texts uncomment the line below
            //Console.ReadKey();
        }

        //Executes cmd operations
        public static string SDDLManager(string command)
        {
            string output = string.Empty;
            string error = string.Empty;

            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd", command);
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
            processStartInfo.UseShellExecute = false;

            Process process = Process.Start(processStartInfo);
            using (StreamReader streamReader = process.StandardOutput)
            {
                output = streamReader.ReadToEnd();
            }

            using (StreamReader streamReader = process.StandardError)
            {
                error = streamReader.ReadToEnd();
            }

            Console.WriteLine("The following output was detected:");
            Console.WriteLine(output);

            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine("The following error was detected:");
                Console.WriteLine(error);
            }

            return output;
        }

        public static void CreatingSDDLString()
        {
            int index = 0;
            string separator = "S:";
            string aux = string.Empty;

            index = CurrentSDDLString.IndexOf(separator, 0);

            if (index > 0)
            {
                aux = CurrentSDDLString.Substring(0, index);

                SDDLString = aux + SDDLString + CurrentSDDLString.Substring(index, CurrentSDDLString.Length - index);

                command2 = command2 + " " + SDDLString;

                command2 = command2.Replace("\n", string.Empty).Replace("\t", string.Empty).Replace("\r", string.Empty);
            }
            else
            {
                command2 = (command2 + " " + CurrentSDDLString + SDDLString).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace("\r", string.Empty);
            }
        }
    }    
}
