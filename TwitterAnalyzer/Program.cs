using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ini.Net;
using System.IO;
using Nito.AsyncEx;


namespace TwitterAnalyzer
{
    class Program
    {
        private static string termFreq = string.Empty;
        private static string resultFilteredCsv, resultSampleCsv = string.Empty;
        private static string confData = string.Empty;
        static void Main(string[] args)
        {
            Console.CancelKeyPress += ((sender, e) =>
            {
                TwitterStreamControl.StopAllStreams();
                Console.WriteLine("Stopped by user.");

            });

            for (int i = 0;i < args.Length;i++)
            {
                if(args[i].Contains("--settings"))
                {
                    confData = args[i + 1];
                }
                else if(args[i].Contains("--termfreq"))
                {
                    termFreq = args[i + 1];
                }
                else if(args[i].Contains("--resultfiltered") || args.Contains("-rf"))
                {
                    resultFilteredCsv = args[i + 1];
                }
                else if (args[i].Contains("--resultsample") || args.Contains("-rs"))
                {
                    resultSampleCsv = args[i + 1];
                }

                else if(args[i].Contains("--help") || args.Contains("-?") || args.Contains("-h"))
                {
                    Console.WriteLine("Twitter Analyzer");
                    Console.WriteLine("By Jackson Ming Hu @ RMIT University, Australia");
                    Console.WriteLine();
                    Console.WriteLine("Usage:");
                    Console.WriteLine("ta.exe  --settings PATH_TO_CONFIG --termfreq PATH_TO_TERMFREQ_CONFIG --resultfiltered PATH_TO_FILTERED_CSV --resultsample PATH_TO_SAMPLED_RESULT_CSV");
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

                Task.Run(async () =>
                {
                    await TwitterStreamControl.StartSampleStream(termFreq, resultSampleCsv);
                }
                );

                Task.Run(async () =>
                {
                    await TwitterStreamControl.StartFilteredStream(termFreq, resultFilteredCsv);
                }
                );

                Console.ReadLine();

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






        }

    }
}
