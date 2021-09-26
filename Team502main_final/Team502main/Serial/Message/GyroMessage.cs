using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team502main.Serial.Message
{
    /// <summary>
    /// 메인보드에서 수신된 차량의 자이로 업데이트 정보 객체입니다.
    /// </summary>
    class GyroMessage : SerialMessage
    {
        private double yaw;
        /// <summary>
        /// 정북방향을 기준으로 하는 방위각입니다.
        /// </summary>
        public double Yaw { get => yaw; set => yaw = value; }
        public GyroMessage(string rawData = "", double yaw = 0.0) : base(rawData)
        {
            this.yaw = yaw;
            MessageType = Type.TYPE_GYRO;
        }
    }
}
