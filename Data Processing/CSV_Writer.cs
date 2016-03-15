using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team_E_FIRE_Data_Processing_Program_BBB
{
    class CSV_Writer
    {
        //string winDir = System.Environment.GetEnvironmentVariable("windir");
        public void WriteStrikeToCSV()
        {
            string s = "Hello";
            //a TextWriter writes characters to a stream; can be used to open as well
            StreamWriter writer = new StreamWriter("C:\\EfireData"); 
            writer.WriteLine("<strike #>, <diode struck>, <electron energy>, <strike time>");
            File.AppendText(s);
            writer.Close();
            //Console.ReadLine();
        }
    }
}
