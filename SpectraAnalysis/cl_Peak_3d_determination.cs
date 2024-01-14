using System.Data;

namespace SpectraAnalysis
{
    class cl_Peak_3d_determination
    {
        int group_num = 1;
        int start_ascending;
        int end_descending;
        int peak_width;
        //DataTable tb_peaks_data_raw = new DataTable();
        DataTable tb_peaks_data = new DataTable();
        cl_Tasks task = new cl_Tasks();

        public cl_Peak_3d_determination(Guid baseline_id)
        {
            task = new cl_Tasks();
            string query = "select * from MEPhI_disser.dbo.Peaks_data where baseline_id = '" + baseline_id.ToString() + "' order by start_peak, end_peak";
            tb_peaks_data = task.GetData(query);

            //Peaks_dataTableAdapter ad_peaks_data = new Peaks_dataTableAdapter();
            //tb_peaks_data_raw = ad_peaks_data.GetData();
            /*
            for (int j = 0; j < tb_peaks_data_raw.Columns.Count; j++)
                tb_peaks_data.Columns.Add(tb_peaks_data_raw.Columns[j].ColumnName, tb_peaks_data_raw.Columns[j].DataType);
            for (int j = 0; j < tb_peaks_data_raw.Rows.Count; j++)
            {
                DataRow row = tb_peaks_data.NewRow();
                row.BeginEdit();
                row[0] = tb_peaks_data_raw.Rows[j][0];
                row[1] = tb_peaks_data_raw.Rows[j][1];
                row[2] = tb_peaks_data_raw.Rows[j][2];
                row[3] = tb_peaks_data_raw.Rows[j][3];
                row[4] = tb_peaks_data_raw.Rows[j][4];
                row[5] = tb_peaks_data_raw.Rows[j][5];
                row.EndEdit();

                tb_peaks_data.Rows.Add(row);
            }*/

            //DataColumn column = new DataColumn();
            //column.ColumnName = "peak_num";
            //column.DataType = Type.GetType("System.Int32");
            //tb_peaks_data.Columns.Add(column);
            foreach (DataRow row in tb_peaks_data.Rows)
            {
                if (tb_peaks_data.Rows.IndexOf(row) != 0 &&
                    (int.Parse(row.ItemArray[4].ToString()) * 100 / start_ascending > 125
                    || int.Parse(row.ItemArray[5].ToString()) * 100 / end_descending > 125
                    || (int.Parse(row.ItemArray[5].ToString()) - int.Parse(row.ItemArray[4].ToString())) * 100 / end_descending - start_ascending > 150))
                {
                    /*Console.WriteLine(int.Parse(row.ItemArray[2].ToString()) * 100 / start_ascending);
                    Console.WriteLine(int.Parse(row.ItemArray[3].ToString()) * 100 / end_descending);
                    Console.WriteLine(int.Parse(row.ItemArray[4].ToString()) * 100 / peak_width);*/
                    group_num++;
                }

                row.BeginEdit();
                row[8] = group_num;
                row.EndEdit();
                //row.AcceptChanges();

                int.TryParse(row.ItemArray[4].ToString(), out start_ascending);
                int.TryParse(row.ItemArray[5].ToString(), out end_descending);
                //int.TryParse(row.ItemArray[6].ToString(), out peak_width);
            }

            task = new cl_Tasks("exec MEPhI_disser.dbo.sp_Peaks_3d_groupping @Peaks_data = ", tb_peaks_data);

            Console.WriteLine("Groups of peaks are ready");
            //Console.ReadLine();
        }
    }
}
