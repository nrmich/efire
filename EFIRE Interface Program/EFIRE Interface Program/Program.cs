using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Byte;

public sealed class SPI : IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

namespace Team_E_FIRE_Data_Processing_Program_BBB
{
    class Program       //use Ctrl+F5 to run!
    {
        static void Main(string[] args)
        {
            //receive data in binary or decimal format using SPI control

            //SPI function goes here and outputs eStrike objects
            //while loop of inputs to csv.WriteStrikeToCSV with inputs of eStrikei.membervar 
                //while loop will run as long as test is running; while(eof) or while(strike==true)
            bool strike = true; //this will be determined in the SPI control

            //initializes count variable 'i' and object for CSV_Writer
            int i = 0;
            CSV_Writer csv = new CSV_Writer();
            while (strike)
            {
                i++;
                eStrike strike_i = new eStrike();
                csv.WriteStrikeToCSV(strike_i.diodeStruck, strike_i.electronEnergy, strike_i.strikeTime);
                strike_i = null;
            }

            //int ToInt32 {byte value;}

        }
    }
}


