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

        //TODO need destructor to close csv writer
        public CSV_Writer(string targetFile)
        {
            //writer = new StreamWriter(targetFile);
            writer = new StreamWriter(@"C:\Users\Nathaniel\test_hard.csv");
            writer.WriteLine("<strike #>, <diode struck>, <electron energy>, <strike time>");
            //writer.Close();
        }

        public StreamWriter writer;

        public void WriteStrikeToCSV(int strikeNumber, int diodeStruck, decimal electronEnergy, DateTime strikeTime)
        {
            string diodeStruck_string = System.Convert.ToString(diodeStruck);
            string electronEnergy_string = System.Convert.ToString(electronEnergy);
            string strikeTime_string = System.Convert.ToString(strikeTime);
            string strikeNumber_string = strikeNumber.ToString();
            writer.WriteLine(strikeNumber_string + "," + diodeStruck_string + "," + electronEnergy_string + "," + strikeTime_string);
            //writer.Close();
        }

        public void close()
        {
            writer.Close();
        }


       /* public void WriteStrikeToCSV(int diodeStruck, decimal electronEnergy, decimal strikeTime)
        {
            string diodeStruck_string = System.Convert.ToString(diodeStruck);
            string electronEnergy_string = System.Convert.ToString(electronEnergy);
            string strikeTime_string = System.Convert.ToString(strikeTime);
            //a TextWriter writes characters to a stream; can be used to open as well
            //StreamWriter writer = new StreamWriter("C:\\Users\\Derek\\Documents\\EfireData.txt");

            writer.WriteLine("<strike #>, <diode struck>, <electron energy>, <strike time>");
            writer.Close();
            File.AppendAllText(@"C:\\Users\\Derek\\Documents\\EfireData.txt", diodeStruck_string);
            File.AppendAllText(@"C:\\Users\\Derek\\Documents\\EfireData.txt", ", ");
            File.AppendAllText(@"C:\\Users\\Derek\\Documents\\EfireData.txt", electronEnergy_string);
            File.AppendAllText(@"C:\\Users\\Derek\\Documents\\EfireData.txt", ", ");
            File.AppendAllText(@"C:\\Users\\Derek\\Documents\\EfireData.txt", strikeTime_string);
            File.AppendAllText(@"C:\\Users\\Derek\\Documents\\EfireData.txt", "\n");
            //Console.ReadLine();
        }*/
    }
}
