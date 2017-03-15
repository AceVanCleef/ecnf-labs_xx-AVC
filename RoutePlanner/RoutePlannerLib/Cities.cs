using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/** Lessons learned:
 *  - parse double/int to string: 
 *    http://stackoverflow.com/questions/1354924/how-do-i-parse-a-string-with-a-decimal-point-to-a-double
 *  - Split a string into an array of substrings (string[]). Used in combinatioin
 *    with Streamreader's ReadLine().
 *    http://stackoverflow.com/questions/2797647/separate-string-by-tab-characters
 *  - Create an indexer for a List<T> (not yet verified if it works)
 *    http://stackoverflow.com/questions/6831654/how-can-i-create-an-index-accessor-for-a-listt-property-i-have-in-a-custom-cla
 *  - What StreamReader's .Peek() does:
 *    https://msdn.microsoft.com/en-us/library/system.io.streamreader.peek(v=vs.110).aspx
 *  - How to use a StreamReader (not yet verified if it works)
 *    https://msdn.microsoft.com/en-us/library/system.io.streamreader.readline(v=vs.110).aspx
 *  - Closing files/streams: 
 *    'using' closes the file/stream automatically and also more  
      reliably. No .Close() or .Dispose() necessary.  
 * 
 */

//ToDo: 

namespace Fhnw.Ecnf.RoutePlanner.RoutePlannerLib
{
    public class Cities
    {
        private List<City> _cities = new List<City>();

        /* amount of cities read from file and stored in _cities */
        public int Count { get { return _cities.Count(); } }  //Done: set löschen; get: _cities.count()

        public City this[int index] //indexer implementation
        {
            get {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException("index too high or too low.");
                }
                return this._cities[index]; }
            /*
            set {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException("index too high or too low.");
                }
                this._cities[index] = value;
            }
            */
        }


        public int ReadCities(string filename)
        {
            int countNewEntries = 0;
            try
            {
                if (!File.Exists(filename))
                {
                    throw new IOException("File not found");
                }

                using (var reader = new StreamReader(filename))
                {
                    while (reader.Peek() >= 0) // <- Are there more lines to read?
                    {
                        string _FileLine = reader.ReadLine();
                        
                        //create new City:
                        City NewCity = ConverttoCity(_FileLine); //nicht so. Sondern cities.add(city)
                        //ist city schon drinn? wenn nein, dann erst hinzufügen
                        if (!_cities.Contains(NewCity))
                        {
                            _cities.Add(NewCity);
                            //increment count
                            ++countNewEntries;
                        }

                        
                    }
                    //Note: using closes the file/stream automatically and also more  
                    //      reliably. No .Close() or .Dispose() necessary.
                }
            } catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
            return countNewEntries;
        }

        ///converts a String with \t as value delimiter to City attributes.
        private static City ConverttoCity(string fileline)
        {
            string[] CityAttributes = fileline.Split('\t'); // single quote = character!
            string name = CityAttributes[0];
            string city = CityAttributes[1];
            int population = int.Parse(CityAttributes[2]);
            double latitude = double.Parse(CityAttributes[3]);
            double longitude = double.Parse(CityAttributes[4]);

            WayPoint location = new WayPoint(name, latitude, longitude);

            //create new City:
            return new City(name, city, population, location);
        }

    }
}
