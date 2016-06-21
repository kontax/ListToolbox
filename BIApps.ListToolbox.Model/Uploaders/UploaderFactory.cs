using System;
using System.IO;

namespace BIApps.ListToolbox.ListHelpers.Uploaders {

    /// <summary>
    /// Class used to choose the <see cref="IListUploader"/> depending on the extension of the file
    /// </summary>
    public class UploaderFactory {

        /// <summary>
        /// Returns a specific <see cref="IListUploader"/> depending on the filename passed as a parameter.
        /// </summary>
        /// <param name="sourceName">The filename to search</param>
        /// <returns>An <see cref="IListUploader"/></returns>
        public IListUploader GetUploader(string sourceName) {

            var extension = Path.GetExtension(sourceName);
            if(string.IsNullOrEmpty(extension))
                throw new ArgumentException("The filetype extension cannot be found");

            switch(extension.ToLower()) {
                case ".csv":
                case ".txt":
                case ".tab":
                    return new TextListUploader(sourceName);
                case ".xls":
                    return new XlsListUploader(sourceName);
                case ".xlsx":
                    return new XlsxListUploader(sourceName);
                default:
                    throw new ArgumentException("The filetype extension cannot be found");
            }
        }
    }
}
