using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BIApps.ListToolbox.Model;
using BIApps.ListToolbox.Model.Helpers;
using BIApps.ListToolbox.Model.Operators;
using BIApps.ListToolbox.Tests.Model_Tests.HelperClasses;
using NUnit.Framework;

namespace BIApps.ListToolbox.Tests.Model_Tests.Operators_Tests {

    [TestFixture]
    public class DeduperTests {

        [Test]
        public void CheckCaseSensitiveNumericDedupeBetweenTwoLists() {
            const int expectedRows1 = 10;
            const int expectedRows2 = 5;
            var list1 = new UploadedList(new FakeListUploader { SourceName = "GetList1" });
            var list2 = new UploadedList(new FakeListUploader { SourceName = "GetList2" });
            var listGroup = new UploadedListGroup {list1, list2};

            var deduper = new Deduper(listGroup, "ppo_cust_id", Case.Sensitive);

            var dedupedLists = deduper.Operate();

            Assert.AreEqual(expectedRows1, dedupedLists.UploadedLists[0].ListRowCount);
            Assert.AreEqual(expectedRows2, dedupedLists.UploadedLists[1].ListRowCount);
        }

        [Test]
        public void CheckCaseInsensitiveNumericDedupeBetweenTwoLists() {
            const int expectedRows1 = 10;
            const int expectedRows2 = 5;
            var list1 = new UploadedList(new FakeListUploader { SourceName = "GetList1" });
            var list2 = new UploadedList(new FakeListUploader { SourceName = "GetList2" });
            var listGroup = new UploadedListGroup { list1, list2 };

            var deduper = new Deduper(listGroup, "ppo_cust_id", Case.Insensitive);

            var dedupedLists = deduper.Operate();

            Assert.AreEqual(expectedRows1, dedupedLists.UploadedLists[0].ListRowCount);
            Assert.AreEqual(expectedRows2, dedupedLists.UploadedLists[1].ListRowCount);
        }

        [Test]
        public void CheckCaseSensitiveNumericDedupeBetweenThreeLists() {
            const int expectedRows1 = 10;
            const int expectedRows2 = 5;
            const int expectedRows3 = 3;
            var list1 = new UploadedList(new FakeListUploader { SourceName = "GetList1" });
            var list2 = new UploadedList(new FakeListUploader { SourceName = "GetList2" });
            var list3 = new UploadedList(new FakeListUploader { SourceName = "GetList3" });
            var listGroup = new UploadedListGroup {list1, list2, list3};

            var deduper = new Deduper(listGroup, "ppo_cust_id", Case.Sensitive);

            var dedupedLists = deduper.Operate();

            Assert.AreEqual(expectedRows1, dedupedLists.UploadedLists[0].ListRowCount);
            Assert.AreEqual(expectedRows2, dedupedLists.UploadedLists[1].ListRowCount);
            Assert.AreEqual(expectedRows3, dedupedLists.UploadedLists[2].ListRowCount);
        }

        [Test]
        public void CheckCaseInsensitiveNumericDedupeBetweenThreeLists() {
            const int expectedRows1 = 10;
            const int expectedRows2 = 5;
            const int expectedRows3 = 3;
            var list1 = new UploadedList(new FakeListUploader { SourceName = "GetList1" });
            var list2 = new UploadedList(new FakeListUploader { SourceName = "GetList2" });
            var list3 = new UploadedList(new FakeListUploader { SourceName = "GetList3" });
            var listGroup = new UploadedListGroup { list1, list2, list3 };

            var deduper = new Deduper(listGroup, "ppo_cust_id", Case.Insensitive);

            var dedupedLists = deduper.Operate();

            Assert.AreEqual(expectedRows1, dedupedLists.UploadedLists[0].ListRowCount);
            Assert.AreEqual(expectedRows2, dedupedLists.UploadedLists[1].ListRowCount);
            Assert.AreEqual(expectedRows3, dedupedLists.UploadedLists[2].ListRowCount);
        }

        [Test]
        public void CheckCaseSensitiveNumericDedupeBetweenThreeListsWhenListsAreMoved() {
            const int expectedRows1 = 10;
            const int expectedRows2 = 3;
            const int expectedRows3 = 5;
            var list1 = new UploadedList(new FakeListUploader { SourceName = "GetList1" });
            var list2 = new UploadedList(new FakeListUploader { SourceName = "GetList2" });
            var list3 = new UploadedList(new FakeListUploader { SourceName = "GetList3" });
            var listGroup = new UploadedListGroup {list1, list2, list3};

            listGroup.Swap(list1, list3);

            var deduper = new Deduper(listGroup, "ppo_cust_id", Case.Sensitive);

            var dedupedLists = deduper.Operate();

            Assert.AreEqual(expectedRows1, dedupedLists.UploadedLists[0].ListRowCount);
            Assert.AreEqual(expectedRows2, dedupedLists.UploadedLists[1].ListRowCount);
            Assert.AreEqual(expectedRows3, dedupedLists.UploadedLists[2].ListRowCount);
        }

        [Test]
        public void CheckCaseInsensitiveNumericDedupeBetweenThreeListsWhenListsAreMoved() {
            const int expectedRows1 = 10;
            const int expectedRows2 = 3;
            const int expectedRows3 = 5;
            var list1 = new UploadedList(new FakeListUploader { SourceName = "GetList1" });
            var list2 = new UploadedList(new FakeListUploader { SourceName = "GetList2" });
            var list3 = new UploadedList(new FakeListUploader { SourceName = "GetList3" });
            var listGroup = new UploadedListGroup { list1, list2, list3 };

            listGroup.Swap(list1, list3);

            var deduper = new Deduper(listGroup, "ppo_cust_id", Case.Insensitive);

            var dedupedLists = deduper.Operate();

            Assert.AreEqual(expectedRows1, dedupedLists.UploadedLists[0].ListRowCount);
            Assert.AreEqual(expectedRows2, dedupedLists.UploadedLists[1].ListRowCount);
            Assert.AreEqual(expectedRows3, dedupedLists.UploadedLists[2].ListRowCount);
        }

        [Test]
        public void CheckCaseSensitiveTextDedupeBetweenTwoLists() {
            const int expectedRows1 = 10;
            const int expectedRows2 = 10;
            var list1 = new UploadedList(new FakeListUploader { SourceName = "GetTextList1" });
            var list2 = new UploadedList(new FakeListUploader { SourceName = "GetTextList2" });
            var listGroup = new UploadedListGroup { list1, list2 };

            var deduper = new Deduper(listGroup, "username", Case.Sensitive);

            var dedupedLists = deduper.Operate();

            Assert.AreEqual(expectedRows1, dedupedLists.UploadedLists[0].ListRowCount);
            Assert.AreEqual(expectedRows2, dedupedLists.UploadedLists[1].ListRowCount);
        }

        [Test]
        public void CheckCaseInsensitiveTextDedupeBetweenTwoLists() {
            const int expectedRows1 = 10;
            const int expectedRows2 = 4;
            var list1 = new UploadedList(new FakeListUploader { SourceName = "GetTextList1" });
            var list2 = new UploadedList(new FakeListUploader { SourceName = "GetTextList2" });
            var listGroup = new UploadedListGroup { list1, list2 };

            var deduper = new Deduper(listGroup, "username", Case.Insensitive);

            var dedupedLists = deduper.Operate();

            Assert.AreEqual(expectedRows1, dedupedLists.UploadedLists[0].ListRowCount);
            Assert.AreEqual(expectedRows2, dedupedLists.UploadedLists[1].ListRowCount);
        }

        [Test]
        public void CheckCaseSensitiveTextDedupeBetweenThreeLists() {
            const int expectedRows1 = 10;
            const int expectedRows2 = 10;
            const int expectedRows3 = 7;
            var list1 = new UploadedList(new FakeListUploader { SourceName = "GetTextList1" });
            var list2 = new UploadedList(new FakeListUploader { SourceName = "GetTextList2" });
            var list3 = new UploadedList(new FakeListUploader { SourceName = "GetTextList3" });
            var listGroup = new UploadedListGroup { list1, list2, list3 };

            var deduper = new Deduper(listGroup, "username", Case.Sensitive);

            var dedupedLists = deduper.Operate();

            Assert.AreEqual(expectedRows1, dedupedLists.UploadedLists[0].ListRowCount);
            Assert.AreEqual(expectedRows2, dedupedLists.UploadedLists[1].ListRowCount);
            Assert.AreEqual(expectedRows3, dedupedLists.UploadedLists[2].ListRowCount);
        }

        [Test]
        public void CheckCaseInsensitiveTextDedupeBetweenThreeLists() {
            const int expectedRows1 = 10;
            const int expectedRows2 = 4;
            const int expectedRows3 = 3;
            var list1 = new UploadedList(new FakeListUploader { SourceName = "GetTextList1" });
            var list2 = new UploadedList(new FakeListUploader { SourceName = "GetTextList2" });
            var list3 = new UploadedList(new FakeListUploader { SourceName = "GetTextList3" });
            var listGroup = new UploadedListGroup { list1, list2, list3 };

            var deduper = new Deduper(listGroup, "username", Case.Insensitive);

            var dedupedLists = deduper.Operate();

            Assert.AreEqual(expectedRows1, dedupedLists.UploadedLists[0].ListRowCount);
            Assert.AreEqual(expectedRows2, dedupedLists.UploadedLists[1].ListRowCount);
            Assert.AreEqual(expectedRows3, dedupedLists.UploadedLists[2].ListRowCount);
        }

        [Test]
        public void CheckCaseSensitiveTextDedupeBetweenThreeListsWhenListsAreMoved() {
            const int expectedRows1 = 10;
            const int expectedRows2 = 10;
            const int expectedRows3 = 7;
            var list1 = new UploadedList(new FakeListUploader { SourceName = "GetTextList1" });
            var list2 = new UploadedList(new FakeListUploader { SourceName = "GetTextList2" });
            var list3 = new UploadedList(new FakeListUploader { SourceName = "GetTextList3" });
            var listGroup = new UploadedListGroup { list1, list2, list3 };

            listGroup.Swap(list1, list3);

            var deduper = new Deduper(listGroup, "username", Case.Sensitive);

            var dedupedLists = deduper.Operate();

            Assert.AreEqual(expectedRows1, dedupedLists.UploadedLists[0].ListRowCount);
            Assert.AreEqual(expectedRows2, dedupedLists.UploadedLists[1].ListRowCount);
            Assert.AreEqual(expectedRows3, dedupedLists.UploadedLists[2].ListRowCount);
        }

        [Test]
        public void CheckCaseInsensitiveTextDedupeBetweenThreeListsWhenListsAreMoved() {
            const int expectedRows1 = 10;
            const int expectedRows2 = 3;
            const int expectedRows3 = 4;
            var list1 = new UploadedList(new FakeListUploader { SourceName = "GetTextList1" });
            var list2 = new UploadedList(new FakeListUploader { SourceName = "GetTextList2" });
            var list3 = new UploadedList(new FakeListUploader { SourceName = "GetTextList3" });
            var listGroup = new UploadedListGroup { list1, list2, list3 };

            listGroup.Swap(list1, list3);

            var deduper = new Deduper(listGroup, "username", Case.Insensitive);

            var dedupedLists = deduper.Operate();

            Assert.AreEqual(expectedRows1, dedupedLists.UploadedLists[0].ListRowCount);
            Assert.AreEqual(expectedRows2, dedupedLists.UploadedLists[1].ListRowCount);
            Assert.AreEqual(expectedRows3, dedupedLists.UploadedLists[2].ListRowCount);
        }

        [Test]
        public void EnsureOutputWithNoRowsStillReturnsEmptyDataTableCaseSensitiveNumeric() {
            const int expectedRows1 = 10;
            const int expectedRows2 = 0;
            var list1 = new UploadedList(new FakeListUploader { SourceName = "GetList1" });
            var list2 = new UploadedList(new FakeListUploader { SourceName = "GetList1" });
            var listGroup = new UploadedListGroup { list1, list2 };
            
            var deduper = new Deduper(listGroup, "ppo_cust_id", Case.Sensitive);
            var dedupedLists = deduper.Operate();

            Assert.AreEqual(expectedRows1, dedupedLists.UploadedLists[0].ListRowCount);
            Assert.AreEqual(expectedRows2, dedupedLists.UploadedLists[1].ListRowCount);
        }

        [Test]
        public void EnsureOutputWithNoRowsStillReturnsEmptyDataTableCaseInsensitiveNumeric() {
            const int expectedRows1 = 10;
            const int expectedRows2 = 0;
            var list1 = new UploadedList(new FakeListUploader { SourceName = "GetList1" });
            var list2 = new UploadedList(new FakeListUploader { SourceName = "GetList1" });
            var listGroup = new UploadedListGroup { list1, list2 };

            var deduper = new Deduper(listGroup, "ppo_cust_id", Case.Insensitive);
            var dedupedLists = deduper.Operate();

            Assert.AreEqual(expectedRows1, dedupedLists.UploadedLists[0].ListRowCount);
            Assert.AreEqual(expectedRows2, dedupedLists.UploadedLists[1].ListRowCount);
        }

        [Test]
        public void EnsureOutputWithNoRowsStillReturnsEmptyDataTableCaseSensitiveText() {
            const int expectedRows1 = 10;
            const int expectedRows2 = 0;
            var list1 = new UploadedList(new FakeListUploader { SourceName = "GetTextList1" });
            var list2 = new UploadedList(new FakeListUploader { SourceName = "GetTextList1" });
            var listGroup = new UploadedListGroup { list1, list2 };

            var deduper = new Deduper(listGroup, "username", Case.Sensitive);
            var dedupedLists = deduper.Operate();

            Assert.AreEqual(expectedRows1, dedupedLists.UploadedLists[0].ListRowCount);
            Assert.AreEqual(expectedRows2, dedupedLists.UploadedLists[1].ListRowCount);
        }

        [Test]
        public void EnsureOutputWithNoRowsStillReturnsEmptyDataTableCaseInsensitiveTextc() {
            const int expectedRows1 = 10;
            const int expectedRows2 = 0;
            var list1 = new UploadedList(new FakeListUploader { SourceName = "GetTextList1" });
            var list2 = new UploadedList(new FakeListUploader { SourceName = "GetTextList1" });
            var listGroup = new UploadedListGroup { list1, list2 };

            var deduper = new Deduper(listGroup, "username", Case.Insensitive);
            var dedupedLists = deduper.Operate();

            Assert.AreEqual(expectedRows1, dedupedLists.UploadedLists[0].ListRowCount);
            Assert.AreEqual(expectedRows2, dedupedLists.UploadedLists[1].ListRowCount);
        }
    }
}
