using System.Threading.Tasks;

namespace SpectraAnalysis
{
    public class cl_Visualization
    {
        public decimal[,] baseline_correction_data;
        public decimal max_peak_height;
        cl_Tasks task = new cl_Tasks();

        public cl_Visualization(Guid baseline_id)
        {
            task = new cl_Tasks();
            string query = "select * from MEPhI_disser.dbo.vw_Baseline_correction_data_simulated where baseline_id = '" + baseline_id.ToString() + "' order by retention_time";
            var tb_baseline_correction_data = task.GetData(query);

            int data_width = tb_baseline_correction_data.Columns.Count - 4;
            int data_height = tb_baseline_correction_data.Rows.Count;

            baseline_correction_data = new decimal[data_height, data_width];
            for (int i = 4; i < data_width + 4; i++)
                for (int j = 0; j < data_height; j++)
                    baseline_correction_data[j, i - 4] = decimal.Parse(tb_baseline_correction_data.Rows[j].ItemArray[i].ToString(), System.Globalization.NumberStyles.Float);
        }

        public decimal GetMaxPeakHeight(Guid baseline_id)
        {
            string query = "select max(peak_height) max_peak_height from MEPhI_disser.dbo.Peaks_data where baseline_id = '" + baseline_id.ToString() + "'";
            max_peak_height = int.Parse(task.GetData(query).Rows[0].ItemArray[0].ToString());
            return max_peak_height;
        }
    }
}
