using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using Fhnw.Ecnf.RoutePlanner.RoutePlannerLib;

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
        ///	<param name="filename">name	of links file</param>
        ///	<returns>number	of read	route</returns>
        public int ReadLinks(string filename)
        {
            var previousCount = Count;
            using (var reader = new StreamReader(filename))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var tokens = line.Split('\t');
                    
                    var city1 = cities[tokens[0]];
                    var city2 = cities[tokens[1]];
                    
                    links.Add(new Link(city1, city2, city1.Location.Distance(city2.Location), TransportMode.Rail));
                }
            }
            
            return Count - previousCount;
        }

        


        /*    region Lab04: Dijkstra implementation    */
        public List<Link> FindShortestRouteBetween(string fromCity, string toCity, TransportMode mode)
        {
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

            //did we find any route?
            if (!visited.ContainsKey(cities[toCity]))
                return null;

            //create a list of cities that we passed along the way
            var citiesEnRoute = new List<City>();
            for (var c = cities[toCity]; c != null; c = visited[c].PreviousCity)
                citiesEnRoute.Add(c);
            citiesEnRoute.Reverse();

            //convert that city-list into a list of links
            IEnumerable<Link> paths = ConvertListOfCitiesToListOfLinks(citiesEnRoute);
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
    }
}
