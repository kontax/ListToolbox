using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIApps.ListToolbox.Tests.Model_Tests.HelperClasses {
    class FakeUploadedList {
        DataTable ListDetails { get; set; }
        string ListName { get; set; }
        int ListRowCount { get; set; }
    }
}
