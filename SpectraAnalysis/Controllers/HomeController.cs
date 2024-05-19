using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using SpectraAnalysis.Models;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace SpectraAnalysis.Controllers
{
    public class HomeController : Controller
    {
        HomeModel model = new HomeModel();
        cl_Tasks task;
        //Guid param.spectra_id = Guid.Parse("2D008327-BDBB-4415-A0F2-10021792328E");
        cl_Spectra_Analysis_Parameters param = new cl_Spectra_Analysis_Parameters();

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UploadFile()
        {
            //for (int k = 0; k < Request.Form.Files.Count; k++)
            //{

            //param = new cl_Spectra_Analysis_Parameters();

            if (Request.Form.Files.Count > 0)
            {
                try
                {
                    DataTable raw_spectra = new DataTable();
                    DataTable list_of_spectrum = new DataTable();

                    var file = Request.Form.Files[0];
                    string fileName = file.FileName;
                    //string mimeType = file.ContentType;

                    string directory = Directory.GetCurrentDirectory();
                    var filePath = Path.Combine(directory, "Uploads", file.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);

                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        using (ExcelPackage package = new ExcelPackage(stream))
                        {
                            //for (int sheet_cnt = 0; sheet_cnt < package.Workbook.Worksheets.Count; sheet_cnt++)
                            //{
                            ExcelWorksheet sheet = package.Workbook.Worksheets[0];
                            int columnNum = sheet.Columns.Count();
                            int rowNum = sheet.Rows.Count();

                            raw_spectra.Columns.Add("spectra_id", typeof(Guid));
                            raw_spectra.Columns.Add("retention_time", typeof(int));

                            list_of_spectrum.Columns.Add("spectra_dt", typeof(DateTime));
                            list_of_spectrum.Columns.Add("spectra_id", typeof(Guid));
                            list_of_spectrum.Columns.Add("spectra_name", typeof(string));
                            list_of_spectrum.Columns.Add("file_name", typeof(string));

                            for (int i = 1; i < 450; i += 2)
                            {
                                raw_spectra.Columns.Add("column" + i.ToString(), typeof(decimal));
                            }

                            param.spectra_id = Guid.NewGuid();

                            DataRow row_los = list_of_spectrum.NewRow();
                            row_los[0] = DateTime.Now;
                            row_los[1] = param.spectra_id;
                            row_los[2] = Request.Form["spectraName"].ToString();
                            row_los[3] = fileName;

                            list_of_spectrum.Rows.Add(row_los);
                            list_of_spectrum.AcceptChanges();


                            for (int i = 5; i <= rowNum; i++)
                            {
                                DataRow row = raw_spectra.NewRow();

                                row[0] = param.spectra_id;
                                row[1] = Convert.ToInt32(sheet.Cells[i, 1].Value);

                                for (int j = 2; j < row.ItemArray.Length; j++)
                                    row[j] = Convert.ToDecimal(sheet.Cells[i, j].Value);

                                raw_spectra.Rows.Add(row);
                                raw_spectra.AcceptChanges();
                            }
                        }
                    }
                    //}

                    try
                    {
                        task = new cl_Tasks("exec MEPhI_disser.dbo.sp_List_of_spectrum @List_of_spectrum = ", list_of_spectrum);
                        task = new cl_Tasks("exec MEPhI_disser.dbo.sp_Raw_data @Raw_data = ", raw_spectra);
                        return Json(new { message = "success", spectra_id = param.spectra_id.ToString() });
                    }
                    catch (Exception exc)
                    {
                        //Console.WriteLine("Error");
                        //Console.WriteLine("Error_desc: " + exc.Message.ToString());

                        return Json(new { message = exc.Message.ToString() });
                        //throw;
                    }
                }
                catch (Exception exc)
                {
                    return Json(new { message = exc.Message.ToString() });
                }
            }
            else return Json(new { message = "there is no file" });
        }

        [HttpPost]
        public IActionResult SmoothingByWaveletHaar()
        {
            //param = new cl_Spectra_Analysis_Parameters();
            param.num_of_iterations = int.Parse(Request.Form["numOfIterations"]);
            param.spectra_id = Guid.Parse(Request.Form["spectraId"]);
            try
            {
                task = new cl_Tasks("exec MEPhI_disser.dbo.sp_Smoothing_by_wavelet_haar @spectra_id = '" + param.spectra_id + "', @num_of_iterations = " + param.num_of_iterations.ToString());
                return Json(new { message = "success" });
            }
            catch (Exception exc)
            {
                return Json(new { message = exc.Message.ToString() });
            }
        }

        [HttpPost]
        public IActionResult BaselineCorrection()
        {
            //return Json(new { message = "success", baseline_id = "E6C7F7D4-EB27-4F5D-B46E-6CEBE87E9560" }); //to comment

            try
            {
                param.num_of_iterations = int.Parse(Request.Form["numOfIterations"]);
                param.spectra_id = Guid.Parse(Request.Form["spectraId"]);
                param.threshold = double.Parse(Request.Form["threshold"].ToString().Replace(".", ","));

                IterativeAverageController iacontroller = new IterativeAverageController();
                iacontroller.ReadData(param);
                iacontroller.AnalyzeCurrentDataSet();
                param.baseline_id = iacontroller.SetResults();

                //task = new cl_Tasks("exec MEPhI_disser.dbo.sp_Smoothing_by_wavelet_haar @spectra_id = '" + param.spectra_id + "', @num_of_iterations = " + Request.Form["numOfIterations"]);
                return Json(new { message = "success", baseline_id = param.baseline_id });
            }
            catch (Exception exc)
            {
                return Json(new { message = exc.Message.ToString() });
            }
        }

        [HttpPost]
        public IActionResult SimulateAndDenoise()
        {
            param.num_of_iterations = int.Parse(Request.Form["numOfIterations"]);
            param.spectra_id = Guid.Parse(Request.Form["spectraId"]);
            param.baseline_id = Guid.Parse(Request.Form["baselineId"]);
            param.threshold = double.Parse(Request.Form["threshold"].ToString().Replace(".", ","));

            try
            {
                task = new cl_Tasks("exec MEPhI_disser.dbo.sp_Peak_detection @spectra_id = '" + param.spectra_id + "', @baseline_id = '" + param.baseline_id.ToString() + "';" +
                    " exec MEPhI_disser.dbo.sp_Peak_2d_area_calculation @baseline_id = '" + param.baseline_id.ToString() + "'");
                return Json(new { message = "success" });
            }
            catch (Exception exc)
            {
                return Json(new { message = exc.Message.ToString() });
            }
        }

        [HttpPost]
        public IActionResult DetectPeaks()
        {
            param.num_of_iterations = int.Parse(Request.Form["numOfIterations"]);
            param.spectra_id = Guid.Parse(Request.Form["spectraId"]);
            param.baseline_id = Guid.Parse(Request.Form["baselineId"]);
            param.threshold = double.Parse(Request.Form["threshold"].ToString().Replace(".", ","));

            try
            {
                cl_Peak_3d_determination peak_3d = new cl_Peak_3d_determination(param.baseline_id);
                return Json(new { message = "success" });
            }
            catch (Exception exc)
            {
                return Json(new { message = exc.Message.ToString() });
            }
        }

        [HttpGet]
        public IActionResult VisualizeSpectra()
        {
            Guid baseline_id = Guid.Parse("E6C7F7D4-EB27-4F5D-B46E-6CEBE87E9560");
            cl_Visualization visualization = new cl_Visualization(baseline_id);

            decimal max_peak_height = visualization.GetMaxPeakHeight(baseline_id);

            VisualizationModel response = new VisualizationModel
            {
                baseline_correction_data = Json(JsonConvert.SerializeObject(visualization.baseline_correction_data, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        })),
                max_peak_height = Json(JsonConvert.SerializeObject(max_peak_height, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        }))
            };

            return Ok(response);

            /*return Json(JsonConvert.SerializeObject(visualization.baseline_correction_data, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        }));*/
        }

    }
}
