using System;
using Sifter.Tools;

namespace Sifter.Date
{
    public static class DateRangeForPlanning
    {
        public static DateTime Start;
        public static DateTime End;

        public static void LoadRanges()
        {
            Start = ES3.Load(PersistenceConstants.DateRangeStart, DateTime.Today);
            End = ES3.Load(PersistenceConstants.DateRangeEnd, DateTime.Today.AddDays(7));
        }

        public static void SaveRanges()
        {
            ES3.Save(PersistenceConstants.DateRangeStart, Start);
            ES3.Save(PersistenceConstants.DateRangeEnd, End);
        }
    }
}