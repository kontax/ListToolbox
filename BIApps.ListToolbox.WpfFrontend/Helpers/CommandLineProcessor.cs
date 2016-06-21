using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace BIApps.ListToolbox.WpfFrontend.Helpers {
    public class CommandLineProcessor {

        /// <summary>
        /// Process the list of arguments given by the command line.
        /// </summary>
        /// <param name="args">The list of command line arguments</param>
        /// <returns>A new <see cref="ProcessedCommand"/> object containing the details necessary</returns>
        public ProcessedCommand Process(IList<string> args) {
            var execMethod = ConvertEnum<Method>(GetArgsFlags(args, "-m"));
            var column = GetArgsFlags(args, "-h");
            var filenames = GetArgsFiles(args).ToList();

            return new ProcessedCommand {
                Method = execMethod,
                Files = filenames,
                Column = column
            };
        }

        /// <summary>
        /// Process the command line arguments to return the method to use on the lists
        /// </summary>
        private static string GetArgsFlags(IEnumerable<string> args, string flag) {
            var method = args.SingleOrDefault(arg => arg.StartsWith(flag));
            if(!string.IsNullOrEmpty(method)) {
                method = method.Replace(flag + " ", "");
            }
            return method;
        }

        /// <summary>
        /// Process the command line arguments to return the list of files to upload
        /// </summary>
        private static IEnumerable<string> GetArgsFiles(IEnumerable<string> args) {
            return from filename in args
                   where filename.StartsWith("-f")
                   select filename.Replace("-f ", "")
                       into file
                       select Path.GetFullPath(file);
        }

        /// <summary>
        /// Converts a specified string to its related enum, based on the description associated with it.
        /// </summary>
        /// <typeparam name="T">The enum to convert to</typeparam>
        /// <param name="value">The string mapping the description to the enum</param>
        /// <returns>The relevant enum</returns>
        private static T ConvertEnum<T>(string value) {

            var enumType = typeof(T);

            // Loop through each value in the enum
            foreach(var item in Enum.GetValues(enumType)) {
                var itemString = item.ToString();
                var field = enumType.GetField(itemString);
                var attribs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);


                if(attribs.Length > 0)
                    itemString = ((DescriptionAttribute)attribs[0]).Description;

                // If we find a description match, return the enum string
                if(itemString == value) return (T)item;
            }

            throw new ArgumentException("Enum specified doesn't match: " + value + "(" + enumType + ")", "value");
        }
    }
}