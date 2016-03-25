using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team_E_FIRE_Data_Processing_Program_BBB
{
    public class eStrike
    {
        //        eStrike: <strike number>, time: <time>, diode: <diode>, energy: <energy based on 8-bit number and gain settings>
        //           bin: <0-255>
        //            eField: <eFieldValue>
        public eStrike(int voltage, int diode)
        {
            //TODO: constructor for use in SPI_Reader
        }

        public eStrike()
        {
            //Nate: Temporary constructor for random datapoints
            strikeNumber++;
            strikeTime = DateTime.Now;
            Random random = new Random();
            diodeStruck = random.Next(0, 11);
            int whichBin = random.Next(1, 5);
            bin = randomBin(whichBin*50, 10);
            electronEnergy = bin * 4;
        }

        public string createStrikeJSON()
        {
            // var msg = "{strike: 1, time: 0:00, diode: 1, energy: 500, bin: 0}";
            return "{\"strike\":" + eStrike.strikeNumber.ToString() + ",\"time\":\"" + strikeTime + "\",\"diode\":" +
                diodeStruck + ",\"energy\":" + electronEnergy + ",\"bin\":" + bin + "}";
        }

        private static Random rand = new Random();

        //Nate temp: function to create a Gaussian distribution
        public int randomBin(int mean, int stdDev)
        {
            //Random rand = new Random(); //reuse this if you are generating many
            double u1 = rand.NextDouble(); //these are uniform(0,1) random doubles
            double u2 = rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal =
                         mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
            return (int)randNormal;
        }

        //Nate temp: make strikeNumber static so that it increments each time we make this class
        static public int strikeNumber = 0;
        //public int strikeNumber; //the most recent strike number that has been detected
        
        //Nate temp: make strikeTime a date type
        public DateTime strikeTime;
        //public long strikeTime; //the time at which the diode was struck (starting from the first electron detection)


        public int diodeStruck; //the ID number of the diode struck by an electron

        public long electronEnergy; //energy of the electron that impacted the diode
        public long eField; //the hypothetical electric field calculated using the current and previous strike
        public int bin; //the bin number the electron energy is placed within

/*      private int set_strikeNumber(int a)
            { return this->strikeNumber == a; }
        
        long calculate_eField(eStrike s1, this)
        {
            double pitch = 2.54;
            long delta = (fabs(s1.diodeNumber - this->diodeNumber)) * pitch;
            //calculate the eField
        }

        private double fabs(object p)
        {
            throw new NotImplementedException();
        }*/
    }
}
