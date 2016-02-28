using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace TwitterAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
     
            TwitterStreamControl.StartStream(Application.StartupPath + "\term-freq.txt", Application.StartupPath + "\result.csv");
        }
    }
}
