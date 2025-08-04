using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Collections.Generic;
using TreeStore.Models.CustomModels;
using TreeStore.Models.Entities;
using TreeStore.Services;

namespace TreeStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExportController : ControllerBase
    {
        private readonly IOrderServices _ordersService;

        public ExportController(IOrderServices ordersService)
        {
            _ordersService = ordersService;
        }
        [HttpGet("export")]
        public async Task<IActionResult> Index()
        {
            // Set the license context for EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;


            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("SampleData");
                worksheet.Cells[1, 1].Value = "STT";
                worksheet.Cells[1, 2].Value = "Tên Khách Hàng";
                worksheet.Cells[1, 3].Value = "Địa Chỉ";
                worksheet.Cells[1, 4].Value = "Tổng Tiền";
                worksheet.Cells[1, 5].Value = "Ngày Đặt Đơn";
                ResultCustomModel<List<GetListOrderSPResult>> rs = await _ordersService.GetListOrderAsync();
                List<GetListOrderSPResult> lstOrder = rs.Data;
                for (int i = 0; i < lstOrder.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = i + 1;
                    worksheet.Cells[i + 2, 2].Value = lstOrder[i].FullName;
                    worksheet.Cells[i + 2, 3].Value = lstOrder[i].Address;
                    worksheet.Cells[i + 2, 4].Value = lstOrder[i].TotalAmount;
                    worksheet.Cells[i + 2, 5].Value = lstOrder[i].CreateOn;
                    worksheet.Cells[i + 2, 5].Style.Numberformat.Format = "dd/MM/yyyy HH:mm:ss";
                }

                var fileContent = package.GetAsByteArray();
                return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Export.xlsx");
            }
        }
    }
}
