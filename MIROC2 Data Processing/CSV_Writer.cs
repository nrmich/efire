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
        public CSV_Writer() { }
        //string winDir = System.Environment.GetEnvironmentVariable("windir");
        public void WriteStrikeToCSV(decimal d)
        {
            string s = System.Convert.ToString(d);
            //a TextWriter writes characters to a stream; can be used to open as well
            StreamWriter writer = new StreamWriter("C:\\Users\\Derek\\Documents\\EfireData.txt"); 
            writer.WriteLine("<strike #>, <diode struck>, <electron energy>, <strike time>");
            writer.Close();
            File.AppendAllText(@"C:\\Users\\Derek\\Documents\\EfireData.txt", s);
            Console.ReadLine();
        }
    }
}
