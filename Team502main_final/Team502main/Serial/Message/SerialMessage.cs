using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team502main.Serial
{
    /// <summary>
    /// 시리얼 메시지를 핸들링하기 위한 객체입니다.
    /// </summary>
    class SerialMessage
    {
        public SerialMessage(string rawData = "")
        {
            this.rawData = rawData;
        }
        /// <summary>
        /// 해당 시리얼 메시지가 어떤 타입인지 결정합니다.
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// 해당 메시지가 보행자 정보를 나타냄을 표시합니다.
            /// </summary>
            TYPE_PEDESTRIAN,
            /// <summary>
            /// 해당 메시지가 GPS 정보를 나타냄을 표시합니다.
            /// </summary>
            TYPE_GPS,
            /// <summary>
            /// 해당 메시지가 자이로 정보를 나타냄을 표시합니다.
            /// </summary>
            TYPE_GYRO
        }
        private Type messageType;
        public Type MessageType { get => messageType; set => messageType = value; }
        private string rawData;
        public string RawData { get => rawData; set => rawData = value; }
    }
}
