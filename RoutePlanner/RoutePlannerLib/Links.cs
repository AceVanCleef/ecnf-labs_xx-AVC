using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using Fhnw.Ecnf.RoutePlanner.RoutePlannerLib;
using System.Globalization;
using Fhnw.Ecnf.RoutePlanner.RoutePlannerLib.Util;
using System.Threading.Tasks;

namespace Fhnw.Ecnf.RoutePlanner.RoutePlannerLib
{

    ///	<summary>
    ///	Manages	links from a city to another city.
    ///	</summary>
    public class Links
    {
        private List<Link> links = new List<Link>();
        private Cities cities;

        public int Count { get { return links.Count; } }

        /* Links.cs is the publisher of RouteRequested events. It will deliver to its suscribers. */
        // 1. - Define a delegate
        // 2. - define an event based on that delegate
        // 3. - raise the event
        //      Source:
        //      https://www.youtube.com/watch?v=jQgwEsJISy0

        /* sender = source of the event. args = any additional info about event. */
        public delegate void RouteRequestHandler(object sender, RouteRequestEventArgs args);
        public event RouteRequestHandler RouteRequested; //-ing: event should be fired, -ed: event has happened and finished

        public City[] FindCities(TransportMode arg)
        {
            return links.Where(link => link.TransportMode == arg)
                .SelectMany(link => new[] { link.FromCity , link.ToCity })
                .Distinct()
                .ToArray();
        }


        ///	<summary>
        ///	Initializes	the	Links with	the	cities.
        ///	</summary>
        ///	<param name="cities"></param>
        public Links(Cities cities)
        {
            this.cities = cities;
        }

        ///	<summary>
        ///	Reads a	list of	links from the given file.
        ///	Reads only links where the cities exist.
        ///	</summary>
        ///	<changelog>
        ///	lab04 - 1.c): 
        ///	Ignorieren Sie beim Einlesen in Links.ReadRoutes Verbindungen 
        /// zwischen unbekannten Städten.
        /// Bsp: Schaffhausen nicht in citiesTestDataLab4, aber in linksTestDataLab4.
        /// What changed?
        /// Searching a city by using the Cities' Indexer threw a KeyNotFoundException.
        /// I had to catch it using a try/catch - block.
        /// </changelog>
        ///	<param name="filename">name	of links file</param>
        ///	<returns>number	of read	route</returns>
        public int ReadLinks(string filename)
        {
            var previousCount = Count;
            using (TextReader reader = new StreamReader(filename))
            {
                IEnumerable<string[]> linksAsStrings = reader.GetSplittedLines('\t');


                /* Geht nicht. TestTask4Findroutes && TestTask4ReadRoutes */
                //IEnumerable<Link> newLinks = linksAsStrings.Select((string[] lk) =>
                //{
                //    try
                //    {
                //        var city1 = cities[lk[0]]; //city not found? indexer of Cities throws KeyNotFoundExc.
                //        var city2 = cities[lk[1]];

                //        // try == if (cities[city1.Name] != null && cities[city2.Name] != null)
                //        return new Link(city1, city2, city1.Location.Distance(city2.Location), TransportMode.Rail);
                //    }
                //    catch (KeyNotFoundException e)
                //    {
                //        Console.WriteLine(e.Message);
                //        return null;
                //    }
                //});
                //links.AddRange(newLinks);

                foreach (string[] lk in linksAsStrings)
                {
                    try
                    {
                        var city1 = cities[lk[0]]; //city not found? indexer of cities throws keynotfoundexc.
                        var city2 = cities[lk[1]];

                        // try == if (cities[city1.name] != null && cities[city2.name] != null)
                        links.Add(new Link(city1, city2, city1.Location.Distance(city2.Location), TransportMode.Rail));
                    }
                    catch (KeyNotFoundException e)
                    {
                        Console.WriteLine(e.Message);
                    }

                }

            }

            return Count - previousCount;
        }

        


        /*    region Lab04: Dijkstra implementation    */
        /*    lab07 refractoring: added IProgress argument */
        public List<Link> FindShortestRouteBetween(string fromCity, string toCity, 
            TransportMode mode, IProgress<string> reportProgress)
        {
            reportProgress?.Report("Enter Method done");
            //DONE: inform listeners
            /* Find -> also suchen (implementierung folgt später */
            var from = cities[fromCity];
            var to = cities[toCity];
            /* Are there any subscribers? null = no */
            if (RouteRequested != null && from != null && to != null)
            {
                /* RAISE THE EVENT: treat event name as Method with parameter list from its delegate */
                RouteRequested(this, new RouteRequestEventArgs(from, to, mode));
            }
            reportProgress?.Report("Initial done");



            //use dijkstra's algorithm to look for all single-source shortest paths
            var visited = new Dictionary<City, DijkstraNode>();
            var pending = new SortedSet<DijkstraNode>(new DijkstraNode[]
            {
                new DijkstraNode()
                {
                    VisitingCity = cities[fromCity],
                    Distance = 0
                }
            });

            while (pending.Any())
            {
                var cur = pending.Last();
                pending.Remove(cur);

                if (!visited.ContainsKey(cur.VisitingCity))
                {
                    visited[cur.VisitingCity] = cur;

                    foreach (var link in FindAllLinksForCity(cur.VisitingCity, mode))
                        pending.Add(new DijkstraNode()
                        {
                            VisitingCity = (link.FromCity.Equals(cur.VisitingCity)) ? link.ToCity : link.FromCity,
                            Distance = cur.Distance + link.Distance,
                            PreviousCity = cur.VisitingCity
                        });
                }
            }
            reportProgress?.Report("all single-source shortest paths checking done");


            //did we find any route?
            if (!visited.ContainsKey(cities[toCity]))
            {
                reportProgress?.Report("Shortest route does Not exist done");
                return null;
            }
            reportProgress?.Report("ShortestRouteDiscovered done");


            //create a list of cities that we passed along the way
            var citiesEnRoute = new List<City>();
            for (var c = cities[toCity]; c != null; c = visited[c].PreviousCity)
                citiesEnRoute.Add(c);
            citiesEnRoute.Reverse();
            reportProgress?.Report("CitiesEnRouteRecorded done");


            //convert that city-list into a list of links
            IEnumerable<Link> paths = ConvertListOfCitiesToListOfLinks(citiesEnRoute);
            reportProgress?.Report("ConvertingToCityList done");
            return paths.ToList();
        }

        private IEnumerable<Link> ConvertListOfCitiesToListOfLinks(List<City> citiesEnRoute)
        {
            /* Todo: need explanations. Where did you find out what to do? */
            List<Link> _links = new List<Link>();
            for (int i = 0; i < citiesEnRoute.Count - 1; i++)
            {
                _links.Add(new Link(citiesEnRoute[i], citiesEnRoute[i + 1], citiesEnRoute[i].Location.Distance(citiesEnRoute[i + 1].Location)));
            }
            return _links;
        }

        private IEnumerable<Link> FindAllLinksForCity(City visitingCity, TransportMode mode)
        {
            /* Todo: What happens here? Why this way? */
            return links.Where(link => (link.ToCity.Equals(visitingCity) || link.FromCity.Equals(visitingCity)) && link.TransportMode.Equals(mode));
        }


        private class DijkstraNode : IComparable<DijkstraNode>
        {
            public City VisitingCity;
            public double Distance;
            public City PreviousCity;

            public int CompareTo(DijkstraNode other)
            {
                return other.Distance.CompareTo(Distance);
            }
        }
        /* END of lab04 */

        
        /*************************** lab06 *******************************/
        public int GetCountOfThreeBiggestCitiesInLinks()
        {
            //Bei wievielen Links...    -> links.Count()
            //treten die drei bevölkerungsreichsten Städte... 
            //                          -> OrderByDescending(), .Take(3)
            //aller Cities...           -> cities.CityList
            //in den Links...           -> links
            //als Start-Stadt auf ?     -> links.Where(link => threeBiggestCities.Contains(link.FromCity)) -ish
            var threeBiggestCities = cities.CityList.OrderByDescending(city => city.Population)
                .Take(3);
            var count = links.Where(link => threeBiggestCities.Contains(link.FromCity))
                .Count();
            

            return count;
        }

        public int GetCountOfThreeCitiesWithLongestNameInLinks()
        {
            //Bei wievielen Links...                -> links.Count();
            //treten die Städte...                  -> cities.CityList
            //mit den drei längsten Städtenamen...  -> .OrderByDescending(city => city.Name.Length).Take(3)
            //in den Links...                       -> links.Where(..*.)
            //insgesamt auf, entweder als Start- oder als Ziel-Stadt? -> *longestNamedCities.Contains(a) || longestNamedCities.Contains(b)
            //a = link.FromCity, b = link.Tocity
            var longestNamedCities = cities.CityList.OrderByDescending(city => city.Name.Length)
                .Take(3);
            var count = links.Where(link => longestNamedCities.Contains(link.FromCity) || longestNamedCities.Contains(link.ToCity))
                .Count();

            return count;
        }

        /*************************** END of lab06 *******************************/


        /*************************** lab07 *******************************/

        //Note:  reportProgress?.Report("myReportMsg") is the same as
        //       if (reportProgress != null) reportProgress.Report("myReportMsg")

        public async Task<List<Link>> FindShortestRouteBetweenAsync(string v1, string v2, TransportMode rail)
        {
            return await Task.Run(() => FindShortestRouteBetween(v1, v2, rail));
        }

        public async Task<List<Link>> FindShortestRouteBetweenAsync(string v1, string v2, 
            TransportMode rail, Progress<String> reportProgress)
        {
            return await Task.Run(() => FindShortestRouteBetween(v1, v2, rail, reportProgress));
        }

        public List<Link> FindShortestRouteBetween(string fromCity, string toCity, TransportMode mode)
        {
            return FindShortestRouteBetween(fromCity, toCity, mode, null);
        }
        /*************************** END of lab07 *******************************/

    }
}
