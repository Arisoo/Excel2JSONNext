using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Excel2JSONCoreLib
{
    internal static class Utils
    {
        //public static Regex reg_identifier = new Regex("_")
        /// <summary>
        /// 변수명으로써 가능한지, 클래스명이라던지
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsValidIdentifier(string name)
        {
            // 첫글자 조사
            string f = new string(name[0], 1);
            if (!Regex.IsMatch(f, @"^[a-zA-Z_]*$"))
            {
                return false;
            }

            return Regex.IsMatch(name, @"^[a-zA-Z0-9_]*$");
        }

        public static bool IsNullOrEmpty(this System.Collections.ICollection collection)
        {
            return collection == null || collection.Count == 0;
        }

        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input[0].ToString().ToUpper() + input.Substring(1);
            }
        }

        public static bool IsFilteredSeet(string Name)
        {
            var dest = Name.ToLower();
            if (dest[0] == '#' ||
                            dest[0] == '$' ||
                            dest.Contains("overview") ||
                            dest.Contains("filter") ||
                            Name.Contains("NO") ||
                            dest.Contains("backup")
                            )
                return true;
            return false;
        }
    }
}
