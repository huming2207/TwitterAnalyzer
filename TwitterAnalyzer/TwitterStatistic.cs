using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterAnalyzer
{
    public class TwitterStatistic
    {
        public static void GetStatisticOpt(string tfSettingPath, string resultPath, string tweetCount)
        {
            Console.WriteLine();
            Console.WriteLine("Counting statistic result, it may takes a while...");
            StreamWriter statWriter = new StreamWriter(resultPath + ".statresult.csv");
            statWriter.WriteLine("\r\n\r\n\r\n" + "********S.T.A.T.I.S.T.I.C********\r\n");
            statWriter.WriteLine("Total Amount," + tweetCount);
            for (int i = 0; i < File.ReadLines(tfSettingPath).Count(); i++)
            {
                string matchedWord = File.ReadLines(tfSettingPath).Skip(i).Take(1).First();
                statWriter.WriteLine(matchedWord + ", Found " + countWordsInFile(matchedWord, resultPath) + " ");
                statWriter.Flush(); // Sometimes it won't save the data to disk, so let it do more than once.
            }
            statWriter.Flush();
            statWriter.Dispose();
        }

        private static int countWordsInFile(string searchTerm, string fileName)
        {
            // Originally comes from: https://msdn.microsoft.com/en-us/library/bb546166.aspx

            string text = File.ReadAllText(fileName);
            string[] source = text.Split(new char[] { '.', '?', '!', ' ', ';', ':', ',' }, StringSplitOptions.RemoveEmptyEntries);

            var matchQuery = from word in source
                             where word.ToLowerInvariant() == searchTerm.ToLowerInvariant()
                             select word;

            return matchQuery.Count();
        }

    }
}
