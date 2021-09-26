using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team502main
{
    /// <summary>
    /// 디바이스의 운전자 정보를 나타냄
    /// </summary>
    class User
    {
        /// <summary>
        /// 새 운전자 객체를 생성합니다.
        /// </summary>
        /// <param name="lat">위도 좌표입니다.</param>
        /// <param name="lng">경도 좌표입니다.</param>
        /// <param name="theta">바라보는 각도입니다.</param>
        public User(double lat = 0.0, double lng = 0.0, double theta = 0.0)
        {
            Lat = lat;
            Lng = lng;
            Theta = theta;
        }
        private double lat;
        private double lng;

        private double theta;

        /// <summary>
        /// 위도를 나타냅니다.
        /// </summary>
        public double Lat { get => lat; set => lat = value; }
        /// <summary>
        /// 경도를 나타냅니다.
        /// </summary>
        public double Lng { get => lng; set => lng = value; }
        /// <summary>
        /// 바라보는 각도를 나타냅니다.
        /// </summary>
        public double Theta { get => theta; set => theta = value; }
    }
}
