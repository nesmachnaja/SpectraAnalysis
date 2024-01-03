using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UploadFile()
        {
            for (int k = 0; k < Request.Form.Files.Count; k++)
            {
                var file = Request.Form.Files[k];
                //string fileName = file.FileName;
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

                        DataTable raw_spectra = new DataTable();
                        raw_spectra.Columns.Add("spectra_id", typeof(Guid));
                        raw_spectra.Columns.Add("retention_time", typeof(int));

                        for (int i = 1; i < 450; i += 2)
                        {
                            raw_spectra.Columns.Add("column" + i.ToString(), typeof(decimal));
                        }

                        Guid uid = Guid.NewGuid();

                        for (int i = 5; i <= rowNum; i++) 
                        {
                            DataRow row = raw_spectra.NewRow();

                            row[0] = uid;
                            row[1] = Convert.ToInt32(sheet.Cells[i, 1].Value); 

                            for (int j = 2; j < row.ItemArray.Length; j++)
                                row[j] = Convert.ToDecimal(sheet.Cells[i, j].Value);

                            raw_spectra.Rows.Add(row);
                            raw_spectra.AcceptChanges();
                        }
                    }
                }

                try
                {
                    task = new cl_Tasks("exec DWH_Risk.dbo.sp_BIH_SNAP_raw @BIH_SNAP_raw = ", bih_snap);
                }
                catch (Exception exc)
                {
                    logAdapter.InsertRow("cl_Parser_BIH", "parse_BIH_SNAP", "BIH", DateTime.Now, false, exc.Message);
                    Console.WriteLine("Error");
                    Console.WriteLine("Error_desc: " + exc.Message.ToString());
                    ex.Quit();

                    throw;
                }
            }
            return Json(new { message = "success" });
        }

    }
}
