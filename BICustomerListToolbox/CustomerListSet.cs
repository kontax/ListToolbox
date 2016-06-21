using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace BIApps.ListToolbox {

    public class CustomerListSet : DataSet, INotifyCollectionChanged {

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public CustomerListSet() : base() { }


        /// <summary>
        /// Take a group of CustomerLists in a CustomerListSet prioritized by the 
        /// priority flag, and remove values taken from the higher priority tables 
        /// from the lower priority ones.
        /// </summary>
        /// <param name="cls">The CustomerListSet of CustomerLists to dedupe</param>
        /// <param name="dedupeColumn">The column to dedupe by</param>
        /// <param name="caseSensitive">Whether the comparison is case sensitive or not</param>
        /// <returns>A CustomerListSet of deduped CustomerLists</returns>
        public CustomerListSet DeDupeCustomerList(string dedupeColumn, bool caseSensitive) {
            CustomerListSet output = new CustomerListSet();
            DataTable removeTable = new DataTable();
            // Get the type of the column selected
            Type dedupeColumnType = this.Tables[0].Columns[dedupeColumn].DataType;
            removeTable.Columns.Add(dedupeColumn, dedupeColumnType);
            removeTable.DefaultView.Sort = dedupeColumn + " ASC";

            // Loop through the tables in the input CustomerListSet
            for(int i = 0; i < this.Tables.Count; ++i) {
                CustomerList cl = new CustomerList("temp");
                CustomerList outList = new CustomerList("temp");

                // Ensure the highest priority one is picked first
                foreach(CustomerList custList in this.Tables) {
                    if(custList.Priority == i) {
                        cl = custList;
                        break;
                    }
                }

                // Use linq to remove those who have been found already
                var tempList = cl.AsEnumerable();
                var removeList = removeTable.AsEnumerable().Distinct();
                IEnumerable<DataRow> finalList;
                if(caseSensitive) {
                    finalList = from c in tempList
                                join b in removeList
                                    on c.Field<dynamic>(dedupeColumn)
                                    equals b.Field<dynamic>(dedupeColumn)
                                    into j
                                from x in j.DefaultIfEmpty()
                                where x == null
                                select c;
                } else {
                    finalList = from c in tempList
                                join b in removeList
                                    on c.Field<dynamic>(dedupeColumn).ToLower()
                                    equals b.Field<dynamic>(dedupeColumn).ToLower()
                                    into j
                                from x in j.DefaultIfEmpty()
                                where x == null
                                select c;
                }

                DataTable tempDT = finalList.CopyToDataTable();
                foreach(DataRow dr in cl.Rows) {
                    removeTable.Rows.Add(dr[dedupeColumn]);
                }
                outList = CustomerList.DataTableToCustomerList(tempDT, cl.TableName);
                output.Tables.Add(outList);
            }

            return output;
        }


        /// <summary>
        /// Add a new CustomerList to the CustomerListSet and set the priority
        /// according to which order it's been added in.
        /// </summary>
        /// <param name="cl">CustomerList to be added</param>
        public void Add(CustomerList cl) {
            cl.Priority = this.maxPriority() + 1;
            this.Tables.Add(cl);
            if(CollectionChanged != null) {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add));
            }
        }


        /// <summary>
        /// Remove a CustomerList from the CustomerListSet.
        /// </summary>
        /// <param name="cl">The CustomerList to be removed</param>
        public void Remove(CustomerList cl) {
            this.Tables.Remove(cl);
            if(CollectionChanged != null) {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove));
            }
        }

        /// <summary>
        /// Returns the columns that the CustomerLists have in common
        /// </summary>
        /// <returns>List of column headers available to all CustomerLists</returns>
        public List<string> ColumnsInCommon() {
            return (from CustomerList list in Tables
                    from DataColumn dc in list.Columns
                    select new { colName = dc.ColumnName, tableName = dc.Table.TableName }
                        into colNames
                        group colNames.tableName.Count() by colNames.colName
                            into colCount
                            where colCount.Count() == Tables.Count
                            select colCount.Key).ToList();
            /*
            List<string> columnsInCommon = new List<string>();
            Dictionary<string, int> columnCount = new Dictionary<string, int>();

            foreach(CustomerList cl in this.Tables) {
                foreach(DataColumn dc in cl.Columns) {
                    if(!columnCount.ContainsKey(dc.ColumnName)) {
                        columnCount.Add(dc.ColumnName, 1);
                    } else {
                        columnCount[dc.ColumnName]++;
                    }
                }
            }

            foreach(KeyValuePair<string, int> x in columnCount) {
                if(x.Value == this.Tables.Count) {
                    columnsInCommon.Add(x.Key);
                }
            }

            return columnsInCommon;*/
        }


        /// <summary>
        /// Take a set of CustomerLists and merges them all into one.
        /// </summary>
        /// <param name="cls">CustomerListSet of lists to merge</param>
        /// <returns>CustomerList of merged lists</returns>
        public CustomerList MergeLists() {
            CustomerList output = new CustomerList("merged_list");

            foreach(CustomerList cl in this.Tables) {
                output.Merge(cl);
            }

            return output;
        }


        /// <summary>
        /// Find the max priority for all the CustomerLists stored
        /// in the CustomerListSet
        /// </summary>
        /// <returns>Highest priority in the CustomerListSet</returns>
        private int maxPriority() {
            int maxPriority = -1;
            foreach(CustomerList cl in this.Tables) {
                maxPriority = cl.Priority >= maxPriority ? cl.Priority : maxPriority;
            }
            return maxPriority;
        }
    }
}
