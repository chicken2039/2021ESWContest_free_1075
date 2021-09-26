using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team502main.Serial
{
    /// <summary>
    /// Serial 메시지 수신 이벤트를 전달하기 위한 콜백 인터페이스입니다.
    /// </summary>
    interface ISerialReadNotification
    {
        /// <summary>
        /// Serial 메시지가 수신되면 콜백이 호출됩니다.
        /// </summary>
        /// <param name="message">수신된 메시지 데이터입니다.</param>
        void OnMessageReceive(string message);
    }
}
