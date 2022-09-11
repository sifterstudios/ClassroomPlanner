using System;
using System.Collections.Generic;
using Sifter.Tools;

namespace Sifter.Date
{
    public static class ExcludedDates
    {
        public const string ED = PersistenceConstants.ExcludedDates;
        public static List<DateTime> Excluded;


        public static void Save()
        {
            ES3.Save(ED, Excluded);
        }

        public static void Load()
        {
            Excluded = ES3.Load(ED, new List<DateTime>());
        }

        public static void Add(DateTime date)
        {
            if (!Excluded.Contains(date)) Excluded.Add(date);
        }

        public static void AddRange(DateTime date, int numberOfDays)
        {
            for (var dateRange = 0; dateRange < numberOfDays; dateRange++) Excluded.Add(date.AddDays(dateRange));
        }

        public static void Remove(DateTime date)
        {
            if (Excluded.Contains(date)) Excluded.Remove(date);
        }

        public static void RemoveRange(DateTime date, int numberOfDays)
        {
            for (var dateRange = 0; dateRange < numberOfDays; dateRange++) Excluded.Remove(date.AddDays(dateRange));
        }
    }
}