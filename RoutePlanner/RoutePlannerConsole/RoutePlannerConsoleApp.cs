﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

/*Import WayPoint.cs */
using Fhnw.Ecnf.RoutePlanner.RoutePlannerLib;

/** Lessons learned:
 *  - when instantiating objects of a class situated in another assembly/namespace,
 *    its class must be public (or any other valid visibility level).
 */

namespace Fhnw.Ecnf.RoutePlanner.RoutePlannerConsole
{
    class RoutePlannerConsoleApp
    {

        static void Main(string[] args)
        {
            Console.WriteLine("My first C# Program: {0}", Assembly.GetExecutingAssembly().GetName().Version);

            var wayPoint = new WayPoint("Windisch", 47.479319847061966, 8.212966918945312);
            //Console.WriteLine($"{wayPoint.Name}: {wayPoint.Latitude}/{wayPoint.Longitude}");
            Console.WriteLine(wayPoint);

            //testing myWayPoint.distance(otherWayPoint):
            var wpBern = new WayPoint("Bern", 46.57266, 7.26471);
            var wpTripolis = new WayPoint("Tripolis", 32.53270, 13.10358);
            double distance = wpBern.Distance(wpTripolis);
            Console.WriteLine("The Distance between...");
            Console.WriteLine(wpBern + " and ");
            Console.WriteLine(wpTripolis);
            Console.WriteLine("equals: " + distance + "km.");


            //Um zu verhindern, dass Konsolenfenster gleich wieder geschlossen wird:
            Console.WriteLine("Press any key to quit");
            Console.ReadKey();
        }
    }
}
