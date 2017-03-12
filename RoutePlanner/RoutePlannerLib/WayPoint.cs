﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/** Lessons learned:
 *  - How to override ToString(): 'public override string ToString()'
 *    (Source: https://msdn.microsoft.com/en-us/library/ms173154.aspx )
 *    
 *    Inheritance and overriding of methods:
 *    Note, that in class Object, the method head looks like:
 *    public virtual string ToString(). 
 *    virtual means, it is the root class's method implementation you 
 *    will inherit or override.
 *    To define one of your own class's method the new base implementation, 
 *    use 'new virtual'.
 *    
 *  - rounding double using String.Format():
 *    string lat = String.Format("{0:0.00}", Latitude);
 *    string lng = String.Format("{0:0.00}", Longitude);
 *    (source: http://stackoverflow.com/questions/7076841/how-to-round-to-two-decimal-places-in-a-string )
 *
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

        /** How to override ToString():
         *  see: https://msdn.microsoft.com/en-us/library/ms173154.aspx
         */
        public override string ToString()
        {
            // rounding double using String.Format():
            string lat = String.Format("{0:0.00}", Latitude);
            string lng = String.Format("{0:0.00}", Longitude);
            //source: http://stackoverflow.com/questions/7076841/how-to-round-to-two-decimal-places-in-a-string

            return $"WayPoint: {(Name == null? "" : Name)} {lat}/{lng}";
        }
    }
}
