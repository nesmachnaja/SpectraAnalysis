using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectraAnalysis
{
    class IterativeAverageModel : Model
    {
        private DataTable _iterative_average;
        private DataTable _raw_3d_spectra;
        private DataTable _baselines_3d;
        private List<double> _raw_spectra;
        private List<double> _current_iteration;
        private List<double> _previous_iteration;

        public IterativeAverageModel()
        {
            this.Raw_3d_spectra = new DataTable();
            this.Baselines_3d = new DataTable();
            this.Iterative_average = new DataTable();
            this.Raw_spectra = new List<double>();
            this.Current_iteration = new List<double>();
            this.Previous_iteration = new List<double>();
        }

        public DataTable Raw_3d_spectra
        {
            get => _raw_3d_spectra;
            set
            {
                _raw_3d_spectra = value;
            }
        }

        public DataTable Baselines_3d
        {
            get => _baselines_3d;
            set
            {
                _baselines_3d = value;
            }
        }

        public DataTable Iterative_average
        {
            get => _iterative_average;
            set
            {
                _iterative_average = value;
            }
        }

        public List<double> Raw_spectra
        {
            get => _raw_spectra;
            set
            {
                _raw_spectra = value;
            }
        }

        public List<double> Current_iteration
        {
            get => _current_iteration;
            set
            {
                _current_iteration = value;
            }
        }

        public List<double> Previous_iteration
        {
            get => _previous_iteration;
            set
            {
                _previous_iteration = value;
            }
        }
    }
}
