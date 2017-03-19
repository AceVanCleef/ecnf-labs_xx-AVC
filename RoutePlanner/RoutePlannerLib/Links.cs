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

        /// <summary>
        /// this method, when called, does the raising of an RouteRequested event.
        /// or: this method, when called, fires an RouteRequested event.
        /// Eventaufruf = firing an event
        /// </summary>
        /// <param name="fromCity"></param>
        /// <param name="toCity"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public List<Link> FindShortestRouteBetween(string fromCity, string toCity, TransportMode mode)
        {
            /* Find -> also suchen (implementierung folgt später */
            var from = cities[fromCity];
            var to = cities[toCity];
            /* Are there any subscribers? null = no */
            if (RouteRequested != null && from != null && to != null) 
            {
                /* RAISE THE EVENT: treat event name as Method with parameter list from its delegate */
                RouteRequested(this, new RouteRequestEventArgs(from, to, mode));
            }

            //TODO: implementierung fertig stellen
            return new List<Link>();
        }
    }
}
