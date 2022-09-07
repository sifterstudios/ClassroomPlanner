using System;
using UnityEngine;

namespace Sifter.Date
{
    public static class DateRangeForPlanning
    {
        public static DateTime Start;
        public static DateTime End;
        public static void LoadRanges()
        {
            Start = ES3.Load("DateRangeStart", DateTime.Today);
            End = ES3.Load("DateRangeEnd", DateTime.Today);
        }

        public static void SaveRanges()
        {
            ES3.Save("DateRangeStart", Start);
            ES3.Save("DateRangeEnd", End);
        }
    }
}