using System;
using System.Collections.Generic;

namespace Sifter.Date
{
    public static class ExcludedDates
    {
        public static List<DateTime> Excluded;

        public static void Load()
        {
            Excluded = ES3.Load("ExcludedDates", Excluded);
        }

        public static void Save()
        {
            ES3.Save("ExcludedDates", Excluded);
        }

        public static void Add(DateTime date)
        {
            if (!Excluded.Contains(date)) Excluded.Add(date);
        }

        public static void AddRange(DateTime date, int numberOfDays)
        {
            numberOfDays--; // TODO: Check if the implementation uses zero-based counting or not
            for (var dateRange = 0; dateRange < numberOfDays; dateRange++)
            {
                date = date.AddDays(dateRange);
                if (Excluded.Contains(date)) continue;
                Excluded.Add(date);
            }
        }

        public static void Remove(DateTime date)
        {
            if (Excluded.Contains(date)) Excluded.Remove(date);
        }

        public static void RemoveRange(DateTime date, int numberOfDays)
        {
            numberOfDays--; // TODO: Check if the implementation uses zero-based counting or not
            for (var dateRange = 0; dateRange < numberOfDays; dateRange++)
            {
                date = date.AddDays(dateRange);
                if (!Excluded.Contains(date)) continue;
                Excluded.Remove(date);
            }
        }
    }
}