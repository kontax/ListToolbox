using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BIApps.ListToolbox.Model;
using BIApps.ListToolbox.Model.Helpers;
using BIApps.ListToolbox.Model.Operators;
using BIApps.ListToolbox.Tests.Model_Tests.HelperClasses;
using NUnit.Framework;

namespace BIApps.ListToolbox.Tests.Model_Tests.Operators_Tests {

    [TestFixture]
    public class MergerTests {

        [Test]
        public void CheckTwoListsWithSameColumnsGetMerged() {
            const int expectedRows = 20;
            const int expectedColumns = 3;
            var list1 = new UploadedList(new FakeListUploader { SourceName = "GetList1" });
            var list2 = new UploadedList(new FakeListUploader { SourceName = "GetList2" });
            var listGroup = new UploadedListGroup { list1, list2 };

            var merger = new Merger(listGroup);

            var mergedLists = merger.Operate();

            Assert.AreEqual(expectedRows, mergedLists[0].ListRowCount);
            Assert.AreEqual(expectedColumns, mergedLists[0].ListDetails.Columns.Count);
        }

        [Test]
        public void CheckThreeListsWithSameColumnsGetMerged() {
            const int expectedRows = 30;
            const int expectedColumns = 3;
            var list1 = new UploadedList(new FakeListUploader { SourceName = "GetList1" });
            var list2 = new UploadedList(new FakeListUploader { SourceName = "GetList2" });
            var list3 = new UploadedList(new FakeListUploader { SourceName = "GetList3" });
            var listGroup = new UploadedListGroup { list1, list2, list3 };

            var merger = new Merger(listGroup);

            var mergedLists = merger.Operate();

            Assert.AreEqual(expectedRows, mergedLists[0].ListRowCount);
            Assert.AreEqual(expectedColumns, mergedLists[0].ListDetails.Columns.Count);
        }

        [Test]
        public void CheckTwoListsWithDifferentColumnsGetMerged() {
            const int expectedRows = 20;
            const int expectedColumns = 4;
            var list1 = new UploadedList(new FakeListUploader { SourceName = "GetList1" });
            var list2 = new UploadedList(new FakeListUploader { SourceName = "GetTextList1" });
            var listGroup = new UploadedListGroup { list1, list2 };

            var merger = new Merger(listGroup);

            var mergedLists = merger.Operate();

            Assert.AreEqual(expectedRows, mergedLists[0].ListRowCount);
            Assert.AreEqual(expectedColumns, mergedLists[0].ListDetails.Columns.Count);
        }

        [Test]
        public void CheckThreeListsWithDifferentColumnsGetMerged() {
            const int expectedRows = 30;
            const int expectedColumns = 4;
            var list1 = new UploadedList(new FakeListUploader { SourceName = "GetList1" });
            var list2 = new UploadedList(new FakeListUploader { SourceName = "GetTextList2" });
            var list3 = new UploadedList(new FakeListUploader { SourceName = "GetList3" });
            var listGroup = new UploadedListGroup { list1, list2, list3 };

            var merger = new Merger(listGroup);

            var mergedLists = merger.Operate();

            Assert.AreEqual(expectedRows, mergedLists[0].ListRowCount);
            Assert.AreEqual(expectedColumns, mergedLists[0].ListDetails.Columns.Count);
        }

        [Test]
        public void CheckListsWithSameColumnsOfDifferentCaseGetMerged() {
            const int expectedRows = 20;
            const int expectedColumns = 3;
            var list1 = new UploadedList(new FakeListUploader { SourceName = "GetTextList1" });
            var list2 = new UploadedList(new FakeListUploader { SourceName = "GetUppercaseHeadings" });
            var listGroup = new UploadedListGroup { list1, list2 };

            var merger = new Merger(listGroup);

            var mergedLists = merger.Operate();

            Assert.AreEqual(expectedRows, mergedLists[0].ListRowCount);
            Assert.AreEqual(expectedColumns, mergedLists[0].ListDetails.Columns.Count);
        }
    }
}
