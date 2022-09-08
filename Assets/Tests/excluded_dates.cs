using System;
using NSubstitute;
using NUnit.Framework;
using Sifter.Date;
using UnityEngine;

namespace Tests
{
    public class excluded_dates
    {
        // A Test behaves as an ordinary method
        [Test]
        public void can_add_date_to_list()
        {
            var excludedDates = Substitute.For<IExcludedDates>();
            var now = DateTime.Now;
            Debug.Log(now);
            excludedDates.Add(now, excludedDates.exclusions);
            var exclusions = excludedDates.exclusions;
            Debug.Log(exclusions);
        }
    }
}