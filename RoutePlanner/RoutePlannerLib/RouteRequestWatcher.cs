using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fhnw.Ecnf.RoutePlanner.RoutePlannerLib
{
    /// <summary>
    /// instances of this class are going to be subscribers to
    /// RouteRequested events(see Links.cs)
    /// </summary>
    public class RouteRequestWatcher
    {
        /* City: angefragte Stadt. int: Anz. Anfragen für City. */
        private Dictionary<City, int> RequestStatistics = new Dictionary<City, int>();

        /// <summary>
        /// THE EVENT HANDLER: it must have the same signature as the event's delegate.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void LogRouteRequests(object source, RouteRequestEventArgs e)
        {
            var key = e.ToCity;
            var value = GetCityRequests(key);
            if (source == null || e == null)
            {
                //throw exception
            }
            RequestStatistics.Add(key, value); //GetCityRequests um int zu inkrementieren

            //Zählerstände auf console.write

        }

        /// <summary>
        /// returns the amount of requests for city.
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public int GetCityRequests(City city)
        {
            if (city == null)
            {
                //throw exception
            }
            if (!RequestStatistics.ContainsKey(city))
            {
                return 0;
            }

            return RequestStatistics[city];
        }
    }
}
