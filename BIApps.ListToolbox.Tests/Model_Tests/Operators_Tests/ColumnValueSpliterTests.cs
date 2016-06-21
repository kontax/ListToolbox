using BIApps.ListToolbox.Model;
using BIApps.ListToolbox.Model.Operators;
using BIApps.ListToolbox.Tests.Model_Tests.HelperClasses;
using NUnit.Framework;

namespace BIApps.ListToolbox.Tests.Model_Tests.Operators_Tests {

    [TestFixture]
    public class ColumnValueSpliterTests {

        [Test]
        public void TestColumnValueSpliterSplitsFakeListIntoThree() {
            const int expectedTables = 3;
            const int expectedRows1 = 4;
            const int expectedRows2 = 3;
            const int expectedRows3 = 3;
            var listGroup = new UploadedListGroup {
                new UploadedList(new FakeListUploader {SourceName = "GetList1"})
            };

            var splitter = new ColumnValueSplitter(listGroup, "channel");

            var splitLists = splitter.Operate();

            Assert.AreEqual(expectedTables, splitLists.UploadedLists.Count);
            Assert.AreEqual(expectedRows1, splitLists.UploadedLists[0].ListRowCount);
            Assert.AreEqual(expectedRows2, splitLists.UploadedLists[1].ListRowCount);
            Assert.AreEqual(expectedRows3, splitLists.UploadedLists[2].ListRowCount);
        }

        [Test]
        public void TestColumnValueSpliterSplitsLargeListIntoFive() {
            const int expectedTables = 5;
            var listGroup = new UploadedListGroup {
                new UploadedList(new FakeListUploader(20000))
            };

            var splitter = new ColumnValueSplitter(listGroup, "channel");

            var splitLists = splitter.Operate();

            Assert.AreEqual(expectedTables, splitLists.UploadedLists.Count);
        }
    }
}
