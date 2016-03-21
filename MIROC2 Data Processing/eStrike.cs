using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team_E_FIRE_Data_Processing_Program_BBB
{
    class eStrike
    {
        //        eStrike: <strike number>, time: <time>, diode: <diode>, energy: <energy based on 8-bit number and gain settings>
        //           bin: <0-255>
        //            eField: <eFieldValue>
        public eStrike() { }
        public
            int strikeNumber; //the most recent strike number that has been detected
        public long strikeTime; //the time at which the diode was struck (starting from the first electron detection)
        public int diodeStruck; //the ID number of the diode struck by an electron
        public long electronEnergy; //energy of the electron that impacted the diode
            long eField; //the hypothetical electric field calculated using the current and previous strike
            int bin; //the bin number the electron energy is placed within
/*        private
            int set_strikeNumber(int a)
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
        }
    }*/
}
