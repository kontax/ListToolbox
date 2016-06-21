using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BIApps.ListToolbox.Model.Operators {
    interface IListOperation {

        UploadedListGroup UploadedListGroup { get; }

        UploadedListGroup Operate();
    }
}
