using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team502main.Serial.Message
{
    /// <summary>
    /// 메인보드에서 수신된 보행자 인식 정보 객체입니다.
    /// </summary>
    class PedestrianMessage : SerialMessage
    {
        private Target user;
        private string rawData;
        private double latitude;
        private double longitude;
        public PedestrianMessage(Target user = null, string rawData = "", double latitude = 0.0, double longitude = 0.0) : base(rawData)
        {
            if (user != null) this.user = user;
            else this.user = new Target("",0,latitude, longitude);
            this.rawData = rawData;
            this.latitude = latitude;
            this.longitude = longitude;
            MessageType = Type.TYPE_PEDESTRIAN;
        }
        public Target User { get => user; set => user = value; }
        public double Latitude { get => latitude; set => latitude = value; }
        public double Longitude { get => longitude; set => longitude = value; }
    }
}
