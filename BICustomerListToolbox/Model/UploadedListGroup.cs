using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;

namespace BIApps.ListToolbox.Model {

    /// <summary>
    /// A collection of <see cref="UploadedList"/> objects.
    /// </summary>
    /// <remarks>
    /// This is used to hold a collection of <see cref="UploadedList"/> objects which usually either 
    /// contain lists uploaded by the user, or those which have had operations performed on them. It 
    /// implements the <see cref="INotifyCollectionChanged"/> interface to raise an event whenever a 
    /// list has been added or removed.
    /// TODO: Add an enumerator to ListGroup
    /// </remarks>
    public class UploadedListGroup : INotifyCollectionChanged, IEnumerable<UploadedList> {

        #region Properties & Fields

        /// <summary>
        /// A collection of <see cref="UploadedList"/> objects.
        /// </summary>
        public List<UploadedList> UploadedLists { get; set; }

        /// <summary>
        /// The indexer for the collection. Index is used in this collection for priority in deduping.
        /// </summary>
        /// <param name="index">The index of the <see cref="UploadedList"/></param>
        /// <returns>A specific <see cref="UploadedList"/> as per the index specified</returns>
        public UploadedList this[int index] {
            get { return UploadedLists[index]; }
            set {
                UploadedLists[index] = value;
                CollectionChanged(UploadedLists, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move));
                CollectionChanged(ColumnsInCommon, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move));
            }
        }

        /// <summary>
        /// The list of columns that each list in the collection have in common.
        /// </summary>
        public List<string> ColumnsInCommon {
            get { return GetColumnsInCommon(); }
        }

        #endregion

        public UploadedListGroup() {
            UploadedLists = new List<UploadedList>();
        }

        #region Public Methods

        /// <summary>
        /// Add a new <see cref="UploadedList"/> to the UploadedListGroup.
        /// </summary>
        /// <param name="list"><see cref="UploadedList"/> to be added to the collection</param>
        public void Add(UploadedList list) {
            UploadedLists.Add(list);
            if(CollectionChanged != null) {
                CollectionChanged(UploadedLists, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add));
                CollectionChanged(ColumnsInCommon, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add));
            }
        }

        /// <summary>
        /// Remove an <see cref="UploadedList"/> from the UploadedListGroup.
        /// </summary>
        /// <param name="list"><see cref="UploadedList"/> to be removed from the collection</param>
        public void Remove(UploadedList list) {
            UploadedLists.Remove(list);
            if(CollectionChanged != null) {
                CollectionChanged(UploadedLists, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove));
                CollectionChanged(ColumnsInCommon, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove));
            }
        }

        /// <summary>
        /// Remove all <see cref="UploadedList"/> objects from this group.
        /// </summary>
        public void RemoveAll() {
            foreach(var list in UploadedLists) {
                Remove(list);
            }
        }

        /// <summary>
        /// Swap the index position of two <see cref="UploadedList"/> objects within th current 
        /// <see cref="UploadedListGroup"/>.
        /// </summary>
        /// <param name="list1">The first <see cref="UploadedList"/> to swap</param>
        /// <param name="list2">The <see cref="UploadedList"/> to switch position with list1</param>
        public void Swap(UploadedList list1, UploadedList list2) {
            if (!(UploadedLists.Contains(list1) || UploadedLists.Contains(list2)))
                throw new InvalidOperationException(
                    "One of the uploaded lists is not contained within this collection. Ensure " +
                    "that the list has been properly uploaded and try again. If in doubt, just " +
                    "Clear and re-Upload the lists.");

            var firstIndex = UploadedLists.IndexOf(list1);
            UploadedLists.Remove(list1);
            UploadedLists.Insert(UploadedLists.IndexOf(list2), list1);
            UploadedLists.Remove(list2);
            UploadedLists.Insert(firstIndex, list2);
        }

        /// <summary>
        /// Insert an <see cref="UploadedList"/> at a specific place in the group. This can be applied to 
        /// lists already within the group.
        /// </summary>
        /// <param name="index">The 0 based index to insert the list to</param>
        /// <param name="list">The list to be inserted at the index</param>
        public void Insert(int index, UploadedList list) {
            if (UploadedLists.Contains(list)) UploadedLists.Remove(list);
            UploadedLists.Insert(index, list);
        }

        /// <summary>
        /// Insert an <see cref="UploadedList"/> before the location of an <see cref="UploadedList"/>
        /// already contained in the collection. This can also 
        /// </summary>
        /// <param name="currentList">The <see cref="UploadedList"/> already contained in the collection
        /// which the second list is going to be inserted in front of</param>
        /// <param name="listToMove">The <see cref="UploadedList"/> to insert into the collection</param>
        public void Insert(UploadedList currentList, UploadedList listToMove) {
            if(!(UploadedLists.Contains(currentList)))
                throw new InvalidOperationException(
                    "The current uploaded list is not contained within this collection. Ensure " +
                    "that the list has been properly uploaded and try again. If in doubt, just " +
                    "Clear and re-Upload the lists.");

            if(UploadedLists.Contains(listToMove)) UploadedLists.Remove(listToMove);
            UploadedLists.Insert(UploadedLists.IndexOf(currentList), listToMove);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get a list of columns that each list in the collection have in common.
        /// </summary>
        /// <returns>A List of strings of columns that are contained in each table in the collection</returns>
        private List<String> GetColumnsInCommon() {
            return (from list in UploadedLists
                    from DataColumn column in list.ListDetails.Columns
                    select new { colName = column.ColumnName, tableName = column.Table.TableName }
                        into c
                        group c.tableName.Count() by c.colName
                            into colCount
                            where colCount.Count() == UploadedLists.Count
                            select colCount.Key).ToList();
        }

        #endregion

        #region INotifyCollectionChanged Property Members
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        #endregion

        public IEnumerator<UploadedList> GetEnumerator() {
            foreach (var list in UploadedLists) {
                if (list == null) break;
                yield return list;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
