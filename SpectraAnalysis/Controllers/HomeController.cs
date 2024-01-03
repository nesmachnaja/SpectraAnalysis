﻿using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UploadFile()
        {
            DataTable raw_spectra = new DataTable();
            DataTable list_of_spectrum = new DataTable();
            
            //for (int k = 0; k < Request.Form.Files.Count; k++)
            //{
            var file = Request.Form.Files[0];
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

                    raw_spectra.Columns.Add("spectra_id", typeof(Guid));
                    raw_spectra.Columns.Add("retention_time", typeof(int));

                    list_of_spectrum.Columns.Add("spectra_dt", typeof(DateTime));
                    list_of_spectrum.Columns.Add("spectra_id", typeof(Guid));
                    list_of_spectrum.Columns.Add("spectra_name", typeof(string));

                    for (int i = 1; i < 450; i += 2)
                    {
                        raw_spectra.Columns.Add("column" + i.ToString(), typeof(decimal));
                    }

                    Guid uid = Guid.NewGuid();

                    DataRow row_los = list_of_spectrum.NewRow();
                    row_los[0] = DateTime.Now; 
                    row_los[1] = uid;
                    row_los[2] = Request.Form["spectraName"].ToString();

                    list_of_spectrum.Rows.Add(row_los);
                    list_of_spectrum.AcceptChanges();


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
            //}

            try
            {
                task = new cl_Tasks("exec MEPhI_disser.dbo.sp_List_of_spectrum @List_of_spectrum = ", list_of_spectrum);
                task = new cl_Tasks("exec MEPhI_disser.dbo.sp_Raw_data @Raw_data = ", raw_spectra);
                return Json(new { message = "success" });
            }
            catch (Exception exc)
            {
                Console.WriteLine("Error");
                Console.WriteLine("Error_desc: " + exc.Message.ToString());

                return Json(new { message = "failed" });
                throw;
            }
        }

    }
}
