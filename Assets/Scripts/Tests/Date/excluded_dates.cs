using System;
using NSubstitute;
using NUnit.Framework;
using Sifter.Date;
using Sifter.Tools;

namespace Tests
{
    public class excluded_dates
    {
        [Test]
        public void can_add_date_to_list()
        {
            var excludedDates = ExcludedDates.Excluded;
            var now = DateTime.Now;
            ExcludedDates.Add(now);
            Assert.Contains(now, excludedDates);
            ExcludedDates.Excluded.Clear();
        }

        [Test]
        public void can_remove_date_from_list()
        {
            var excludedDates = ExcludedDates.Excluded;
            var now = DateTime.Now;
            var date = DateTime.UnixEpoch;
            ExcludedDates.Add(now);
            ExcludedDates.Add(date);
            Assert.AreEqual(2, ExcludedDates.Excluded.Count);
            ExcludedDates.Remove(now);
            Assert.AreEqual(1, ExcludedDates.Excluded.Count);
            ExcludedDates.Excluded.Clear();
        }

        [Test]
        public void does_not_add_duplicates()
        {
            var excludedDates = ExcludedDates.Excluded;
            var now = DateTime.Now;
            ExcludedDates.Add(now);
            ExcludedDates.Add(now);
            Assert.AreEqual(1, ExcludedDates.Excluded.Count);
            ExcludedDates.Excluded.Clear();
        }

        [Test]
        public void does_not_remove_date_if_not_present()
        {
            var excludedDates = ExcludedDates.Excluded;
            var now = DateTime.Now;
            var date = DateTime.UnixEpoch;
            ExcludedDates.Add(now);

            Assert.AreEqual(1, ExcludedDates.Excluded.Count);
            ExcludedDates.Remove(date);
            Assert.AreEqual(1, ExcludedDates.Excluded.Count);
            ExcludedDates.Excluded.Clear();
        }

        [Test]
        public void can_add_date_range()
        {
            var now = DateTime.Now;
            ExcludedDates.Excluded.Clear();
            ExcludedDates.AddRange(now, 5);
            Assert.AreEqual(5, ExcludedDates.Excluded.Count);
            ExcludedDates.Excluded.Clear();
        }

        [Test]
        public void can_remove_date_range()
        {
            var now = DateTime.Now;
            ExcludedDates.Excluded.Clear();
            ExcludedDates.AddRange(now, 5);
            ExcludedDates.RemoveRange(now, 5);
            Assert.AreEqual(0, ExcludedDates.Excluded.Count);
        }
        
        [Test]
        public void can_load_from_storage()
        {
            var ed = Substitute.For<PersistenceConstants>();
            ExcludedDates.Load();
            Assert.NotNull(ExcludedDates.Excluded);
        }
    }
}