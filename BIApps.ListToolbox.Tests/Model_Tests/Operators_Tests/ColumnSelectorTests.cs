using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BIApps.ListToolbox.Model;
using BIApps.ListToolbox.Model.Operators;
using BIApps.ListToolbox.Tests.Model_Tests.HelperClasses;
using NUnit.Framework;

namespace BIApps.ListToolbox.Tests.Model_Tests.Operators_Tests {

    [TestFixture]
    public class ColumnSelectorTests {

        [Test]
        public void ColumnSelectorOutputsOneColumnFromThree() {
            const int expectedColumns = 1;
            const int expectedRows = 10;
            var list = new UploadedListGroup();
            list.UploadedLists.Add(new UploadedList(new FakeListUploader {SourceName = "GetList1"}));

            var selector = new ColumnSelector(list, new List<string> {"ppo_cust_id"});

            var columns = selector.Operate();

            foreach (var col in columns) {
                Assert.AreEqual(expectedColumns, col.ListDetails.Columns.Count);
                Assert.AreEqual(expectedRows, col.ListDetails.Rows.Count);
            }
        }

        [Test]
        public void ColumnSelectorOutputsTwoColumnsFromThree() {
            const int expectedColumns = 2;
            const int expectedRows = 10;
            var list = new UploadedListGroup();
            list.UploadedLists.Add(new UploadedList(new FakeListUploader {SourceName = "GetList1"}));

            var selector = new ColumnSelector(list, new List<string> {"ppo_cust_id", "channel"});

            var columns = selector.Operate();

            foreach (var col in columns) {
                Assert.AreEqual(expectedColumns, col.ListDetails.Columns.Count);
                Assert.AreEqual(expectedRows, col.ListDetails.Rows.Count);
            }
        }
    }
}
