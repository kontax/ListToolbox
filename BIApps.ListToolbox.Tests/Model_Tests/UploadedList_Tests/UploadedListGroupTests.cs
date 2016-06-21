using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BIApps.ListToolbox.Model;
using BIApps.ListToolbox.Tests.Model_Tests.HelperClasses;
using NUnit.Framework;

namespace BIApps.ListToolbox.Tests.Model_Tests.UploadedList_Tests {

    [TestFixture]
    public class UploadedListGroupTests {

        [Test]
        public void CheckFakeListUploadsFourListsCorrectly() {
            const int expected = 4;

            var fakeList1 = new UploadedList(new FakeListUploader());
            var fakeList2 = new UploadedList(new FakeListUploader());
            var fakeList3 = new UploadedList(new FakeListUploader());
            var fakeList4 = new UploadedList(new FakeListUploader());
            var listGroup = new UploadedListGroup {fakeList1, fakeList2, fakeList3, fakeList4};

            Assert.AreEqual(expected, listGroup.UploadedLists.Count);
        }

        [Test]
        public void CheckListsSwapCorrectly() {
            const int expected1 = 3;
            const int expected2 = 1;

            var fakeList1 = new UploadedList(new FakeListUploader {SourceName = "GetList1"});
            var fakeList2 = new UploadedList(new FakeListUploader { SourceName = "GetList2" });
            var fakeList3 = new UploadedList(new FakeListUploader { SourceName = "GetList3" });
            var fakeList4 = new UploadedList(new FakeListUploader(10000));
            var listGroup = new UploadedListGroup {fakeList1, fakeList2, fakeList3, fakeList4};

            listGroup.Swap(fakeList2, fakeList4);

            var test1 = listGroup.UploadedLists.IndexOf(fakeList2);
            var test2 = listGroup.UploadedLists.IndexOf(fakeList4);

            Assert.AreEqual(expected1, test1);
            Assert.AreEqual(expected2, test2);
        }

        [Test]
        public void CheckExistingListInsertsCorrectly() {
            const int expected1 = 1;
            const int expected2 = 2;

            var fakeList1 = new UploadedList(new FakeListUploader { SourceName = "GetList1" });
            var fakeList2 = new UploadedList(new FakeListUploader { SourceName = "GetList2" });
            var fakeList3 = new UploadedList(new FakeListUploader { SourceName = "GetList3" });
            var fakeList4 = new UploadedList(new FakeListUploader(10000));
            var listGroup = new UploadedListGroup { fakeList1, fakeList2, fakeList3, fakeList4 };

            listGroup.Insert(fakeList2, fakeList4);

            var test1 = listGroup.UploadedLists.IndexOf(fakeList4);
            var test2 = listGroup.UploadedLists.IndexOf(fakeList2);

            Assert.AreEqual(expected1, test1);
            Assert.AreEqual(expected2, test2);
        }

        [Test]
        public void CheckNonExistingListInsertsCorrectly() {
            const int expectedInitial = 1;
            const int expected1 = 2;
            const int expected2 = 1;

            var fakeList1 = new UploadedList(new FakeListUploader { SourceName = "GetList1" });
            var fakeList2 = new UploadedList(new FakeListUploader { SourceName = "GetList2" });
            var fakeList3 = new UploadedList(new FakeListUploader { SourceName = "GetList3" });
            var fakeList4 = new UploadedList(new FakeListUploader(10000));
            var listGroup = new UploadedListGroup { fakeList1, fakeList2, fakeList3 };

            var initial = listGroup.UploadedLists.IndexOf(fakeList2);

            listGroup.Insert(fakeList2, fakeList4);

            var test1 = listGroup.UploadedLists.IndexOf(fakeList2);
            var test2 = listGroup.UploadedLists.IndexOf(fakeList4);

            Assert.AreEqual(expectedInitial, initial);
            Assert.AreEqual(expected1, test1);
            Assert.AreEqual(expected2, test2);
        }
    }
}
