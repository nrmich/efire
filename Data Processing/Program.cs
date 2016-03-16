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
    class Program
    {
        friend CSV_Writer;
        static void Main(string[] args)
        {
            CSV_Writer csv;
            WriteStrikeToCSV();
            //int ToInt32 {byte value;}

        }
    }
}


