        //public async Task<IActionResult> ExportKTXH(PagingParam param)
        //{
        //    try
        //    {
        //        var temp = await _exportKtxhServices.ExportToExcelKTXH(param, new PagingModel<KhenThuongKTXHModel>());
        //        var list = temp;
        //        using (var workbook = new XLWorkbook())
        //        {
        //            string sheetName = "DanhSachKhenThuongKTXH";
        //            var worksheet = workbook.Worksheets.Add(sheetName);
        //            var currentRow = 1;
        //            var STT = 0;
        //            worksheet.Cell(currentRow, (int) ECOLKTXH.STT).Value = "STT";
        //            worksheet.Cell(currentRow, (int) ECOLKTXH.DanhXung).Value = "Danh xưng";
        //            worksheet.Cell(currentRow, (int) ECOLKTXH.HoVaTen).Value = "Tên cá nhân / đơn vị";
        //            worksheet.Cell(currentRow, (int) ECOLKTXH.ChucVu).Value = "Chức vụ";
        //            worksheet.Cell(currentRow, (int) ECOLKTXH.DonVi).Value = "Đơn vị công tác";
        //            worksheet.Cell(currentRow, (int) ECOLKTXH.HinhThucKT).Value = "Hình thức khen thưởng";
        //            worksheet.Cell(currentRow, (int) ECOLKTXH.QuyetDinh).Value = "Quyết định";
        //            worksheet.Cell(currentRow, (int) ECOLKTXH.NgayKy).Value = "Ngày ký quyết định";
        //            worksheet.Cell(currentRow, (int) ECOLKTXH.LoaiKT).Value = "Loại khen thưởng";
        //            worksheet.Cell(currentRow, (int) ECOLKTXH.MucKT).Value = "Danh mục khen thưởng";
        //            worksheet.Cell(currentRow, (int) ECOLKTXH.ThanhTich).Value = "Thành tích";


        //            worksheet.Row(1).Style.Font.Bold = true;

        //            worksheet.Row(1).Style.Font.FontColor = XLColor.White;

        //            worksheet.Range(currentRow, (int) ECOLKTXH.STT, currentRow, (int) ECOLKTXH.NgayKy)
        //                .Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

        //            worksheet.Column((int) ECOLKTXH.NgayKy).Style.DateFormat.Format = "dd/MM/yyyy";

        //            worksheet.Range(currentRow, (int) ECOLKTXH.STT, currentRow, (int) ECOLKTXH.NgayKy).Style.Alignment
        //                .SetHorizontal(XLAlignmentHorizontalValues.Center);

        //            worksheet.Rows().AdjustToContents();
        //            worksheet.Columns().AdjustToContents();
        //            worksheet.Columns().Style.Font.FontName = "Times New Roman";
        //            worksheet.Column((int) ECOLKTXH.HoVaTen).Width = 50;
        //            worksheet.Column((int) ECOLKTXH.ChucVu).Width = 32;
        //            worksheet.Column((int) ECOLKTXH.DonVi).Width = 50;
        //            worksheet.Column((int) ECOLKTXH.QuyetDinh).Width = 18;
        //            worksheet.Column((int) ECOLKTXH.STT).Style.Alignment
        //                .SetHorizontal(XLAlignmentHorizontalValues.Center);
        //            worksheet.Column((int) ECOLKTXH.NgayKy).Style.Alignment
        //                .SetHorizontal(XLAlignmentHorizontalValues.Center);
        //            worksheet.Column((int) ECOLKTXH.ThanhTich).Style.Alignment.WrapText = true;
        //            worksheet.Column((int) ECOLKTXH.ThanhTich).Width = 50;
        //            worksheet.Rows().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        //            worksheet.Column((int) ECOLKTXH.DanhXung).Style.Alignment
        //                .SetHorizontal(XLAlignmentHorizontalValues.Center);
        //            worksheet.Column((int) ECOLKTXH.LoaiKT).Style.Alignment
        //                .SetHorizontal(XLAlignmentHorizontalValues.Center);
        //            worksheet.Column((int) ECOLKTXH.MucKT).Style.Alignment
        //                .SetHorizontal(XLAlignmentHorizontalValues.Center);


        //            foreach (var it in list)
        //            {
        //                currentRow++;
        //                STT++;
        //                worksheet.Cell(currentRow, (int) ECOLKTXH.STT).Value = STT;
        //                worksheet.Cell(currentRow, (int) ECOLKTXH.DanhXung).Value =
        //                    (it.DanhXung != null) ? it.DanhXung.Name : "";
        //                worksheet.Cell(currentRow, (int) ECOLKTXH.HoVaTen).Value =
        //                    (it.HoVaTen != null) ? it.HoVaTen : "";
        //                worksheet.Cell(currentRow, (int) ECOLKTXH.ChucVu).Value =
        //                    (it.ChucVu != null) ? it.ChucVu.Name : "";
        //                worksheet.Cell(currentRow, (int) ECOLKTXH.DonVi).Value =
        //                    (it.DonVi != null) ? it.DonVi.Ten : it.DonViNgoaiHeThong;
        //                worksheet.Cell(currentRow, (int) ECOLKTXH.HinhThucKT).Value =
        //                    (it.HinhThucKT != null) ? it.HinhThucKT.Name : "";
        //                worksheet.Cell(currentRow, (int) ECOLKTXH.QuyetDinh).Value =
        //                    (it.QuyetDinh != null) ? it.QuyetDinh.SoQuyetDinh : "";
        //                worksheet.Cell(currentRow, (int) ECOLKTXH.NgayKy).Value =
        //                    (it.QuyetDinh != null) ? it.QuyetDinh.ShowNgayKy : "";
        //                worksheet.Cell(currentRow, (int) ECOLKTXH.LoaiKT).Value =
        //                    (it.PhanLoaiKT != null) ? it.PhanLoaiKT.Name : "";
        //                worksheet.Cell(currentRow, (int) ECOLKTXH.MucKT).Value =
        //                    (it.DanhMucKT != null) ? it.DanhMucKT.Name : "";
        //                worksheet.Cell(currentRow, (int) ECOLKTXH.ThanhTich).Value =
        //                    (it.ThanhTich != null) ? it.ThanhTich : "";
        //            }

        //            for (int i = 1; i <= list.Count + 1; i++)
        //            {
        //                for (int j = 1; j <= worksheet.Columns().Count(); j++)
        //                {
        //                    worksheet.Cell(i, j).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //                }
        //            }

        //            using (var stream = new MemoryStream())
        //            {
        //                workbook.SaveAs(stream);
        //                var content = stream.ToArray();
        //                return Ok(File(
        //                    content,
        //                    System.Net.Mime.MediaTypeNames.Application.Octet,
        //                    String.Format("{0} - {1}.xlsx", sheetName, DateTime.Now.ToString("dd-MM-yyyy"))));
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //    }

        //    return Ok("Fail");
        //}
