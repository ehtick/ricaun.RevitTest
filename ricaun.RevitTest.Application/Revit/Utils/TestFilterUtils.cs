using NamedPipeWrapper.Json;
using System;
using System.Collections.Generic;

namespace ricaun.RevitTest.Application.Revit.Utils
{
    public static class TestFilterUtils
    {
        public static string[] GetFilters(string testFilter)
        {
            //return testFilter.SplitComma();
            return testFilter.Split(',');
            //try
            //{
            //    return testFilter.JsonDeserialize<string[]>();
            //}
            //catch (Exception)
            //{
            //    return testFilter.Split(',');
            //}
        }

        /// <summary>
        /// Splits a string into an array of substrings based on the ',' separator.
        /// Considers quoted strings enclosed in double quotes "" as a single substring.
        /// </summary>
        /// <param name="input">The input string to be split into an array of substrings.</param>
        /// <returns>An array of substrings that were separated by the ',' separator.</returns>
        private static string[] SplitComma(this string input)
        {
            return input.SpecialSplit(',');
        }
        /// <summary>
        /// Splits a string into an array of substrings based on the <paramref name="separator"/> separator. 
        /// Considers quoted strings enclosed in double quotes "" as a single substring.
        /// </summary>
        /// <param name="input">The input string to be split into an array of substrings.</param>
        /// <returns>An array of substrings that were separated by the <paramref name="separator"/> separator.</returns>
        private static string[] SpecialSplit(this string input, char separator)
        {
            List<string> outputList = new List<string>();
            bool inQuotes = false;
            int start = 0;

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '"' && (i == 0 || input[i - 1] != '\\'))
                {
                    inQuotes = !inQuotes;
                }
                else if (input[i] == separator && !inQuotes)
                {
                    outputList.Add(input.Substring(start, i - start));
                    start = i + 1;
                }
            }

            if (start < input.Length)
            {
                outputList.Add(input.Substring(start));
            }

            return outputList.ToArray();
        }

    }
}
