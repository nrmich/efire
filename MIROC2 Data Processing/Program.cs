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
            CSV_Writer csv = new CSV_Writer();
            csv.WriteStrikeToCSV(456);
            //int ToInt32 {byte value;}

        }
    }
}


