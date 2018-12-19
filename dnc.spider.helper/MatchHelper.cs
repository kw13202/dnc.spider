using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace dnc.spider.helper
{
    public class MatchHelper
    {
        private static readonly Regex numReg = new Regex("\\d+\\.?\\d*");

        public static decimal getDecimalFirstOrDefault(string text)
        {
            if (numReg.IsMatch(text))
            {
                Match match = numReg.Match(text);
                string result = match.Value;
                return Convert.ToDecimal(result);
            }
            else
            {
                return 0;
            }
            
        }

    }
}
