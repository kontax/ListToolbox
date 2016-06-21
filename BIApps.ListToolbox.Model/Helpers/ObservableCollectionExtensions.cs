using System;
using System.Collections.Generic;

namespace BIApps.ListToolbox.ListHelpers.Helpers {
    public static class ObservableCollectionExtensions {
        public static void AddRange<T>(this ICollection<T> oc, IEnumerable<T> collection) {
            if(collection == null) {
                throw new ArgumentNullException("collection");
            }
            foreach(var item in collection) {
                oc.Add(item);
            }
        } 
    }
}
