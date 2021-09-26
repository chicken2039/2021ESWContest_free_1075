using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team502main.Util
{
    class LocationManager
    {
        /// <summary>
        /// 운전자와 보행자 간 거리를 계산합니다.
        /// </summary>
        /// <param name="t">계산할 보행자 객체입니다.</param>
        /// <param name="u">계산할 운전자 객체입니다.</param>
        /// <returns>두 사용자 간 거리를 m로 반환합니다.</returns>
        public static double GetDistance(Target t, User u)
        {
            if ((t.Lat == 0.0) || (t.Lng == 0.0) || (u.Lat == 0) || (u.Lng == 0))
            {
                if (t.dbm > 9) { return 12.5; }
                else if (t.dbm > 0 && t.dbm <= 9) { return 37.5; }
                else { return 70.5; }

            }
            else
            {
                double theta = t.Lng - u.Lng;
                double dist = Math.Sin(Deg2rad(t.Lat)) * Math.Sin(Deg2rad(u.Lat)) + Math.Cos(Deg2rad(t.Lat)) * Math.Cos(Deg2rad(u.Lat)) * Math.Cos(Deg2rad(theta));
                dist = Math.Acos(dist);
                dist = Rad2deg(dist);
                dist = dist * 60 * 1.1515;
                dist *= 1609.344;
                return dist;
            }
        }

        /// <summary>
        /// 운전자와 보행자 간 각도를 계산합니다.
        /// </summary>
        /// <param name="t">계산할 보행자 객체입니다.</param>
        /// <param name="u">계산할 운전자 객체입니다.</param>
        /// <returns>두 사용자 간 각도를 정북방향 기준으로 반환합니다.</returns>
        public static double GetBearing(Target t, User u)
        {
            double tLat = Deg2rad(t.Lat); double tLng = Deg2rad(t.Lng);//φ₁ ; λ₁
            double uLat = Deg2rad(u.Lat); double uLng = Deg2rad(u.Lng);//φ₂ ; λ₂

            double theta = Math.Atan2(Math.Sin(tLng - uLng) * Math.Cos(tLat), Math.Cos(uLat) * Math.Sin(tLat) - Math.Sin(uLat) * Math.Cos(tLat) * Math.Cos(tLng - uLng));
            double degree = (theta * 180 / Math.PI + 360) % 360;
            return degree;

        }

        /// <summary>
        /// 산술각도를 라디안으로 변환합니다.
        /// </summary>
        /// <param name="deg">산술각도입니다.</param>
        /// <returns>변환된 라디안 각도입니다.</returns>   
        public static double Deg2rad(double deg)
        {
            return deg * Math.PI / 180.0;
        }

        /// <summary>
        /// 라디안 각도를 산술각도로 변환합니다.
        /// </summary>
        /// <param name="rad">라디안 각도입니다.</param>
        /// <returns>변환된 산술각도입니다.</returns>
        public static double Rad2deg(double rad)
        {
            return rad / Math.PI * 180.0;
        }
    }
}
