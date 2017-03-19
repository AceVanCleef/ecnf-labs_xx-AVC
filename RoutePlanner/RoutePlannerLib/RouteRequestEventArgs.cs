using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fhnw.Ecnf.RoutePlanner.RoutePlannerLib
{
    /// <summary>
    /// instances of this class hold the information needed 
    /// by the RouteRequested event. 
    /// 
    /// See Links.cs:
    /// Firing event -> RouteRequested(object source, EventArgs e)
    /// </summary>
    public class RouteRequestEventArgs : EventArgs
    {
        /* readonly = superior final (avoid const) */
        public readonly string fromCity;
        public readonly string toCity;
        public readonly TransportMode tansportMode;

        /* required to create an object that holds informatioin for the event */
        public RouteRequestEventArgs(string fromCity, string toCity, TransportMode tm)
        {
            this.fromCity = fromCity;
            this.toCity = toCity;
            tansportMode = tm;
        }

    }
}
