using System;
using System.Collections.Generic;

namespace insoden
{
    public static class Lib
    {
        public static string chkDigitAccountNo(string AccountNo)
        {
            int[] iWeight = { 1, 7, 3, 1, 7, 3, 1, 7, 3, 1, 7, 3, 1 };
            int chkDigit = 0;
            for (int i = 0; i < 13; i++)
            {
                chkDigit = chkDigit + iWeight[i] * Convert.ToInt32(AccountNo[i]);
            }
            chkDigit = chkDigit % 10;
            if (chkDigit != 0)
            {
                chkDigit = 10 - chkDigit;
            }
            else
            {
                chkDigit = 0;
            }
            return AccountNo + chkDigit;
        }

        public static IEnumerable<DateTime> GetDateRange(DateTime startingDate, DateTime endingDate)
        {
            if (startingDate > endingDate)
            {
                return null;
            }
            var rv = new List<DateTime>();
            DateTime tmpDate = startingDate;
            do
            {
                rv.Add(tmpDate);
                tmpDate = tmpDate.AddDays(1);
            } while (tmpDate <= endingDate);
            return rv;
        }

        public static string Split_ALong_Line(string str, int maxValue, short i1)
        {
            while (String.CompareOrdinal(str, str.Replace("  ", " ")) != 0)
            {
                str = str.Replace("  ", " ");
            }
            if (str.Length <= maxValue)
            {
                return str;
            }
            int num = str.Substring(0, maxValue).LastIndexOf(" ", StringComparison.Ordinal);
            return (str.Substring(0, num) + " ~V 15 ~H 25 ~t " + str.Substring(num + 1).Trim() + " ");
        }
    }
}