using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fhnw.Ecnf.RoutePlanner.RoutePlannerLib
{
    /* instances of this class are going to be subscribers to 
     * RouteRequested events (see Links.cs) */
    public class RouteRequestWatcher
    {
        /* City: angefragte Stadt. int: Anz. Anfragen für City. */
        private Dictionary<string, int> RequestStatistics = new Dictionary<City, int>();

        /// <summary>
        /// THE EVENT HANDLER: it must have the same signature as the event's delegate.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void LogRouteRequests(object source, EventArgs e)
        {
            
            if (source == null || e == null)
            {
                //throw exception
            }
            RequestStatistics.Add(); //GetCityRequests um int zu inkrementieren

            //Zählerstände auf console.write

        }

        /// <summary>
        /// returns the amount of requests for city.
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public int GetCityRequests(string cityName)
        {
            if (cityName == null)
            {
                //throw exception
            }
            if (!RequestStatistics.ContainsKey(cityName))
            {
                //throw exception
            }

            return RequestStatistics[cityName];
        }
    }
}
