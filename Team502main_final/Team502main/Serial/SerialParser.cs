using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Team502main.Serial.Message;

namespace Team502main.Serial
{
    class SerialParser
    {
        public SerialMessage ParseMessage(string rawData)
        {
            try
            {
                switch (rawData[0])
                {
                    case 'p':
                        return ParsePedestrianMessage(rawData);
                    case 'g':
                        return ParseGPSMessage(rawData);
                    case 'y':
                        return ParseGyroMessage(rawData);
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        private GPSMessage ParseGPSMessage(string rawData)
        {
            var slat = rawData.Substring(1, 10);
            var slng = rawData.Substring(11, 11);
            var lat = Math.Round(Convert.ToDouble(slat.Substring(0, 2)) + Convert.ToDouble(slat.Substring(2)) / 60, 6);
            var lng = Math.Round(Convert.ToDouble(slng.Substring(0, 3)) + Convert.ToDouble(slng.Substring(3)) / 60, 6);
            return new GPSMessage(rawData, lat, lng);
        }
        private PedestrianMessage ParsePedestrianMessage(string rawData)
        {
            var uid = rawData.Substring(1, 16);
            var slat = rawData.Substring(17, 10);
            var slng = rawData.Substring(27, 11);
            var lat = Math.Round(Convert.ToDouble(slat.Substring(0, 2)) + Convert.ToDouble(slat.Substring(2)) / 60, 6);
            var lng = Math.Round(Convert.ToDouble(slng.Substring(0, 3)) + Convert.ToDouble(slng.Substring(3)) / 60, 6);
            var dBm = Convert.ToDouble(rawData.Substring(38, 6)) / 100;
            return new PedestrianMessage(new Target(uid, dBm, lat, lng), rawData, lat, lng);
        }
        private GyroMessage ParseGyroMessage(string rawData)
        {
            var yaw = Convert.ToDouble(rawData.Substring(1, 7));
            return new GyroMessage(rawData, yaw);
        }
    }
}
