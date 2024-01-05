using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectraAnalysis.Controllers
{
    class IterativeAverageController
    {
        //Smoothed_dataTableAdapter exp_data_ad = new Smoothed_dataTableAdapter();
        //Raw_dataDataTable exp_data_tb = new Raw_dataDataTable();

        private IterativeAverageModel model = new IterativeAverageModel();
        private int cycle = 1;
        private double threshold;
        private int num_of_iterations;
        private int length;
        private int column_num;
        private string spectra_id;
        private double Sabs_curr = 0;
        private double Sabs_prev = 0;
        private double deltaSabs = 0;
        private cl_Tasks task;
        private Guid baseline_id;

        public void ReadData(cl_Spectra_Analysis_Parameters param)
        {
            //model.Raw_3d_spectra = exp_data_ad.GetData();
            task = new cl_Tasks();
            string query = "select * from MEPhI_disser.dbo.Smoothed_data where spectra_id = '" + param.spectra_id.ToString() + "' and num_of_iterations = " + param.num_of_iterations.ToString() + " order by retention_time";
            model.Raw_3d_spectra = task.GetData(query);

            model.Raw_3d_spectra.Columns.RemoveAt(2);
            model.Raw_3d_spectra.Columns.RemoveAt(2);
            model.Raw_3d_spectra.Columns.RemoveAt(2);

            for (int j = 0; j < model.Raw_3d_spectra.Columns.Count; j++)
                model.Baselines_3d.Columns.Add(model.Raw_3d_spectra.Columns[j].ColumnName, model.Raw_3d_spectra.Columns[j].DataType);

            //model.Baselines_3d = model.Raw_3d_spectra.Clone();

            DataColumn uid_column = new DataColumn("baseline_id");
            model.Baselines_3d.Columns.Add(uid_column);

            spectra_id = model.Raw_3d_spectra.Rows[0].ItemArray[0].ToString();
            model.Raw_3d_spectra.Columns.RemoveAt(0);
            model.Raw_3d_spectra.Columns.RemoveAt(0);

            length = model.Raw_3d_spectra.Rows.Count;

            threshold = param.threshold;
            num_of_iterations = param.num_of_iterations;
        }

        public void AnalyzeCurrentDataSet()
        {
            baseline_id = Guid.NewGuid();

            foreach (DataColumn column in model.Raw_3d_spectra.Columns)
            {
                foreach (DataRow x in model.Raw_3d_spectra.Rows)
                {
                    //Console.WriteLine(x.IsNull(column) == true ? 0 : (double)x.ItemArray[model.Raw_3d_spectra.Columns.IndexOf(column)]);
                    model.Raw_spectra.Add(x.IsNull(column) == true ? 0 : (double)x.ItemArray[model.Raw_3d_spectra.Columns.IndexOf(column)]);
                }

                column_num = model.Raw_3d_spectra.Columns.IndexOf(column) + 2;

                GetBaseline();

                Sabs_curr = 0;
                Sabs_prev = 0;
                deltaSabs = 0;
                cycle = 1;

                SetCurrentColumn();
                model.Raw_spectra.Clear();

                //SetResults();
            }
        }

        private void GetBaseline()
        {
            model.Previous_iteration.Clear();
            model.Current_iteration.Clear();

            while (cycle == 1 || deltaSabs > threshold)
            {
                GetAnotherCycle();
                Sabs_prev = Sabs_curr;
                Sabs_curr = FindSabs(model.Current_iteration);

                deltaSabs = (Sabs_curr - Sabs_prev) / Sabs_curr;

                //Console.WriteLine("column_id = " + column_num + ", cycle " + cycle.ToString() + ", Sabs = " + deltaSabs + ", x1 = " + model.Current_iteration[1]); //FindSabs(model.Current_iteration).ToString());
                cycle++;
                model.Previous_iteration.Clear();
                model.Previous_iteration.AddRange(model.Current_iteration);
                model.Current_iteration.Clear();
            }

            //Console.WriteLine("column_id = " + column_num + ", cycle " + cycle.ToString() + ", Sabs = " + deltaSabs);
        }

        private void GetAnotherCycle()
        {
            if (cycle == 1) CalculateAnotherIteration(model.Raw_spectra, model.Current_iteration);
            else CalculateAnotherIteration(model.Previous_iteration, model.Current_iteration);

        }

        private double FindSabs(List<double> calculated_data)
        {
            double Sabs = 0;
            for (int i = 0; i < length; i++)
                Sabs = Sabs + Math.Abs(model.Raw_spectra[i] - calculated_data[i]); //model.Current_iteration[i]);
            return Sabs;
        }


        private void CalculateAnotherIteration(List<double> prev, List<double> curr)
        {
            for (int i = 0; i < length; i++)
            {
                if (i == 0)
                    curr.Add(prev[i]);
                else if (i == length - 1)
                    curr.Add(prev[i]);
                else
                    curr.Add(FindMinimal((prev[i - 1] + prev[i + 1]) / 2, prev[i]));
            }
        }

        private double FindMinimal(double xi_xi_2, double xi_1)
        {
            return Math.Min(xi_xi_2, xi_1);
        }

        private void SetCurrentColumn()
        {
            if (column_num != 2)
                for (int i = 0; i < model.Previous_iteration.Count; i++)
                {
                    model.Baselines_3d.Rows[i].BeginEdit();
                    model.Baselines_3d.Rows[i][column_num] = model.Previous_iteration[i];
                    model.Baselines_3d.Rows[i].EndEdit();
                    model.Baselines_3d.AcceptChanges();
                    //.ItemArray[column_num] = model.Previous_iteration[i]
                }
            else
                for (int i = 0; i < model.Previous_iteration.Count; i++)
                {
                    DataRow row = model.Baselines_3d.NewRow();
                    row[0] = spectra_id;
                    row[1] = i;
                    row[column_num] = model.Previous_iteration[i];
                    row[model.Baselines_3d.Columns.Count - 1] = baseline_id.ToString().ToUpper();
                    model.Baselines_3d.Rows.Add(row);
                }
        }

        public void SetResults()
        {
            string query = "insert into MEPhI_disser.dbo.List_of_baselines (spectra_id,baseline_id,baseline_dt,method,threshold) values ('" + spectra_id.ToUpper() + "','" + baseline_id.ToString().ToUpper() + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "','Iterative average'," + threshold.ToString().Replace(",", ".") + ")";
            task = new cl_Tasks(query);
            task = new cl_Tasks("exec MEPhI_disser.dbo.sp_Baselines_data @Baselines_data = ", model.Baselines_3d);
            task = new cl_Tasks("exec MEPhI_disser.dbo.sp_Baseline_correction @spectra_id = '" + spectra_id.ToString() + "', @baseline_id = '" + baseline_id.ToString() + "', @num_of_iterations = " + num_of_iterations.ToString());
        }
    }
}
