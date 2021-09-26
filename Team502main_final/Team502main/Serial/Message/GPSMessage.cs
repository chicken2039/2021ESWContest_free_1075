using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team502main.Serial.Message
{
    /// <summary>
    /// 메인보드에서 수신된 차량의 GPS 업데이트 정보 객체입니다.
    /// </summary>
    class GPSMessage : SerialMessage
    {
        private double latitude;
        private double longitude;
        public GPSMessage(string rawData = "", double latitude = 0.0, double longitude = 0.0) : base(rawData)
        {
           
            this.latitude = latitude;
            this.longitude = longitude;
            MessageType = Type.TYPE_GPS;
        }
        public double Latitude{ get => latitude; set => latitude = value; }
        public double Longitude { get => longitude; set => longitude = value; }
    }
}
