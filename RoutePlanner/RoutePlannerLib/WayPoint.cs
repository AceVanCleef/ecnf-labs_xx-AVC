using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/** Lessons learned:
 *  - How to override ToString(): 'public override string ToString()'
 *    (Source: https://msdn.microsoft.com/en-us/library/ms173154.aspx )
 *    
 *  - rounding double using String.Format():
 *    string lat = String.Format("{0:0.00}", Latitude);
 *    string lng = String.Format("{0:0.00}", Longitude);
 *    (source: http://stackoverflow.com/questions/7076841/how-to-round-to-two-decimal-places-in-a-string )
 *
 *  - Culture specific characters:
 *    'CultureInfo.CreateSpecificCulture("de-CH")' in 
 *    string lat = string.Format(CultureInfo.CreateSpecificCulture("de-CH"), "{0:0.00}", Latitude);
 *    For doubles: ch = x.xx, de = x,xx.
 *    Why is this relevant? To prevent runtime errors when working with predefined string format
 *    You can also use CultureInfo culture = CultureInfo.InvariantCulture (recommended).
 */


namespace Fhnw.Ecnf.RoutePlanner.RoutePlannerLib
{
    public class WayPoint
    {

        public string Name { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public WayPoint(string name, double latitude, double longitude)
        {
            Name = name;
            Latitude = latitude;
            Longitude = longitude;
        }

        /// d = R arccos [sin (φa) • sin(φb) + cos(φa) • cos(φb) • cos(λa - λb)]
        /// Radius  R=6371km
        /// Borrowed by: Yanick Schraner
        public double Distance(WayPoint target)
        {
            int R = 6371;
            //return R * Math.Acos(Math.Sin(Longitude) * Math.Sin(target.Longitude) + Math.Cos(Longitude) * Math.Cos(target.Longitude) * Math.Cos(Latitude - target.Latitude));
            return R * Math.Acos(Math.Sin(DegreeToRad(Latitude))
                * Math.Sin(DegreeToRad(target.Latitude))
                + Math.Cos(DegreeToRad(Latitude))
                * Math.Cos(DegreeToRad(target.Latitude))
                * Math.Cos(DegreeToRad(Longitude - target.Longitude)));
        }

        private double DegreeToRad(double degree)
        {
            return degree * Math.PI / 180;
        }

        /** How to override ToString():
         *  see: https://msdn.microsoft.com/en-us/library/ms173154.aspx
         */
        public override string ToString()
        {
            /* Invariante Culture property: macht Formattierung Spachenunabhängig */
            CultureInfo _Invc = CultureInfo.InvariantCulture;
            CultureInfo _De_CH = CultureInfo.CreateSpecificCulture("de-CH");

            // rounding double using String.Format():
            string lat = String.Format(_Invc, "{0:0.00}", Latitude);
            string lng = String.Format(_Invc, "{0:0.00}", Longitude);
            //source: http://stackoverflow.com/questions/7076841/how-to-round-to-two-decimal-places-in-a-string

            return $"WayPoint:{((Name == null || Name == "") ? "" : " " + Name)} {lat}/{lng}";
        }


        /* lab04: operator overloading */

        /// <summary>
        /// implicitly overrides +=
        /// </summary>
        /// <param name="wp1"></param>
        /// <param name="wp2"></param>
        /// <returns></returns>
        public static WayPoint operator+ (WayPoint wp1, WayPoint wp2)
        {
            return new WayPoint(wp1.Name, 
                                wp1.Latitude + wp2.Latitude,
                                wp1.Longitude + wp2.Longitude);
        }

        /// <summary>
        /// implicitly overrides -=
        /// </summary>
        /// <param name="wp1"></param>
        /// <param name="wp2"></param>
        /// <returns></returns>
        public static WayPoint operator- (WayPoint wp1, WayPoint wp2)
        {
            return new WayPoint(wp1.Name,
                                wp1.Latitude - wp2.Latitude,
                                wp1.Longitude - wp2.Longitude);
        }
        /* END of lab04: operator overloading */

    }
}
