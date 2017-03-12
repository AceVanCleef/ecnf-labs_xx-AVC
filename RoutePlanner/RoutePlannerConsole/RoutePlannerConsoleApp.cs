using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

/*Import WayPoint.cs */
using Fhnw.Ecnf.RoutePlanner.RoutePlannerLib;

namespace Fhnw.Ecnf.RoutePlanner.RoutePlannerConsole
{
    class RoutePlannerConsoleApp
    {

        static void Main(string[] args)
        {
            Console.WriteLine("My first C# Program: {0}", Assembly.GetExecutingAssembly().GetName().Version);

            var wayPoint = new WayPoint("Windisch", 47.479319847061966, 8.212966918945312);
            Console.WriteLine($"{wayPoint.Name}: {wayPoint.Latitude}/{wayPoint.Longitude}");

            //Um zu verhindern, dass Konsolenfenster gleich wieder geschlossen wird:
            Console.WriteLine("Press any key to quit");
            Console.ReadKey();
        }
    }
}
