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


        /* lab06: tracks request dates */
        private Dictionary<DateTime, List<City>> requestsDate = new Dictionary<DateTime, List<City>>();


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
            if (RequestStatistics.ContainsKey(key))
            {
                //key already exists
                RequestStatistics[key] = value + 1;
            }
            else
            {
                //key doesnt exist
                RequestStatistics.Add(key, value + 1);
            }

            /*lab 06: */
            if (requestsDate.ContainsKey(GetCurrentDate))
            {
                requestsDate[GetCurrentDate].Add(e.ToCity);
            }
            else
            {
                requestsDate.Add(GetCurrentDate, new List<City>());
                requestsDate[GetCurrentDate].Add(e.ToCity);
            }

            //Zählerstände auf Konsole ausgeben
            Console.WriteLine($"ToCity: {key.Name} has been requested {(GetCityRequests(key))} times");

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
                return 0; //initial value
            }

            return RequestStatistics[city];
        }

        /*************************** lab06 *******************************/

        public virtual DateTime GetCurrentDate { get { return DateTime.Now; } }

        public IEnumerable<City> GetBiggestCityOnDay(DateTime day)
        {
            //Was waren die drei...             -> .Take(3)
            //bevölkerungsreichsten Städte,...  -> .OrderByDescending(city => city.Population)
            //die an einem bestimmten Tag...    ->  .Where( ? )
            //abgefragt wurden?
            if (requestsDate.ContainsKey(day))
            {
                return requestsDate[day].Distinct().OrderByDescending(city => city.Population).Take(3);
            }
            return new List<City>();

        }

        public IEnumerable<City> GetLongestCityNamesWithinPeriod(DateTime from, DateTime to)
        {
            return requestsDate.Where(elem => elem.Key >= from && elem.Key <= to).SelectMany(elem => elem.Value).Distinct().OrderByDescending(cities => cities.Name.Length).Take(3);
        }

        public IEnumerable<City> GetNotRequestedCities(Cities cities)
        {
            return cities.CityList.Except(requestsDate.Where(elem => elem.Key >= GetCurrentDate.AddDays(-14)).SelectMany(elem => elem.Value).Distinct());
        }

        /*************************** END of lab06 *******************************/

    }
}
