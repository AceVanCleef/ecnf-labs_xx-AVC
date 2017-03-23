using System;
using System.Collections.Generic;
using System.IO; // <- for TextReader
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fhnw.Ecnf.RoutePlanner.RoutePlannerLib.Util
{
    /// <summary>
    /// static class => no instances, no inheritance (no children classes)
    /// </summary>
    public static class Extensions
    {

        /// <summary>
        /// reads a file and returns a list<string[]> containing each line as string[].
        /// 
        /// Note:
        /// - Extension method of TextReader.cs.
        /// - one string[] = one FileLine
        /// - List<string[]> = all FileLines
        /// - delimiter [COMP] = Begrenzungssymbol
        /// </summary>
        /// <param name="tr">this TextReader</param>
        /// <param name="delimiter">the delimiter character</param>
        /// <returns></returns>
        public static List<String[]> GetSplittedLines(this TextReader tr, char delimiter)
        {
            List<String[]> AllLines = new List<string[]>();
            while (tr.Peek() >= 0) // <- Are there more lines to read?
            {
                //read a line
                string _FileLine = tr.ReadLine();

                //split it to string[]
                string[] SingleLine = _FileLine.Split(delimiter);

                //add string[] to List<String[]>
                AllLines.Add(SingleLine);

            }

            return AllLines;
        }
    }
}
