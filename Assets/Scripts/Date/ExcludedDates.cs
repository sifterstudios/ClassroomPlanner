using System;
using System.Collections.Generic;
using Sifter.Tools;

namespace Sifter.Date
{
    public class ExcludedDates : IExcludedDates
    {
        public static List<DateTime> Excluded = new();
        public const string ED = PersistenceConstants.ExcludedDates;

        public ExcludedDates()
        {
            Excluded = ES3.Load(ED, Excluded);
        }

        public void Save()
        {
            ES3.Save(ED, Excluded);
        }

        public void Add(DateTime date, List<DateTime> exclusions)
        {
            if (!Excluded.Contains(date)) Excluded.Add(date);
            if (exclusions.Contains(date)) exclusions.Add(date);
        }

        public void AddRange(DateTime date, List<DateTime> exclusions, int numberOfDays)
        {
            for (var dateRange = 0; dateRange < numberOfDays; dateRange++) Excluded.Add(date.AddDays(dateRange));
        }

        public void Remove(DateTime date, List<DateTime> exclusions)
        {
            if (Excluded.Contains(date)) Excluded.Remove(date);
        }

        public void RemoveRange(DateTime date, List<DateTime> exclusions, int numberOfDays)
        {
            for (var dateRange = 0; dateRange < numberOfDays; dateRange++) Excluded.Remove(date.AddDays(dateRange));
        }
    }
}