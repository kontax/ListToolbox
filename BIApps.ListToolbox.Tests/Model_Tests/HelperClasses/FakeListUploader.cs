using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BIApps.ListToolbox.Model.Uploaders;

namespace BIApps.ListToolbox.Tests.Model_Tests.HelperClasses {

    enum Channel {
        Bingo,
        Casino,
        Games,
        LiveCasino,
        Poker,
        Vegas
    }

    class FakeListUploader : IListUploader {
        private readonly int _rowCount;
        public string SourceName { get; set; }

        public FakeListUploader() {
            _rowCount = 0;
            SourceName = "GetList1";
        }

        public FakeListUploader(string listType) {
            SourceName = listType;
        }

        public FakeListUploader(int rowCount) {
            _rowCount = rowCount;
            SourceName = "FixedRowCount";
        }

        public DataTable UploadList() {
            if(SourceName == null) SourceName = "GetList1";

            if (SourceName == "FixedRowCount") return GetFixedRowList();
            if(SourceName == "GetList1") return PopulateList1();
            if(SourceName == "GetList2") return PopulateList2();
            if(SourceName == "GetList3") return PopulateList3();
            if(SourceName == "GetTextList1") return PopulateTextList1();
            if(SourceName == "GetTextList2") return PopulateTextList2();
            if(SourceName == "GetTextList3") return PopulateTextList3();
            if (SourceName == "GetUppercaseHeadings") return PopulateUppercaseHeadingList();
            throw new InvalidOperationException("Set SourceName correctly");
        }

        private DataTable GetFixedRowList() {
            var dt = new DataTable("fake_list_" + _rowCount);

            var dc1 = new DataColumn("ppo_cust_id", typeof(int));
            var dc2 = new DataColumn("channel", typeof(string));
            var dc3 = new DataColumn("stake", typeof(float));

            dt.Columns.AddRange(new[] { dc1, dc2, dc3 });

            for (int i = 0; i < _rowCount; i++) {
                var dr = dt.NewRow();
                dr["ppo_cust_id"] = i;
                dr["channel"] = (Channel) (i%5);
                dr["stake"] = (float) (i%20);
                dt.Rows.Add(dr);
            }

            return dt;
        }

        private static DataTable PopulateList1() {
            var dt = new DataTable("fake_list_1");

            var dc1 = new DataColumn("ppo_cust_id", typeof(int));
            var dc2 = new DataColumn("channel", typeof(string));
            var dc3 = new DataColumn("stake", typeof(float));

            dt.Columns.AddRange(new[] {dc1, dc2, dc3});

            var dr0 = dt.NewRow();
            dr0["ppo_cust_id"] = 0;
            dr0["channel"] = "Bingo";
            dr0["stake"] = 10;
            dt.Rows.Add(dr0);
            var dr1 = dt.NewRow();
            dr1["ppo_cust_id"] = 1;
            dr1["channel"] = "Bingo";
            dr1["stake"] = 10;
            dt.Rows.Add(dr1);
            var dr2 = dt.NewRow();
            dr2["ppo_cust_id"] = 2;
            dr2["channel"] = "Casino";
            dr2["stake"] = 13;
            dt.Rows.Add(dr2);
            var dr3 = dt.NewRow();
            dr3["ppo_cust_id"] = 3;
            dr3["channel"] = "Games";
            dr3["stake"] = 512;
            dt.Rows.Add(dr3);
            var dr4 = dt.NewRow();
            dr4["ppo_cust_id"] = 4;
            dr4["channel"] = "Casino";
            dr4["stake"] = 95;
            dt.Rows.Add(dr4);
            var dr5 = dt.NewRow();
            dr5["ppo_cust_id"] = 5;
            dr5["channel"] = "Casino";
            dr5["stake"] = 12;
            dt.Rows.Add(dr5);
            var dr6 = dt.NewRow();
            dr6["ppo_cust_id"] = 6;
            dr6["channel"] = "Bingo";
            dr6["stake"] = 45;
            dt.Rows.Add(dr6);
            var dr7 = dt.NewRow();
            dr7["ppo_cust_id"] = 7;
            dr7["channel"] = "Games";
            dr7["stake"] = 57;
            dt.Rows.Add(dr7);
            var dr8 = dt.NewRow();
            dr8["ppo_cust_id"] = 8;
            dr8["channel"] = "Games";
            dr8["stake"] = 84;
            dt.Rows.Add(dr8);
            var dr9 = dt.NewRow();
            dr9["ppo_cust_id"] = 9;
            dr9["channel"] = "Bingo";
            dr9["stake"] = 13;
            dt.Rows.Add(dr9);

            return dt;
        }

        private static DataTable PopulateList2() {
            var dt = new DataTable("fake_list_2");

            var dc1 = new DataColumn("ppo_cust_id", typeof(int));
            var dc2 = new DataColumn("channel", typeof(string));
            var dc3 = new DataColumn("stake", typeof(float));

            dt.Columns.AddRange(new[] {dc1, dc2, dc3});

            var dr0 = dt.NewRow();
            dr0["ppo_cust_id"] = 5;
            dr0["channel"] = "Bingo";
            dr0["stake"] = 10;
            dt.Rows.Add(dr0);
            var dr1 = dt.NewRow();
            dr1["ppo_cust_id"] = 6;
            dr1["channel"] = "Bingo";
            dr1["stake"] = 10;
            dt.Rows.Add(dr1);
            var dr2 = dt.NewRow();
            dr2["ppo_cust_id"] = 7;
            dr2["channel"] = "Casino";
            dr2["stake"] = 13;
            dt.Rows.Add(dr2);
            var dr3 = dt.NewRow();
            dr3["ppo_cust_id"] = 8;
            dr3["channel"] = "Games";
            dr3["stake"] = 512;
            dt.Rows.Add(dr3);
            var dr4 = dt.NewRow();
            dr4["ppo_cust_id"] = 9;
            dr4["channel"] = "Casino";
            dr4["stake"] = 95;
            dt.Rows.Add(dr4);
            var dr5 = dt.NewRow();
            dr5["ppo_cust_id"] = 10;
            dr5["channel"] = "Casino";
            dr5["stake"] = 12;
            dt.Rows.Add(dr5);
            var dr6 = dt.NewRow();
            dr6["ppo_cust_id"] = 11;
            dr6["channel"] = "Bingo";
            dr6["stake"] = 45;
            dt.Rows.Add(dr6);
            var dr7 = dt.NewRow();
            dr7["ppo_cust_id"] = 12;
            dr7["channel"] = "Games";
            dr7["stake"] = 57;
            dt.Rows.Add(dr7);
            var dr8 = dt.NewRow();
            dr8["ppo_cust_id"] = 13;
            dr8["channel"] = "Games";
            dr8["stake"] = 84;
            dt.Rows.Add(dr8);
            var dr9 = dt.NewRow();
            dr9["ppo_cust_id"] = 14;
            dr9["channel"] = "Bingo";
            dr9["stake"] = 13;
            dt.Rows.Add(dr9);

            return dt;
        }

        private static DataTable PopulateList3() {
            var dt = new DataTable("fake_list_3");

            var dc1 = new DataColumn("ppo_cust_id", typeof(int));
            var dc2 = new DataColumn("channel", typeof(string));
            var dc3 = new DataColumn("stake", typeof(float));

            dt.Columns.AddRange(new[] { dc1, dc2, dc3 });

            var dr0 = dt.NewRow();
            dr0["ppo_cust_id"] = 8;
            dr0["channel"] = "Bingo";
            dr0["stake"] = 10;
            dt.Rows.Add(dr0);
            var dr1 = dt.NewRow();
            dr1["ppo_cust_id"] = 9;
            dr1["channel"] = "Bingo";
            dr1["stake"] = 10;
            dt.Rows.Add(dr1);
            var dr2 = dt.NewRow();
            dr2["ppo_cust_id"] = 10;
            dr2["channel"] = "Casino";
            dr2["stake"] = 13;
            dt.Rows.Add(dr2);
            var dr3 = dt.NewRow();
            dr3["ppo_cust_id"] = 11;
            dr3["channel"] = "Games";
            dr3["stake"] = 512;
            dt.Rows.Add(dr3);
            var dr4 = dt.NewRow();
            dr4["ppo_cust_id"] = 12;
            dr4["channel"] = "Casino";
            dr4["stake"] = 95;
            dt.Rows.Add(dr4);
            var dr5 = dt.NewRow();
            dr5["ppo_cust_id"] = 13;
            dr5["channel"] = "Casino";
            dr5["stake"] = 12;
            dt.Rows.Add(dr5);
            var dr6 = dt.NewRow();
            dr6["ppo_cust_id"] = 14;
            dr6["channel"] = "Bingo";
            dr6["stake"] = 45;
            dt.Rows.Add(dr6);
            var dr7 = dt.NewRow();
            dr7["ppo_cust_id"] = 15;
            dr7["channel"] = "Games";
            dr7["stake"] = 57;
            dt.Rows.Add(dr7);
            var dr8 = dt.NewRow();
            dr8["ppo_cust_id"] = 16;
            dr8["channel"] = "Games";
            dr8["stake"] = 84;
            dt.Rows.Add(dr8);
            var dr9 = dt.NewRow();
            dr9["ppo_cust_id"] = 17;
            dr9["channel"] = "Bingo";
            dr9["stake"] = 13;
            dt.Rows.Add(dr9);

            return dt;
        }

        private static DataTable PopulateTextList1() {
            var dt = new DataTable("fake_list_1");

            var dc1 = new DataColumn("username", typeof(string));
            var dc2 = new DataColumn("channel", typeof(string));
            var dc3 = new DataColumn("stake", typeof(float));

            dt.Columns.AddRange(new[] { dc1, dc2, dc3 });

            var dr0 = dt.NewRow();
            dr0["username"] = "a";
            dr0["channel"] = "Bingo";
            dr0["stake"] = 10;
            dt.Rows.Add(dr0);
            var dr1 = dt.NewRow();
            dr1["username"] = "b";
            dr1["channel"] = "Bingo";
            dr1["stake"] = 10;
            dt.Rows.Add(dr1);
            var dr2 = dt.NewRow();
            dr2["username"] = "d";
            dr2["channel"] = "Casino";
            dr2["stake"] = 13;
            dt.Rows.Add(dr2);
            var dr3 = dt.NewRow();
            dr3["username"] = "d";
            dr3["channel"] = "Games";
            dr3["stake"] = 512;
            dt.Rows.Add(dr3);
            var dr4 = dt.NewRow();
            dr4["username"] = "e";
            dr4["channel"] = "Casino";
            dr4["stake"] = 95;
            dt.Rows.Add(dr4);
            var dr5 = dt.NewRow();
            dr5["username"] = "f";
            dr5["channel"] = "Casino";
            dr5["stake"] = 12;
            dt.Rows.Add(dr5);
            var dr6 = dt.NewRow();
            dr6["username"] = "g";
            dr6["channel"] = "Bingo";
            dr6["stake"] = 45;
            dt.Rows.Add(dr6);
            var dr7 = dt.NewRow();
            dr7["username"] = "h";
            dr7["channel"] = "Games";
            dr7["stake"] = 57;
            dt.Rows.Add(dr7);
            var dr8 = dt.NewRow();
            dr8["username"] = "i";
            dr8["channel"] = "Games";
            dr8["stake"] = 84;
            dt.Rows.Add(dr8);
            var dr9 = dt.NewRow();
            dr9["username"] = "j";
            dr9["channel"] = "Bingo";
            dr9["stake"] = 13;
            dt.Rows.Add(dr9);

            return dt;
        }

        private static DataTable PopulateTextList2() {
            var dt = new DataTable("fake_list_2");

            var dc1 = new DataColumn("username", typeof(string));
            var dc2 = new DataColumn("channel", typeof(string));
            var dc3 = new DataColumn("stake", typeof(float));

            dt.Columns.AddRange(new[] { dc1, dc2, dc3 });

            var dr0 = dt.NewRow();
            dr0["username"] = "E";
            dr0["channel"] = "Bingo";
            dr0["stake"] = 10;
            dt.Rows.Add(dr0);
            var dr1 = dt.NewRow();
            dr1["username"] = "F";
            dr1["channel"] = "Bingo";
            dr1["stake"] = 10;
            dt.Rows.Add(dr1);
            var dr2 = dt.NewRow();
            dr2["username"] = "G";
            dr2["channel"] = "Casino";
            dr2["stake"] = 13;
            dt.Rows.Add(dr2);
            var dr3 = dt.NewRow();
            dr3["username"] = "H";
            dr3["channel"] = "Games";
            dr3["stake"] = 512;
            dt.Rows.Add(dr3);
            var dr4 = dt.NewRow();
            dr4["username"] = "I";
            dr4["channel"] = "Casino";
            dr4["stake"] = 95;
            dt.Rows.Add(dr4);
            var dr5 = dt.NewRow();
            dr5["username"] = "J";
            dr5["channel"] = "Casino";
            dr5["stake"] = 12;
            dt.Rows.Add(dr5);
            var dr6 = dt.NewRow();
            dr6["username"] = "K";
            dr6["channel"] = "Bingo";
            dr6["stake"] = 45;
            dt.Rows.Add(dr6);
            var dr7 = dt.NewRow();
            dr7["username"] = "L";
            dr7["channel"] = "Games";
            dr7["stake"] = 57;
            dt.Rows.Add(dr7);
            var dr8 = dt.NewRow();
            dr8["username"] = "M";
            dr8["channel"] = "Games";
            dr8["stake"] = 84;
            dt.Rows.Add(dr8);
            var dr9 = dt.NewRow();
            dr9["username"] = "N";
            dr9["channel"] = "Bingo";
            dr9["stake"] = 13;
            dt.Rows.Add(dr9);

            return dt;
        }

        private static DataTable PopulateTextList3() {
            var dt = new DataTable("fake_list_3");

            var dc1 = new DataColumn("username", typeof(string));
            var dc2 = new DataColumn("channel", typeof(string));
            var dc3 = new DataColumn("stake", typeof(float));

            dt.Columns.AddRange(new[] { dc1, dc2, dc3 });

            var dr0 = dt.NewRow();
            dr0["username"] = "h";
            dr0["channel"] = "Bingo";
            dr0["stake"] = 10;
            dt.Rows.Add(dr0);
            var dr1 = dt.NewRow();
            dr1["username"] = "i";
            dr1["channel"] = "Bingo";
            dr1["stake"] = 10;
            dt.Rows.Add(dr1);
            var dr2 = dt.NewRow();
            dr2["username"] = "j";
            dr2["channel"] = "Casino";
            dr2["stake"] = 13;
            dt.Rows.Add(dr2);
            var dr3 = dt.NewRow();
            dr3["username"] = "k";
            dr3["channel"] = "Games";
            dr3["stake"] = 512;
            dt.Rows.Add(dr3);
            var dr4 = dt.NewRow();
            dr4["username"] = "l";
            dr4["channel"] = "Casino";
            dr4["stake"] = 95;
            dt.Rows.Add(dr4);
            var dr5 = dt.NewRow();
            dr5["username"] = "m";
            dr5["channel"] = "Casino";
            dr5["stake"] = 12;
            dt.Rows.Add(dr5);
            var dr6 = dt.NewRow();
            dr6["username"] = "n";
            dr6["channel"] = "Bingo";
            dr6["stake"] = 45;
            dt.Rows.Add(dr6);
            var dr7 = dt.NewRow();
            dr7["username"] = "o";
            dr7["channel"] = "Games";
            dr7["stake"] = 57;
            dt.Rows.Add(dr7);
            var dr8 = dt.NewRow();
            dr8["username"] = "p";
            dr8["channel"] = "Games";
            dr8["stake"] = 84;
            dt.Rows.Add(dr8);
            var dr9 = dt.NewRow();
            dr9["username"] = "q";
            dr9["channel"] = "Bingo";
            dr9["stake"] = 13;
            dt.Rows.Add(dr9);

            return dt;
        }

        private static DataTable PopulateUppercaseHeadingList() {
            var dt = new DataTable("uppercase_heading_list");

            var dc1 = new DataColumn("USERNAME", typeof(string));
            var dc2 = new DataColumn("CHANNEL", typeof(string));
            var dc3 = new DataColumn("STAKE", typeof(float));

            dt.Columns.AddRange(new[] { dc1, dc2, dc3 });

            var dr0 = dt.NewRow();
            dr0["username"] = "h";
            dr0["channel"] = "Bingo";
            dr0["stake"] = 10;
            dt.Rows.Add(dr0);
            var dr1 = dt.NewRow();
            dr1["username"] = "i";
            dr1["channel"] = "Bingo";
            dr1["stake"] = 10;
            dt.Rows.Add(dr1);
            var dr2 = dt.NewRow();
            dr2["username"] = "j";
            dr2["channel"] = "Casino";
            dr2["stake"] = 13;
            dt.Rows.Add(dr2);
            var dr3 = dt.NewRow();
            dr3["username"] = "k";
            dr3["channel"] = "Games";
            dr3["stake"] = 512;
            dt.Rows.Add(dr3);
            var dr4 = dt.NewRow();
            dr4["username"] = "l";
            dr4["channel"] = "Casino";
            dr4["stake"] = 95;
            dt.Rows.Add(dr4);
            var dr5 = dt.NewRow();
            dr5["username"] = "m";
            dr5["channel"] = "Casino";
            dr5["stake"] = 12;
            dt.Rows.Add(dr5);
            var dr6 = dt.NewRow();
            dr6["username"] = "n";
            dr6["channel"] = "Bingo";
            dr6["stake"] = 45;
            dt.Rows.Add(dr6);
            var dr7 = dt.NewRow();
            dr7["username"] = "o";
            dr7["channel"] = "Games";
            dr7["stake"] = 57;
            dt.Rows.Add(dr7);
            var dr8 = dt.NewRow();
            dr8["username"] = "p";
            dr8["channel"] = "Games";
            dr8["stake"] = 84;
            dt.Rows.Add(dr8);
            var dr9 = dt.NewRow();
            dr9["username"] = "q";
            dr9["channel"] = "Bingo";
            dr9["stake"] = 13;
            dt.Rows.Add(dr9);

            return dt;
        }
    }
}
