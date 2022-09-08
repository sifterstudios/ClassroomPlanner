using System;
using System.Collections.Generic;

namespace Sifter.Date
{
    public interface IExcludedDates
    {
        List<DateTime> exclusions { get; set; }
        public void Load();
        public void Save();
        public void Add(DateTime date, List<DateTime> exclusions);
        public void AddRange(DateTime date, List<DateTime> exclusions, int numberOfDays);
        public void Remove(DateTime date, List<DateTime> exclusions);
        public void RemoveRange(DateTime date, List<DateTime> exclusions, int numberOfDays);
    }
}