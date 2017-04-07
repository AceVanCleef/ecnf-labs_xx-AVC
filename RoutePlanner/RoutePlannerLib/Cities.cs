using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fhnw.Ecnf.RoutePlanner.RoutePlannerLib.Util;

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
 *    reliably. No .Close() or .Dispose() necessary.  
 *
 *  - LINQ:
 *    -----
 *    LINQ enables you to query any collection implementing IEnumerable<T>, whether
 *    an array, list, or XML DOM, as well as remote data sources, such as tables in a SQL
 *    Server database. LINQ offers the benefits of both compile-time type checking and
 *    dynamic query composition. 
 *    
 *    LINQ: pendant zu Java Collection Streams API
            * return:  an output sequence, transformed from input sequence
            * _cities: Enumerable input sequence (such as List, Array, SQL-DB).
            * city:    element of enumerable input sequence
            * a == b  conditional, search filter criteria (Predicate)
            * or:   lambda (n => a.contains(b))
            * .Where() - LINQ standard operator
 *
 * 
 *  - No throws keyword in C#
 *    ------------------------
 *    use this instead:
 *    try {
 *        //some code
 *    } catch (SomeException e){
 *        throw new SomeException(e.Message);
 *    }
 *    
 *  - Culture parsing problem solution:
 *    Double.Parse(string, CultureInfo:InvarianteCulture)
 */


namespace Fhnw.Ecnf.RoutePlanner.RoutePlannerLib
{
    public class Cities
    {
        private List<City> _cities = new List<City>();

        /* public access to _cities (lab06) */
        public List<City> CityList { get; set; }

        /* amount of cities read from file and stored in _cities */
        public int Count { get { return _cities.Count(); } }  //Done: set löschen; get: _cities.count()

         public City this[int index] //indexer implementation
         {
             get
             {
                 if (index < 0 && Count <= index) 
                 {
                     throw new ArgumentOutOfRangeException("index too high or too low.");
                 }
                 else
                 {
                     return this._cities[index];
                 }
             }
         }

        /* search by Key */
        public City this[string cityName]
        {
            get
            {
                if (cityName == null || cityName == "")
                {
                    throw new KeyNotFoundException("Not a valid key.");
                }
                /* Todo: Bitte erklären, was hier geschieht und warum man das so macht.
                 *       Ebenso: Kann man statt dem Lambda eine Methode schreiben und die
                 *       dem Predicate übergeben? Wie sähe diese Methode aus?
                 *       
                 *       Bitte Anwendungsbeispiele für alle in Folien 26 - 30 genannten
                 *       Predefined delegates. Der Sprung von _predicate = myMethod zu
                 *       _predicate = new Predicate("Was kommt hier rein?") ist zu gross.
                 *       Fehlender Zusammenhang.
                 *       
                 *       Speichert ein Rredicate die Referenz auf eine Methode?
                 *       Lambda ersetzt Methode, nehme ich an.
                 *       
                 *       Application examples:
                 *       https://msdn.microsoft.com/en-us/library/bfcke1bz(v=vs.110).aspx
                 */
                Predicate<City> _predicate = new Predicate<City>(
                    city => city.Name.ToLowerInvariant().Equals(cityName.ToLowerInvariant() ));
                City _foundCity = _cities.Find(_predicate);
                /* End of 'Brauche Erklärungen' */
                if (_foundCity == null)
                {
                    throw new KeyNotFoundException($"{cityName} not found.");
                }
                return _foundCity;
            }
        }

        

        /// <summary>
        /// returns all neighbouring cities within distance around location.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public IEnumerable<City> FindNeighbours(WayPoint location, double distance)
        {
            /* LINQ: pendant zu Java Collection Streams API
             * return:  an output sequence, transformed from input sequence
             * _cities: Enumerable input sequnce (such as List, Array, SQL-DB).
             * city:    element of enumerable input sequence
             * a <= b:  conditional / search filter criteria (Predicate)
             * .Where(lambda): LINQ standard operator
            */
            return _cities.Where(city => city.Location.Distance(location) <= distance);
        }

        public IEnumerable<City> FindNeighboursSorted(WayPoint location, double distance)
        {
            // Chaining query operators: inputcollection.FindAll().OrderBy()
            return _cities.FindAll(city => city.Location.Distance(location) <= distance)
                .OrderBy(city => city.Location.Distance(location));
        }

        /// <summary>
        /// reads a \t deliminated file of cities and returns
        /// the amount of newly added cities.
        /// </summary>
        /// <changelog>
        /// lab04: replaced Reading of Files and ConverttoCity(filename) by Extensions.cs's GetSplittedLines() extension method.
        /// lab05: replaced
        /// foreach(string[] cs in citiesAsStrings) {
        ///    _cities.Add(new City(cs[0].Trim(), ...)
        ///    ++countNewEntries;
        /// }
        /// 
        /// ...with LINQ
        ///    IEnumerable<City> newCities = citiesAsStrings.Select(cs => new City(cs[0].Trim(),...)
        ///    
        /// </changelog>>
        /// <param name="filename"></param>
        /// <returns name="countNewEntries"></returns>
        public int ReadCities(string filename)
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException("File not found");
            }

            int countNewEntries = 0;
            try
            {
                
                using (TextReader reader = new StreamReader(new FileStream(filename, FileMode.Open)))  //ToDo: necessary? : new FileStream(filename, FileMode.Open))
                {
                    IEnumerable<string[]> citiesAsStrings = reader.GetSplittedLines('\t');
                    
                    //IEnumerable<City> newcities:
                    IEnumerable<City> newCities = citiesAsStrings.Select(cs => new City(
                            cs[0].Trim(),
                            cs[1].Trim(), 
                            int.Parse(cs[2]),
                            double.Parse(cs[3], CultureInfo.InvariantCulture),
                            double.Parse(cs[4], CultureInfo.InvariantCulture)
                        ));
                    _cities.AddRange(newCities);
                    countNewEntries = newCities.Count();

                }//Note: using closes the file/stream automatically and also more  
                 //      reliably. No .Close() or .Dispose() necessary.
            } 
            catch (FileNotFoundException ffe)
            {
                //C# doesn't know throws keyword. Use this instead:
                throw new FileNotFoundException(ffe.Message);
            }
            return countNewEntries;
        }

        /// <summary>
        /// converts a String with \t as value delimiter to City attributes.
        /// </summary>
        /// <changelog>
        /// lab04: replaced by Extensions.cs's GetSplittedLines() extension method.
        /// </changelog>>
        /// <param name="fileline"></param>
        /// <returns></returns>
        private static City ConverttoCity(string fileline)
        {
            string[] CityAttributes = fileline.Split('\t'); // single quote = character!
            string name = CityAttributes[0];
            string city = CityAttributes[1];
            int population = int.Parse(CityAttributes[2]);
            /*
             * culture parsing problem solution:
             * Double.Parse(string, CultureInfo:InvarianteCulture)
            */
            CultureInfo culture = CultureInfo.InvariantCulture;
            double latitude = double.Parse(CityAttributes[3], culture);
            double longitude = double.Parse(CityAttributes[4], culture);

            WayPoint location = new WayPoint(name, latitude, longitude);

            //create new City:
            return new City(name, city, population, location);
        }


        /*************************** lab06 *******************************/
        public int GetPopulationOfShortestCityNames()
        {
            return 0;
        }

        /*************************** END of lab06 *******************************/


    }
}
