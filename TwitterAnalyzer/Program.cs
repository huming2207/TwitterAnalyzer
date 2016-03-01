using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ini.Net;
using System.IO;


namespace TwitterAnalyzer
{
    class Program
    {
        private static string termFreq = string.Empty;
        private static string resultCsv = string.Empty;
        private static string confData = string.Empty;
        static void Main(string[] args)
        {
            for(int i =0;i < args.Length;i++)
            {
                if(args[i].Contains("--settings"))
                {
                    confData = args[i + 1];
                }
                else if(args[i].Contains("--termfreq"))
                {
                    termFreq = args[i + 1];
                }
                else if(args[i].Contains("--result"))
                {
                    resultCsv = args[i + 1];
                }
                else if(args[i].Contains("--help") || args.Contains("-?") || args.Contains("-h"))
                {
                    Console.WriteLine("Twitter Analyzer");
                    Console.WriteLine("By Jackson Ming Hu @ RMIT University, Australia");
                    Console.WriteLine();
                    Console.WriteLine("Usage:");
                    Console.WriteLine("ta.exe  --settings PATH_TO_CONFIG --termfreq PATH_TO_TERMFREQ_CONFIG --result PATH_TO_RESULT_CSV");
                    Console.ReadLine();
                }
            }

            if(File.Exists(confData) == false)
            {
                File.CreateText(confData);
            }

            if(File.ReadAllText(confData) != "")
            {
                var iniFile = new IniFile(confData);
                TwitterStreamControl.Login(
                    iniFile.ReadString("UserInfo", "UserKey"),
                    iniFile.ReadString("UserInfo", "UserSecret"),
                    iniFile.ReadString("UserInfo", "AccessToken"),
                    iniFile.ReadString("UserInfo", "TokenKey"));

                TwitterStreamControl.StartStream(termFreq, resultCsv);
            }
            else
            {
                var iniFile = new IniFile(confData);
                iniFile.WriteString("UserInfo", "UserKey","");
                iniFile.WriteString("UserInfo", "UserSecret", "");
                iniFile.WriteString("UserInfo", "AccessToken", "");
                iniFile.WriteString("UserInfo", "TokenKey", "");
                Console.WriteLine("You have an empty setting file. Analysis will not run.");
                
            }



            Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                Console.WriteLine();
                Console.WriteLine("Terminated by user. Thanks for using TwitterAnalyzer!");
            };


        }
    }
}
