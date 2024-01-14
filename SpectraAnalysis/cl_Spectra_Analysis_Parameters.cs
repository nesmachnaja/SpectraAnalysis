using System.Data;

namespace SpectraAnalysis
{
    public class cl_Spectra_Analysis_Parameters
    {
        public Guid spectra_id { get; set; }
        public int num_of_iterations { get; set; }
        public double threshold { get; set; }
        public Guid baseline_id { get; set; }
    }
}
