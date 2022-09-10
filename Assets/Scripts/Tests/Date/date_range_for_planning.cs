using NUnit.Framework;
using Sifter.Date;

namespace Tests.Date
{
    public class date_range_for_planning
    {
        [Test]
        public void can_save_and_load_ranges_after_starting_from_no_date()
        {
            var start = DateRangeForPlanning.Start;
            var end = DateRangeForPlanning.End;
            var wasStartEmpty = start == end;
            DateRangeForPlanning.SaveRanges();
            DateRangeForPlanning.LoadRanges();
            Assert.IsNotNull(DateRangeForPlanning.Start);
            Assert.IsNotNull(DateRangeForPlanning.End);
            Assert.IsTrue(wasStartEmpty);
        }
    }
}