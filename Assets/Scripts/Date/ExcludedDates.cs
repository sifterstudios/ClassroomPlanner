using System;
using System.Collections.Generic;

namespace Sifter.Date
{
    public class ExcludedDates : IExcludedDates
    {
        public List<DateTime> Excluded = new();

        public ExcludedDates()
        {
            exclusions = Excluded;
        }

        public List<DateTime> exclusions { get; set; }

        public void Load()
        {
            Excluded = ES3.Load("ExcludedDates", Excluded);
        }

        public void Save()
        {
            ES3.Save("ExcludedDates", Excluded);
        }

        public void Add(DateTime date, List<DateTime> exclusions)
        {
            if (!Excluded.Contains(date)) Excluded.Add(date);
            if (exclusions.Contains(date)) exclusions.Add(date);
        }

        public void AddRange(DateTime date, List<DateTime> exclusions, int numberOfDays)
        {
            numberOfDays--; // TODO: Check if the implementation uses zero-based counting or not
            for (var dateRange = 0; dateRange < numberOfDays; dateRange++)
            {
                date = date.AddDays(dateRange);
                if (Excluded.Contains(date)) continue;
                Excluded.Add(date);
            }
        }

        public void Remove(DateTime date, List<DateTime> exclusions)
        {
            if (Excluded.Contains(date)) Excluded.Remove(date);
        }

        public void RemoveRange(DateTime date, List<DateTime> exclusions, int numberOfDays)
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