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

        /* Overriding Equals() and Overloading operators:
         * https://msdn.microsoft.com/en-us/library/ms173147(VS.80).aspx
         * 
         */

        /// <summary>
        /// Achtung: Signatur muss mit Object.Equals(Object) zu 100% übereinstimmen.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false (this != null).
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to City return false (must be instance of City).
            City c = null;
            try
            {
                c = (City)obj; // <- when can't be cast, c == null
            }
            catch (InvalidCastException e)
            {
                return false;
            }

            
            // Return true if the fields match:
            
            return  this.Name.ToLowerInvariant() == c.Name.ToLowerInvariant() && 
                this.Country.ToLowerInvariant() == c.Country.ToLowerInvariant();
        }

        public bool Equals(City c)
        {
            // If parameter is null return false (this != null).
            if ((object)c == null)
            {
                return false;
            }

            // Return true if the fields match:
            return this.Name.ToLowerInvariant() == c.Name.ToLowerInvariant() &&
                this.Country.ToLowerInvariant() == c.Country.ToLowerInvariant();
        }

        public static bool operator== (City a, City b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }


            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null)) // ^ statt || ?
            {
                return false;
            }


            // Return true if the fields match:
            return a.Name.ToLowerInvariant() == b.Name.ToLowerInvariant() &&
                a.Country.ToLowerInvariant() == b.Country.ToLowerInvariant();
        }

        public static bool operator!= (City a, City b)
        {
            return !(a == b);
        }
    }
}
