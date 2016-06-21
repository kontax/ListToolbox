using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BIApps.ListToolbox.Model;
using BIApps.ListToolbox.Model.Operators;
using BIApps.ListToolbox.Tests.Model_Tests.HelperClasses;
using NUnit.Framework;

namespace BIApps.ListToolbox.Tests.Model_Tests.Operators_Tests {

    [TestFixture]
    public class RowCountSplitterTests {

        [Test]
        public void TenRowListSplitsToTwoFiveRowLists() {
            const int expectedTables = 2;
            const int expectedRows1 = 5;
            const int expectedRows2 = 5;
            var listGroup = new UploadedListGroup {
                new UploadedList(new FakeListUploader {SourceName = "GetList1"})
            };

            var splitter = new RowCountSplitter(listGroup, 5);

            var splitLists = splitter.Operate();

            Assert.AreEqual(expectedTables, splitLists.UploadedLists.Count);
            Assert.AreEqual(expectedRows1, splitLists.UploadedLists[0].ListRowCount);
            Assert.AreEqual(expectedRows2, splitLists.UploadedLists[1].ListRowCount);
        }

        [Test]
        public void TenRowListSplitsToThreeFourRowLists() {
            const int expectedTables = 3;
            const int expectedRows1 = 4;
            const int expectedRows2 = 4;
            const int expectedRows3 = 2;
            var listGroup = new UploadedListGroup {
                new UploadedList(new FakeListUploader {SourceName = "GetList1"})
            };

            var splitter = new RowCountSplitter(listGroup, 4);

            var splitLists = splitter.Operate();

            Assert.AreEqual(expectedTables, splitLists.UploadedLists.Count);
            Assert.AreEqual(expectedRows1, splitLists.UploadedLists[0].ListRowCount);
            Assert.AreEqual(expectedRows2, splitLists.UploadedLists[1].ListRowCount);
            Assert.AreEqual(expectedRows3, splitLists.UploadedLists[2].ListRowCount);
        }

        [Test]
        public void HundredThousandRowListSplitsToTen10000RowLists() {
            const int expectedTables = 10;
            const int expectedRows = 10000;
            var listGroup = new UploadedListGroup {
                new UploadedList(new FakeListUploader(100000))
            };

            var splitter = new RowCountSplitter(listGroup, 10000);

            var splitLists = splitter.Operate();

            Assert.AreEqual(expectedTables, splitLists.UploadedLists.Count);
            foreach (var table in splitLists.UploadedLists) {
                Assert.AreEqual(expectedRows, table.ListRowCount);
            }
        }

        [Test]
        public void HundredThousandRowListSplitsToFive23000RowLists() {
            const int expectedTables = 5;
            const int expectedRows1 = 23000;
            const int expectedRows2 = 23000;
            const int expectedRows3 = 23000;
            const int expectedRows4 = 23000;
            const int expectedRows5 = 8000;
            var listGroup = new UploadedListGroup {
                new UploadedList(new FakeListUploader(100000))
            };

            var splitter = new RowCountSplitter(listGroup, 23000);

            var splitLists = splitter.Operate();

            foreach(var l in splitLists.UploadedLists) {
                Debug.WriteLine(l.ListDetails.TableName);
                Debug.WriteLine(l.ListRowCount);
            }
            Assert.AreEqual(expectedTables, splitLists.UploadedLists.Count);
            Assert.AreEqual(expectedRows1, splitLists.UploadedLists[0].ListRowCount);
            Assert.AreEqual(expectedRows2, splitLists.UploadedLists[1].ListRowCount);
            Assert.AreEqual(expectedRows3, splitLists.UploadedLists[2].ListRowCount);
            Assert.AreEqual(expectedRows4, splitLists.UploadedLists[3].ListRowCount);
            Assert.AreEqual(expectedRows5, splitLists.UploadedLists[4].ListRowCount);
        }
    }
}
