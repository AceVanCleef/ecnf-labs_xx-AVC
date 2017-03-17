using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fhnw.Ecnf.RoutePlanner.RoutePlannerLib
{
    public class City
    {
 
        public string Name { get; set; }
        public string Country { get; set; }
        public int Population { get; set; }
        public WayPoint Location { get; set; }

        public City(string name, string country, int population, WayPoint location)
        {
            Name = name;
            Country = country;
            Population = population;
            Location = location;
        }

        public City(string name, string country, int population, double latitude, double longitude)
        {
            Name = name;
            Country = country;
            Population = population;
            Location = new WayPoint(name, latitude, longitude);
        }
    }
}
