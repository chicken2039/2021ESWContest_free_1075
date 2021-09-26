using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
namespace Team502main
{
    class Target : User
    {
        /// <summary>
        /// 새 보행자 객체를 생성합니다.
        /// </summary>
        /// <param name="uId">User Id</param>
        /// <param name="dBm">수신 감도입니다. 숫자가 클 수록 가깝습니다. 최대값은 12.5입니다.</param>
        /// <param name="lat">위도 좌표입니다.</param>
        /// <param name="lng">경도 좌표입니다.</param>
        /// <param name="theta">바라보는 각도입니다.</param>
        public Target(string uId = "", double dBm = 0.0, double lat = 0.0, double lng = 0.0, double theta = 0.0)
        {
            Uid = uId;
            dbm = dBm;
            Lat = lat;
            Lng = lng;
            Theta = theta;
        }
        public string Uid;
        public double dbm = 0;
        public double distance = 999.0f;
        public double theta = 0f;
    }
}
