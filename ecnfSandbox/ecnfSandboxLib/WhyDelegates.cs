using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecnfSandboxLib
{
    /// <summary>
    /// Source: https://www.youtube.com/watch?v=rxLNJ8jCN1c
    /// </summary>
    public class WhyDelegates
    {
        //define the delegate (?)

        //The method using the delegate

        //delegateable methods:
        private bool GreaterThanTen(int x)
        {
            return x > 10;
        }

        private bool LessThanTen(int x)
        {
            return x < 10;
        }

        private bool EqualsTen(int x)
        {
            return x == 10;
        }

        private bool IsMultipleOfTen(int x)
        {
            return x % 10 == 0;
        }
    }
}
