using System;
using BIApps.ListToolbox.Model.Uploaders;
using NUnit.Framework;

namespace BIApps.ListToolbox.Tests.Model_Tests.Uploaders_Tests {
    [TestFixture]
    public class TextListUploaderTests {

        [Test]
        public void LargeCustIdCsvUploadsCorrectly() {
            const int expectedRowCount = 379393;

            var dir = AppDomain.CurrentDomain.BaseDirectory;
            var upload = dir + "\\..\\..\\TestLists\\large_cust_ids\\test1.csv";
            var uploader = new TextListUploader(upload);
            var dt = uploader.UploadList();

            Assert.AreEqual(expectedRowCount, dt.Rows.Count);
        }

    }
}
