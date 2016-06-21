using System.Collections.Generic;

namespace BIApps.ListToolbox.WpfFrontend.Helpers {
    public class ProcessedCommand {

        /// <summary>
        /// The method that the lists will have performed on them.
        /// </summary>
        public Method Method { get; set; }

        /// <summary>
        /// The collection of file names to upload.
        /// </summary>
        public IEnumerable<string> Files { get; set; }

        /// <summary>
        /// The column that is being operated on.
        /// </summary>
        public string Column { get; set; }
    }
}