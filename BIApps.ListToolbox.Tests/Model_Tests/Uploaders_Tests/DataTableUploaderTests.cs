using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BIApps.ListToolbox.Model.Uploaders;
using BIApps.ListToolbox.Tests.Model_Tests.HelperClasses;
using NUnit.Framework;

namespace BIApps.ListToolbox.Tests.Model_Tests.Uploaders_Tests {

    [TestFixture]
    public class DataTableUploaderTests {

        [Test]
        public void TenThousandRowThreeColumnDataTableUploadsCorrectly() {
            const int expectedRowCount = 10000;
            const int expectedColumnCount = 3;

            var dtu = new DataTableUploader(new FakeListUploader(10000).UploadList);
            var dt = dtu.UploadList();


            Assert.AreEqual(expectedRowCount, dt.Rows.Count);
            Assert.AreEqual(expectedColumnCount, dt.Columns.Count);
        }
    }
}
