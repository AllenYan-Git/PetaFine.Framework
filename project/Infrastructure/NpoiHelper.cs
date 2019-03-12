using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Reflection;
using System.ComponentModel;
using System.Web.Mvc;
using NPOI.SS.Util;

namespace Infrastructure
{
    /// <summary>
    /// Npoi Excel操作
    /// </summary>
    public static class NpoiHelper
    {
        static Color _levelOneColor = Color.Green;
        static Color _levelTwoColor = Color.FromArgb(201, 217, 243);
        static Color _levelThreeColor = Color.FromArgb(231, 238, 248);
        static Color _levelFourColor = Color.FromArgb(232, 230, 231);
        static Color _levelFiveColor = Color.FromArgb(250, 252, 213);

        #region 读取Excel文件内容转换为DataSet
        /// <summary> 
        /// 读取Excel文件内容转换为DataSet,列名依次为 "c0"……c[columnlength-1] 
        /// </summary> 
        /// <param name="fileName">文件绝对路径</param> 
        /// <param name="startRow">数据开始行数(1为第一行)</param> 
        /// <param name="sheetName">导入模板的工作薄名称,用于验证是否和设定的模板一致</param> 
        /// <param name="columnDataType">每列的数据类型</param> 
        /// <returns></returns> 
        public static DataSet ReadExcel(string fileName, int startRow, string sheetName, params NpoiDataType[] columnDataType)
        {
            var ertime = 0;
            var intime = 0;
            var ds = new DataSet("ds");
            var dt = new DataTable("dt");
            var sb = new StringBuilder();
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                var workbook = WorkbookFactory.Create(stream);//使用接口，自动识别excel2003/2007格式 
                var sheet = workbook.GetSheetAt(0);//得到里面第一个sheet 
                if (!string.IsNullOrEmpty(sheetName) && sheet.SheetName != sheetName)
                {
                    throw new Exception("导入的数据模板格式不正确,请重新导入!");
                }
                int j;
                IRow row;
                #region ColumnDataType赋值
                if (columnDataType.Length <= 0)
                {
                    row = sheet.GetRow(startRow - 1);//得到第i行 
                    columnDataType = new NpoiDataType[row.LastCellNum];
                    for (var i = 0; i < row.LastCellNum; i++)
                    {
                        var hs = row.GetCell(i);
                        columnDataType[i] = GetCellDataType(hs);
                    }
                }
                #endregion
                for (j = 0; j < columnDataType.Length; j++)
                {
                    var tp = GetDataTableType(columnDataType[j]);
                    dt.Columns.Add("c" + j, tp);
                }
                for (var i = startRow - 1; i <= sheet.PhysicalNumberOfRows; i++)
                {

                    row = sheet.GetRow(i);//得到第i行 
                    try
                    {
                        var dr = dt.NewRow();
                        if (row != null)
                        {
                            for (j = 0; j < columnDataType.Length; j++)
                            {
                                dr["c" + j] = GetCellData(columnDataType[j], row, j);
                            }
                            dt.Rows.Add(dr);
                        }
                        intime++;
                    }
                    catch (Exception er)
                    {
                        ertime++;
                        sb.Append(string.Format("第{0}行出错：{1}\r\n", i + 1, er.Message));
                        continue;
                    }
                }
                ds.Tables.Add(dt);
            }
            if (ds.Tables[0].Rows.Count == 0 && sb.ToString() != "") throw new Exception(sb.ToString());
            return ds;
        }

        /// <summary> 
        /// 读取Excel文件内容转换为DataSet,列名依次为 "c0"……c[columnlength-1] 
        /// </summary> 
        /// <param name="fileName">文件绝对路径</param> 
        /// <param name="startRow">数据开始行数(1为第一行)</param> 
        /// <param name="columnDataType">每列的数据类型</param> 
        /// <returns></returns> 
        public static DataSet ReadExcel(string fileName, int startRow, params NpoiDataType[] columnDataType)
        {
            return ReadExcel(fileName, startRow, string.Empty, columnDataType);
        }

        /// <summary> 
        /// 读取Excel文件内容转换为DataSet
        /// </summary> 
        /// <param name="fileName">文件绝对路径</param> 
        /// <param name="startRow">数据开始行数(1为第一行)</param> 
        /// <param name="sheetName">导入模板的工作薄名称,用于验证是否和设定的模板一致</param> 
        /// <param name="isShowDisplayName">是否显示列名DisplayName</param>
        /// <param name="columnDataType">每列的数据类型</param> 
        /// <returns></returns> 
        public static DataSet ReadExcel<T>(string fileName, int startRow, string sheetName, bool isShowDisplayName = true, params NpoiDataType[] columnDataType)
        {
            var ertime = 0;
            var intime = 0;
            var ds = new DataSet("ds");
            var dt = new DataTable("dt");
            var sb = new StringBuilder();
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                var workbook = WorkbookFactory.Create(stream);//使用接口，自动识别excel2003/2007格式 
                var sheet = workbook.GetSheetAt(0);//得到里面第一个sheet 
                if (!string.IsNullOrEmpty(sheetName) && sheet.SheetName != sheetName)
                {
                    throw new Exception("导入的数据模板格式不正确,请重新导入!");
                }
                int j;
                IRow row;
                #region ColumnDataType赋值
                if (columnDataType.Length <= 0)
                {
                    row = sheet.GetRow(startRow - 1);//得到第i行 
                    columnDataType = new NpoiDataType[row.LastCellNum];
                    for (var i = 0; i < row.LastCellNum; i++)
                    {
                        var hs = row.GetCell(i);
                        columnDataType[i] = GetCellDataType(hs);
                    }
                }
                #endregion
                PropertyInfo[] arrColumn = typeof(T).GetProperties();
                string columnName = "";
                for (j = 0; j < columnDataType.Length; j++)
                {
                    var tp = GetDataTableType(columnDataType[j]);
                    if (isShowDisplayName)
                    {
                        Attribute attr = Attribute.GetCustomAttribute(arrColumn[j], typeof(DisplayNameAttribute));
                        DisplayNameAttribute attrDisplayName = attr as DisplayNameAttribute;
                        if (attr != null)
                        {
                            if (attrDisplayName != null && !String.IsNullOrWhiteSpace(attrDisplayName.DisplayName))
                                columnName = attrDisplayName.DisplayName;
                            else
                                columnName = arrColumn[j].Name;
                        }
                        else
                            columnName = arrColumn[j].Name;
                    }
                    else
                        columnName = arrColumn[j].Name;
                    dt.Columns.Add(columnName, tp);
                }
                for (var i = startRow - 1; i <= sheet.PhysicalNumberOfRows; i++)
                {
                    row = sheet.GetRow(i);//得到第i行 
                    //if (row == null) continue;
                    try
                    {
                        var dr = dt.NewRow();
                        if (row != null)
                        {
                            for (j = 0; j < columnDataType.Length; j++)
                            {
                                dr[j] = GetCellData(columnDataType[j], row, j) ?? DBNull.Value;
                            }
                        }
                        dt.Rows.Add(dr);
                        intime++;
                    }
                    catch (Exception er)
                    {
                        ertime++;
                        sb.Append(string.Format("第{0}行出错：{1}\r\n", i + 1, er.Message));
                        continue;
                    }
                }
                ds.Tables.Add(dt);
            }
            if (ds.Tables[0].Rows.Count == 0 && sb.ToString() != "") throw new Exception(sb.ToString());
            return ds;
        }
        #endregion

        #region 从DataSet导出到MemoryStream流2003
        /// <summary> 
        /// 从DataSet导出到MemoryStream流2003 
        /// </summary> 
        /// <param name="saveFileName">文件保存路径</param> 
        /// <param name="sheetName">Excel文件中的Sheet名称</param> 
        /// <param name="ds">存储数据的DataSet</param> 
        /// <param name="startRow">从哪一行开始写入，从0开始</param> 
        /// <param name="datatypes">DataSet中的各列对应的数据类型</param> 
        public static bool CreateExcel2003(string saveFileName, string sheetName, DataSet ds, int startRow, params NpoiDataType[] datatypes)
        {
            try
            {
                if (startRow < 0) startRow = 0;
                var wb = new HSSFWorkbook();
                wb = new HSSFWorkbook();
                //DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                //dsi.Company = "pkm";
                //SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                //si.Title =
                //si.Subject = "automatic genereted document";
                //si.Author = "pkm";
                //wb.DocumentSummaryInformation = dsi;
                //wb.SummaryInformation = si;
                ISheet sheet = wb.CreateSheet(sheetName);
                //sheet.SetColumnWidth(0, 50 * 256); 
                //sheet.SetColumnWidth(1, 100 * 256); 
                IRow row;
                ICell cell;
                DataRow dr;
                int j;
                int maxLength = 0;
                int curLength = 0;
                object columnValue;
                DataTable dt = ds.Tables[0];
                if (datatypes.Length < dt.Columns.Count)
                {
                    datatypes = new NpoiDataType[dt.Columns.Count];
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        string dtcolumntype = dt.Columns[i].DataType.Name.ToLower();
                        switch (dtcolumntype)
                        {
                            case "string": datatypes[i] = NpoiDataType.String;
                                break;
                            case "datetime": datatypes[i] = NpoiDataType.Datetime;
                                break;
                            case "boolean": datatypes[i] = NpoiDataType.Bool;
                                break;
                            case "double": datatypes[i] = NpoiDataType.Numeric;
                                break;
                            default: datatypes[i] = NpoiDataType.String;
                                break;
                        }
                    }
                }

                #region 创建表头
                row = sheet.CreateRow(0);//创建第i行 
                ICellStyle style1 = wb.CreateCellStyle();//样式 
                IFont font1 = wb.CreateFont();//字体 

                font1.Color = HSSFColor.WHITE.index;//字体颜色 
                font1.Boldweight = (short)FontBoldWeight.BOLD;//字体加粗样式 
                //style1.FillBackgroundColor = HSSFColor.WHITE.index;//GetXLColour(wb, LevelOneColor);// 设置图案色 
                style1.FillForegroundColor = HSSFColor.GREEN.index;//GetXLColour(wb, LevelOneColor);// 设置背景色 
                style1.FillPattern = FillPatternType.SOLID_FOREGROUND;
                style1.SetFont(font1);//样式里的字体设置具体的字体样式 
                style1.Alignment = HorizontalAlignment.CENTER;//文字水平对齐方式 
                style1.VerticalAlignment = VerticalAlignment.CENTER;//文字垂直对齐方式 
                row.HeightInPoints = 25;
                for (j = 0; j < dt.Columns.Count; j++)
                {
                    columnValue = dt.Columns[j].ColumnName;
                    curLength = Encoding.Default.GetByteCount(columnValue.ToString()) + 2;
                    maxLength = (maxLength < curLength ? curLength : maxLength);
                    int colounwidth = 256 * maxLength;
                    sheet.SetColumnWidth(j, colounwidth);
                    try
                    {
                        cell = row.CreateCell(j);//创建第0行的第j列 
                        cell.CellStyle = style1;//单元格式设置样式 

                        try
                        {
                            cell.SetCellType(CellType.STRING);
                            cell.SetCellValue(columnValue.ToString());
                        }
                        catch { }

                    }
                    catch
                    {
                        continue;
                    }
                }
                #endregion

                #region 创建每一行
                for (int i = startRow; i < ds.Tables[0].Rows.Count; i++)
                {
                    dr = ds.Tables[0].Rows[i];
                    row = sheet.CreateRow(i + 1);//创建第i行 
                    for (j = 0; j < dt.Columns.Count; j++)
                    {
                        columnValue = dr[j];
                        curLength = Encoding.Default.GetByteCount(columnValue.ToString()) + 5;
                        int maxColumnWidth = sheet.GetColumnWidth(j) / 256;
                        if (curLength > 64)
                        {
                            curLength = 64;
                        }
                        maxColumnWidth = (maxColumnWidth < curLength ? curLength : maxColumnWidth);
                        int colounwidth = 256 * maxColumnWidth;
                        sheet.SetColumnWidth(j, colounwidth);
                        try
                        {
                            cell = row.CreateCell(j);//创建第i行的第j列 
                            #region 插入第j列的数据
                            try
                            {
                                NpoiDataType dtype = datatypes[j];
                                switch (dtype)
                                {
                                    case NpoiDataType.String:
                                        {
                                            cell.SetCellType(CellType.STRING);
                                            cell.SetCellValue(columnValue.ToString());
                                        } break;
                                    case NpoiDataType.Datetime:
                                        {
                                            cell.SetCellType(CellType.STRING);
                                            cell.SetCellValue(columnValue.ToString());
                                        } break;
                                    case NpoiDataType.Numeric:
                                        {
                                            cell.SetCellType(CellType.NUMERIC);
                                            cell.SetCellValue(Convert.ToDouble(columnValue));
                                        } break;
                                    case NpoiDataType.Bool:
                                        {
                                            cell.SetCellType(CellType.BOOLEAN);
                                            cell.SetCellValue(Convert.ToBoolean(columnValue));
                                        } break;
                                    case NpoiDataType.Richtext:
                                        {
                                            cell.SetCellType(CellType.FORMULA);
                                            cell.SetCellValue(columnValue.ToString());
                                        } break;
                                }
                            }
                            catch
                            {
                                cell.SetCellType(CellType.STRING);
                                cell.SetCellValue(columnValue.ToString());
                            }
                            #endregion

                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
                #endregion

                //using (FileStream fs = new FileStream(saveFileName, FileMode.OpenOrCreate, FileAccess.Write))//生成文件在服务器上 
                //{
                //    wb.Write(fs);
                //    Console.WriteLine("文件保存成功！" + saveFileName);
                //}
                using (MemoryStream ms = new MemoryStream())
                {
                    //  ms.Position = 0;
                    wb.Write(ms);
                    ms.Flush();
                    // ms.Position = 0;

                    //   sheet.Dispose();
                    //workbook.Dispose();//一般只用写这一个就OK了，他会遍历并释放所有资源，但当前版本有问题所以只释放sheet

                    HttpContext curContext = HttpContext.Current;

                    // 设置编码和附件格式
                    curContext.Response.ContentType = "application/vnd.ms-excel";
                    // curContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    curContext.Response.ContentEncoding = Encoding.UTF8;
                    curContext.Response.Charset = "";
                    if (HttpContext.Current.Request.UserAgent.ToLower().Contains("firefox"))
                    {
                        curContext.Response.AppendHeader("Content-Disposition",
                       "attachment;filename=" + saveFileName);
                    }
                    else
                    {
                        curContext.Response.AppendHeader("Content-Disposition",
                        "attachment;filename=" + HttpUtility.UrlEncode(saveFileName, Encoding.UTF8));
                    }

                    curContext.Response.BinaryWrite(ms.GetBuffer());
                    curContext.Response.End();
                }

                return true;
            }
            catch 
            {
                Console.WriteLine("文件保存失败！" + saveFileName);
                return false;
            }

        }
        #endregion

        #region 从DataSet导出到MemoryStream流
        /// <summary> 
        /// 从List泛型导出到MemoryStream流
        /// 万少华
        /// 2015-1-23
        /// </summary> 
        /// <param name="list">List泛型数据集</param> 
        /// <param name="FileName">文件名</param> 
        /// <param name="sheetName">Excel文件中的Sheet名称</param> > 
        /// <param name="startRow">从哪一行开始写入，索引从0开始</param> 
        /// <param name="templateDic">模板路径</param> 
        /// <returns>StringBuilder</returns>
        public static ActionResult ExportExcel<T>(IList<T> list, string FileName, string sheetName, int startRow = 0, string templateDic = "", string yearMonthField = "WorkDate,BeginTeachDate,InSchoolDate")
        {
            try
            {
                if (startRow < 0) startRow = 0;
                IWorkbook workbook;
                //往模板中写入数据
                using (var stream = new FileStream(templateDic, FileMode.Open, FileAccess.Read))
                {
                    IRow row;   //行
                    ICell cell; //列
                    PropertyInfo[] arrColumn = typeof(T).GetProperties();
                    int columnCount = arrColumn.Length;
                    workbook = WorkbookFactory.Create(stream);//使用接口，自动识别excel2003/2007格式 

                    var sheet = workbook.GetSheetAt(0);//得到里面第一个sheet 

                    #region
                    /*
                    ICellStyle style1 = workbook.CreateCellStyle();//样式 
                    IFont font1 = workbook.CreateFont();//字体 

                    font1.Color = HSSFColor.WHITE.index;//字体颜色 
                    font1.Boldweight = (short)FontBoldWeight.BOLD;//字体加粗样式 
                    //style1.FillBackgroundColor = HSSFColor.WHITE.index;//GetXLColour(wb, LevelOneColor);// 设置图案色 
                    style1.FillForegroundColor = HSSFColor.GREEN.index;//GetXLColour(wb, LevelOneColor);// 设置背景色 
                    style1.FillPattern = FillPatternType.SOLID_FOREGROUND;
                    style1.SetFont(font1);//样式里的字体设置具体的字体样式 
                    style1.Alignment = HorizontalAlignment.CENTER;//文字水平对齐方式 
                    style1.VerticalAlignment = VerticalAlignment.CENTER;//文字垂直对齐方式 
                    row = sheet.CreateRow(0);
                    for(int i=0;i<columnCount;i++)
                    {
                        cell = row.CreateCell(i);//创建第0行的第j列 
                        cell.CellStyle = style1;
                    }
                     */
                    #endregion

                    object columnValue;
                    if (String.IsNullOrEmpty(sheetName) && sheet.SheetName != sheetName)
                    {
                        throw new Exception("导出的数据模板格式不正确,请联系管理员!");
                    }
                    //写入数据
                    for (int i = 0; i < list.Count; i++)
                    {
                        row = sheet.CreateRow(i + startRow);
                        for (int j = 0; j < columnCount; j++)
                        {
                            cell = row.CreateCell(j);
                            columnValue = arrColumn[j].GetValue(list[i], null) ?? "";
                            if (arrColumn[j].PropertyType.ToString().ToLower() == "system.datetime"
                                || arrColumn[j].PropertyType.ToString().ToLower() == "system.nullable`1[system.datetime]")
                            {
                                string strDateFormat = "";
                                if (yearMonthField.Contains(arrColumn[j].Name))
                                    strDateFormat = "yyyy-MM";
                                else
                                    strDateFormat = "yyyy-MM-dd";
                                if (columnValue.ToString() == "" || Convert.ToDateTime(columnValue.ToString()).ToString("yyyy-MM-dd") == "0001-01-01")
                                    cell.SetCellValue("");
                                else
                                    cell.SetCellValue(Convert.ToDateTime(columnValue.ToString()).ToString(strDateFormat));
                            }
                            else
                                cell.SetCellValue(columnValue.ToString());
                        }
                    }
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    workbook.Write(ms);
                    ms.Flush();

                    HttpContext curContext = HttpContext.Current;

                    // 设置编码和附件格式
                    curContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    curContext.Response.ContentEncoding = Encoding.UTF8;
                    curContext.Response.Charset = "UTF-8";
                    //获取浏览器类型
                    var browserType = curContext.Request.Browser.Type.ToLower();
                    //判断是否为火狐浏览器, 操作中发现火狐浏览器和IE浏览器在格式上有些不兼容
                    if (browserType.ToLower().IndexOf("firefox") > -1)
                    {
                        curContext.Response.AddHeader("Content-Disposition", "attachment;filename=\"" + FileName + ".xls\"");
                    }
                    else
                    {
                        curContext.Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(FileName, Encoding.UTF8) + ".xls");
                    }

                    curContext.Response.BinaryWrite(ms.GetBuffer());
                    curContext.Response.Flush();
                    curContext.Response.Close();
                    //curContext.Response.End();
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("文件导出失败！" + ex.ToString());
                return new EmptyResult();
            }
        }
        #endregion

        #region 从DataSet导出到MemoryStream流
        /// <summary> 
        /// 从List泛型导出到MemoryStream流
        /// 万少华
        /// 2015-1-4
        /// </summary> 
        /// <param name="list">List泛型数据集</param> 
        /// <param name="FileName">文件名</param> 
        /// <param name="sheetName">Excel文件中的Sheet名称</param> > 
        /// <param name="columnWidth">列宽</param> 
        /// <param name="isShowDisplayName">是否显示DisplayName</param> 
        /// <param name="startRow">从哪一行开始写入，从0开始</param> 
        /// <param name="isTime">是否保留时分秒</param>
        /// <param name="timeColumnName">保留时分秒的字段名称</param>
        /// <param name="yearMonthField">年月字段</param>
        /// <returns>StringBuilder</returns>
        public static ActionResult GetExcel<T>(IList<T> list, string FileName, string sheetName, int columnWidth = 20, bool isShowDisplayName = true, int startRow = 0, bool isTime = false, string timeColumnName = "", string yearMonthField = "WorkDate,BeginTeachDate,InSchoolDate")
        {
            try
            {
                if (startRow < 0) startRow = 0;
                var wb = new HSSFWorkbook();
                //wb = new HSSFWorkbook();

                ISheet sheet = wb.CreateSheet(sheetName);

                IRow row;
                ICell cell;

                int maxLength = columnWidth;
                int curLength = 0;
                object columnValue;

                #region 创建表头
                row = sheet.CreateRow(0);//创建第i行 
                ICellStyle style1 = wb.CreateCellStyle();//样式 
                IFont font1 = wb.CreateFont();//字体 

                font1.Color = HSSFColor.WHITE.index;//字体颜色 
                font1.Boldweight = (short)FontBoldWeight.BOLD;//字体加粗样式 
                //style1.FillBackgroundColor = HSSFColor.WHITE.index;//GetXLColour(wb, LevelOneColor);// 设置图案色 
                style1.FillForegroundColor = HSSFColor.GREEN.index;//GetXLColour(wb, LevelOneColor);// 设置背景色 
                style1.FillPattern = FillPatternType.SOLID_FOREGROUND;
                style1.SetFont(font1);//样式里的字体设置具体的字体样式 
                style1.Alignment = HorizontalAlignment.CENTER;//文字水平对齐方式 
                style1.VerticalAlignment = VerticalAlignment.CENTER;//文字垂直对齐方式 
                row.HeightInPoints = 25;
                PropertyInfo[] arrColumn = typeof(T).GetProperties();
                for (int i = 0; i < arrColumn.Length; i++)
                {
                    if (isShowDisplayName)
                    {
                        Attribute attr = Attribute.GetCustomAttribute(arrColumn[i], typeof(DisplayNameAttribute));
                        DisplayNameAttribute attrDisplayName = attr as DisplayNameAttribute;
                        if (attr != null)
                        {
                            if (attrDisplayName != null && !String.IsNullOrWhiteSpace(attrDisplayName.DisplayName))
                                columnValue = attrDisplayName.DisplayName;
                            else
                                columnValue = arrColumn[i].Name;
                        }
                        else
                            columnValue = arrColumn[i].Name;
                    }
                    else
                        columnValue = arrColumn[i].Name;
                    curLength = Encoding.Default.GetByteCount(columnValue.ToString());
                    //maxLength = (maxLength < curLength ? curLength : maxLength);
                    int colounwidth = 256 * maxLength;
                    sheet.SetColumnWidth(i, colounwidth);
                    try
                    {
                        cell = row.CreateCell(i);//创建第0行的第j列 
                        cell.CellStyle = style1;//单元格式设置样式 

                        try
                        {
                            //cell.SetCellType(CellType.STRING);
                            cell.SetCellValue(columnValue.ToString());
                        }
                        catch { }

                    }
                    catch
                    {
                        continue;
                    }
                }
                #endregion

                #region 创建每一行
                ICellStyle style2 = wb.CreateCellStyle();//样式 
                style2.WrapText = true;//自动换行
                style2.Alignment = HorizontalAlignment.CENTER;//文字水平对齐方式 
                style2.VerticalAlignment = VerticalAlignment.CENTER;//文字垂直对齐方式 
                for (int i = startRow; i < list.Count; i++)
                {
                    row = sheet.CreateRow(i + 1);//创建第i行 
                    for (int j = 0; j < arrColumn.Length; j++)
                    {
                        columnValue = arrColumn[j].GetValue(list[i], null) ?? "";
                        curLength = Encoding.Default.GetByteCount(columnValue.ToString());
                        //maxLength = (maxLength < curLength ? curLength : maxLength);
                        int colounwidth = 256 * maxLength;
                        sheet.SetColumnWidth(j, colounwidth);
                        try
                        {
                            cell = row.CreateCell(j);//创建第i行的第j列 
                            cell.CellStyle = style2;//单元格式设置样式 
                            #region 插入第j列的数据
                            try
                            {
                                string dataType = arrColumn[j].PropertyType.ToString().ToLower();
                                switch (dataType)
                                {
                                    case "system.string":
                                        {
                                            //cell.SetCellType(CellType.STRING);
                                            cell.SetCellValue(columnValue.ToString());
                                        } break;
                                    case "system.datetime":
                                    case "system.nullable`1[system.datetime]":
                                        {
                                            //cell.SetCellType(CellType.STRING);
                                            string strDateFormat = "";
                                            if (isTime)
                                            {
                                                if (arrColumn[j].Name == timeColumnName)
                                                    strDateFormat = "yyyy-MM-dd HH:mm:ss";
                                                else
                                                    strDateFormat = "yyyy-MM-dd";
                                            }
                                            else if (yearMonthField.Contains(arrColumn[j].Name))
                                                strDateFormat = "yyyy-MM";
                                            else
                                                strDateFormat = "yyyy-MM-dd";
                                            if (Convert.ToDateTime(columnValue.ToString()).ToString("yyyy-MM-dd") == "0001-01-01")
                                                cell.SetCellValue("");
                                            else
                                                cell.SetCellValue(Convert.ToDateTime(columnValue.ToString()).ToString(strDateFormat));
                                        } break;
                                    case "system.numeric":
                                    case "system.double":
                                    case "system.float":
                                        {
                                            //cell.SetCellType(CellType.NUMERIC);
                                            cell.SetCellValue(Convert.ToDouble(columnValue));
                                        } break;
                                    case "system.bool":
                                        {
                                            //cell.SetCellType(CellType.BOOLEAN);
                                            cell.SetCellValue(Convert.ToBoolean(columnValue));
                                        } break;
                                    case "system.formula":
                                        {
                                            //cell.SetCellType(CellType.FORMULA);
                                            cell.SetCellValue(columnValue.ToString());
                                        }
                                        break;
                                }
                            }
                            catch
                            {
                                //cell.SetCellType(CellType.STRING);
                                cell.SetCellValue(columnValue.ToString());
                            }
                            #endregion

                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
                #endregion

                using (MemoryStream ms = new MemoryStream())
                {
                    wb.Write(ms);
                    ms.Flush();

                    HttpContext curContext = HttpContext.Current;

                    // 设置编码和附件格式
                    curContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    curContext.Response.ContentEncoding = Encoding.UTF8;
                    curContext.Response.Charset = "UTF-8";
                    curContext.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(FileName, Encoding.UTF8) + ".xls");

                    curContext.Response.BinaryWrite(ms.GetBuffer());
                    curContext.Response.Flush();
                    curContext.Response.Close();
                    curContext.Response.End();
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("文件导出失败！" + ex.ToString());
                return new EmptyResult();
            }
        }
        #endregion

        #region 从DataSet导出到MemoryStream流2007
        /// <summary> 
        /// 从DataSet导出到MemoryStream流2007 
        /// </summary> 
        /// <param name="saveFileName">文件保存路径</param> 
        /// <param name="sheetName">Excel文件中的Sheet名称</param> 
        /// <param name="ds">存储数据的DataSet</param> 
        /// <param name="startRow">从哪一行开始写入，从0开始</param> 
        /// <param name="datatypes">DataSet中的各列对应的数据类型</param> 
        public static bool CreateExcel2007(string saveFileName, string sheetName, DataSet ds, int startRow, params NpoiDataType[] datatypes)
        {
            try
            {
                if (startRow < 0) startRow = 0;

                XSSFWorkbook wb = new XSSFWorkbook();
                ISheet sheet = wb.CreateSheet(sheetName);
                //sheet.SetColumnWidth(0, 50 * 256); 
                //sheet.SetColumnWidth(1, 100 * 256); 
                IRow row;
                ICell cell;
                DataRow dr;
                int j;
                int maxLength = 0;
                int curLength = 0;
                object columnValue;
                DataTable dt = ds.Tables[0];
                if (datatypes.Length < dt.Columns.Count)
                {
                    datatypes = new NpoiDataType[dt.Columns.Count];
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        string dtcolumntype = dt.Columns[i].DataType.Name.ToLower();
                        switch (dtcolumntype)
                        {
                            case "string": datatypes[i] = NpoiDataType.String;
                                break;
                            case "datetime": datatypes[i] = NpoiDataType.Datetime;
                                break;
                            case "boolean": datatypes[i] = NpoiDataType.Bool;
                                break;
                            case "double": datatypes[i] = NpoiDataType.Numeric;
                                break;
                            default: datatypes[i] = NpoiDataType.String;
                                break;
                        }
                    }
                }

                #region 创建表头
                row = sheet.CreateRow(0);//创建第i行 
                ICellStyle style1 = wb.CreateCellStyle();//样式 
                IFont font1 = wb.CreateFont();//字体 

                font1.Color = HSSFColor.WHITE.index;//字体颜色 
                font1.Boldweight = (short)FontBoldWeight.BOLD;//字体加粗样式 
                //style1.FillBackgroundColor = HSSFColor.WHITE.index;//GetXLColour(wb, LevelOneColor);// 设置图案色 
                style1.FillForegroundColor = HSSFColor.GREEN.index;//GetXLColour(wb, LevelOneColor);// 设置背景色 
                style1.FillPattern = FillPatternType.SOLID_FOREGROUND;
                style1.SetFont(font1);//样式里的字体设置具体的字体样式 
                style1.Alignment = HorizontalAlignment.CENTER;//文字水平对齐方式 
                style1.VerticalAlignment = VerticalAlignment.CENTER;//文字垂直对齐方式 
                row.HeightInPoints = 25;
                for (j = 0; j < dt.Columns.Count; j++)
                {
                    columnValue = dt.Columns[j].ColumnName;
                    curLength = Encoding.Default.GetByteCount(columnValue.ToString());
                    maxLength = 20;  //设置单元格宽度
                    // maxLength = (maxLength < curLength ? curLength : maxLength);
                    int colounwidth = 256 * maxLength;
                    sheet.SetColumnWidth(j, colounwidth);
                    try
                    {
                        cell = row.CreateCell(j);//创建第0行的第j列 
                        cell.CellStyle = style1;//单元格式设置样式 

                        try
                        {
                            //cell.SetCellType(CellType.STRING); 
                            cell.SetCellValue(columnValue.ToString());
                        }
                        catch { }

                    }
                    catch
                    {
                        continue;
                    }
                }
                #endregion

                #region 创建每一行
                ICellStyle style2 = wb.CreateCellStyle();//样式 
                style2.WrapText = true;//自动换行
                style2.Alignment = HorizontalAlignment.CENTER;//文字水平对齐方式 
                style2.VerticalAlignment = VerticalAlignment.CENTER;//文字垂直对齐方式 
                //style2.DataFormat = 49;// HSSFDataFormat.GetBuiltinFormat("0.00");
                for (int i = startRow; i < ds.Tables[0].Rows.Count; i++)
                {
                    dr = ds.Tables[0].Rows[i];
                    // sheet.SetRowBreak(i);
                    row = sheet.CreateRow(i + 1);//创建第i行 
                    for (j = 0; j < dt.Columns.Count; j++)
                    {
                        columnValue = dr[j];
                        curLength = Encoding.Default.GetByteCount(columnValue.ToString());
                        //  maxLength = (maxLength < curLength ? curLength : maxLength);  //设置表格默认宽度
                        int colounwidth = 256 * maxLength;
                        sheet.SetColumnWidth(j, colounwidth);
                        try
                        {
                            cell = row.CreateCell(j);//创建第i行的第j列 
                            // cell.CellStyle.WrapText = false;//设置自动换行
                            cell.CellStyle = style2;//单元格式设置样式 
                            #region 插入第j列的数据
                            try
                            {
                                NpoiDataType dtype = datatypes[j];
                                switch (dtype)
                                {
                                    case NpoiDataType.String:
                                        {
                                            //cell.SetCellType(CellType.STRING); 
                                            cell.SetCellValue(columnValue.ToString());
                                        } break;
                                    case NpoiDataType.Datetime:
                                        {
                                            // cell.SetCellType(CellType.STRING); 
                                            cell.SetCellValue(columnValue.ToString());
                                        } break;
                                    case NpoiDataType.Numeric:
                                        {
                                            //cell.SetCellType(CellType.NUMERIC); 
                                            cell.SetCellValue(Convert.ToDouble(columnValue));
                                        } break;
                                    case NpoiDataType.Bool:
                                        {
                                            //cell.SetCellType(CellType.BOOLEAN); 
                                            cell.SetCellValue(Convert.ToBoolean(columnValue));
                                        } break;
                                    case NpoiDataType.Richtext:
                                        {
                                            // cell.SetCellType(CellType.FORMULA); 
                                            cell.SetCellValue(columnValue.ToString());
                                        } break;
                                }
                            }
                            catch
                            {
                                //cell.SetCellType(HSSFCell.CELL_TYPE_STRING); 
                                cell.SetCellValue(columnValue.ToString());
                            }
                            #endregion

                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
                #endregion

                //foreach (int[] temp in Mergeds)
                //{
                //    sheet.AddMergedRegion(new CellRangeAddress(temp[0], temp[1], temp[2], temp[3]));


                //}
                //using (FileStream fs = new FileStream(saveFileName, FileMode.OpenOrCreate, FileAccess.Write))//生成文件在服务器上 
                //{
                //wb.Write(fs);
                //Console.WriteLine("文件保存成功！" + saveFileName);

                using (MemoryStream ms = new MemoryStream())
                {
                    //  ms.Position = 0;
                    wb.Write(ms);
                    ms.Flush();
                    // ms.Position = 0;

                    //   sheet.Dispose();
                    //workbook.Dispose();//一般只用写这一个就OK了，他会遍历并释放所有资源，但当前版本有问题所以只释放sheet

                    HttpContext curContext = HttpContext.Current;

                    // 设置编码和附件格式
                    // curContext.Response.ContentType = "application/vnd.ms-excel";
                    //curContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //curContext.Response.ContentEncoding = Encoding.UTF8;
                    //curContext.Response.Charset = "";
                    //curContext.Response.AppendHeader("Content-Disposition",
                    //    "attachment;filename=" + HttpUtility.UrlEncode(saveFileName, Encoding.UTF8));

                    //curContext.Response.BinaryWrite(ms.GetBuffer());
                    //curContext.Response.End();

                    //curContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //curContext.Response.ContentEncoding = Encoding.UTF8;
                    //curContext.Response.AddHeader("Content-Disposition", string.Format("attachment;filename=" + HttpUtility.UrlEncode(saveFileName, Encoding.UTF8) + ".xlsx"));
                    //curContext.Response.AddHeader("Content-Length", ms.ToArray().Length.ToString());
                    //curContext.Response.BinaryWrite(ms.ToArray());

                    Encoding encoding;
                    string outputFileName = null;
                    string browser = HttpContext.Current.Request.UserAgent.ToUpper();
                    if (browser.Contains("MS") == true && browser.Contains("IE") == true)
                    {
                        outputFileName = HttpUtility.UrlEncode(saveFileName);
                        encoding = System.Text.Encoding.Default;
                    }
                    else if (browser.Contains("FIREFOX") == true)
                    {
                        outputFileName = saveFileName;
                        encoding = System.Text.Encoding.GetEncoding("GB2312");
                    }
                    else
                    {
                        outputFileName = HttpUtility.UrlEncode(saveFileName);
                        encoding = System.Text.Encoding.Default;
                    }
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.ContentType = "application/ms-excel";
                    HttpContext.Current.Response.Buffer = true;
                    HttpContext.Current.Response.ContentEncoding = encoding;
                    HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xlsx", string.IsNullOrEmpty(outputFileName) ? DateTime.Now.ToString("yyyyMMddHHmmssfff") : outputFileName));
                    HttpContext.Current.Response.BinaryWrite(ms.ToArray());
                    curContext.Response.Flush();
                    HttpContext.Current.Response.End();
                    ms.Close();
                    ms.Dispose();
                }

                // }
                return true;
            }
            catch 
            {
                Console.WriteLine("文件保存失败！" + saveFileName);
                return false;
            }

        }
        /// <summary> 
        /// 从DataSet导出到MemoryStream流2007 
        /// </summary> 
        /// <param name="saveFileName">文件保存路径</param> 
        /// <param name="sheetName">Excel文件中的Sheet名称</param> 
        /// <param name="ds">存储数据的DataSet</param> 
        /// <param name="startRow">从哪一行开始写入，从0开始</param> 
        /// <param name="datatypes">DataSet中的各列对应的数据类型</param> 
        public static bool CreateExcel2007Mergeds(string saveFileName, string sheetName, DataSet ds, int startRow, int[,] Mergeds, params NpoiDataType[] datatypes)
        {
            try
            {
                if (startRow < 0) startRow = 0;

                XSSFWorkbook wb = new XSSFWorkbook();
                ISheet sheet = wb.CreateSheet(sheetName);
                //sheet.SetColumnWidth(0, 50 * 256); 
                //sheet.SetColumnWidth(1, 100 * 256); 
                IRow row;
                ICell cell;
                DataRow dr;
                int j;
                int maxLength = 0;
                int curLength = 0;
                object columnValue;
                DataTable dt = ds.Tables[0];
                if (datatypes.Length < dt.Columns.Count)
                {
                    datatypes = new NpoiDataType[dt.Columns.Count];
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        string dtcolumntype = dt.Columns[i].DataType.Name.ToLower();
                        switch (dtcolumntype)
                        {
                            case "string": datatypes[i] = NpoiDataType.String;
                                break;
                            case "datetime": datatypes[i] = NpoiDataType.Datetime;
                                break;
                            case "boolean": datatypes[i] = NpoiDataType.Bool;
                                break;
                            case "double": datatypes[i] = NpoiDataType.Numeric;
                                break;
                            default: datatypes[i] = NpoiDataType.String;
                                break;
                        }
                    }
                }

                #region 创建表头
                row = sheet.CreateRow(0);//创建第i行 
                ICellStyle style1 = wb.CreateCellStyle();//样式 
                IFont font1 = wb.CreateFont();//字体 

                font1.Color = HSSFColor.WHITE.index;//字体颜色 
                font1.Boldweight = (short)FontBoldWeight.BOLD;//字体加粗样式 
                //style1.FillBackgroundColor = HSSFColor.WHITE.index;//GetXLColour(wb, LevelOneColor);// 设置图案色 
                style1.FillForegroundColor = HSSFColor.GREEN.index;//GetXLColour(wb, LevelOneColor);// 设置背景色 
                style1.FillPattern = FillPatternType.SOLID_FOREGROUND;
                style1.SetFont(font1);//样式里的字体设置具体的字体样式 
                style1.Alignment = HorizontalAlignment.CENTER;//文字水平对齐方式 
                style1.VerticalAlignment = VerticalAlignment.CENTER;//文字垂直对齐方式 
                row.HeightInPoints = 25;
                for (j = 0; j < dt.Columns.Count; j++)
                {
                    columnValue = dt.Columns[j].ColumnName;
                    curLength = Encoding.Default.GetByteCount(columnValue.ToString());
                    maxLength = 20;  //设置单元格宽度
                    // maxLength = (maxLength < curLength ? curLength : maxLength);
                    int colounwidth = 256 * maxLength;
                    sheet.SetColumnWidth(j, colounwidth);
                    try
                    {
                        cell = row.CreateCell(j);//创建第0行的第j列 
                        cell.CellStyle = style1;//单元格式设置样式 

                        try
                        {
                            //cell.SetCellType(CellType.STRING); 
                            cell.SetCellValue(columnValue.ToString());
                        }
                        catch { }

                    }
                    catch
                    {
                        continue;
                    }
                }
                #endregion

                #region 创建每一行
                ICellStyle style2 = wb.CreateCellStyle();//样式 
                style2.WrapText = true;//自动换行
                style2.Alignment = HorizontalAlignment.CENTER;//文字水平对齐方式 
                style2.VerticalAlignment = VerticalAlignment.CENTER;//文字垂直对齐方式 
                //style2.DataFormat = 49;// HSSFDataFormat.GetBuiltinFormat("0.00");
                for (int i = startRow; i < ds.Tables[0].Rows.Count; i++)
                {
                    dr = ds.Tables[0].Rows[i];
                    // sheet.SetRowBreak(i);
                    row = sheet.CreateRow(i + 1);//创建第i行 
                    for (j = 0; j < dt.Columns.Count; j++)
                    {
                        columnValue = dr[j];
                        curLength = Encoding.Default.GetByteCount(columnValue.ToString());
                        //  maxLength = (maxLength < curLength ? curLength : maxLength);  //设置表格默认宽度
                        int colounwidth = 256 * maxLength;
                        sheet.SetColumnWidth(j, colounwidth);
                        try
                        {
                            cell = row.CreateCell(j);//创建第i行的第j列 
                            // cell.CellStyle.WrapText = false;//设置自动换行
                            cell.CellStyle = style2;//单元格式设置样式 
                            #region 插入第j列的数据
                            try
                            {
                                NpoiDataType dtype = datatypes[j];
                                switch (dtype)
                                {
                                    case NpoiDataType.String:
                                        {
                                            //cell.SetCellType(CellType.STRING); 
                                            cell.SetCellValue(columnValue.ToString());
                                        } break;
                                    case NpoiDataType.Datetime:
                                        {
                                            // cell.SetCellType(CellType.STRING); 
                                            cell.SetCellValue(columnValue.ToString());
                                        } break;
                                    case NpoiDataType.Numeric:
                                        {
                                            //cell.SetCellType(CellType.NUMERIC); 
                                            cell.SetCellValue(Convert.ToDouble(columnValue));
                                        } break;
                                    case NpoiDataType.Bool:
                                        {
                                            //cell.SetCellType(CellType.BOOLEAN); 
                                            cell.SetCellValue(Convert.ToBoolean(columnValue));
                                        } break;
                                    case NpoiDataType.Richtext:
                                        {
                                            // cell.SetCellType(CellType.FORMULA); 
                                            cell.SetCellValue(columnValue.ToString());
                                        } break;
                                }
                            }
                            catch
                            {
                                //cell.SetCellType(HSSFCell.CELL_TYPE_STRING); 
                                cell.SetCellValue(columnValue.ToString());
                            }
                            #endregion

                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
                #endregion


                for (int i = 0; i < Mergeds.GetLength(0); i++)
                {
                    sheet.AddMergedRegion(new CellRangeAddress(Mergeds[i, 0], Mergeds[i, 1], Mergeds[i, 2], Mergeds[i, 3]));

      
                }
                //foreach (int[] temp in Mergeds)
                //{
                //    sheet.AddMergedRegion(new CellRangeAddress(temp[0], temp[1], temp[2], temp[3]));

                 
                //}
                //using (FileStream fs = new FileStream(saveFileName, FileMode.OpenOrCreate, FileAccess.Write))//生成文件在服务器上 
                //{
                //wb.Write(fs);
                //Console.WriteLine("文件保存成功！" + saveFileName);

                using (MemoryStream ms = new MemoryStream())
                {
                    //  ms.Position = 0;
                    wb.Write(ms);
                    ms.Flush();
                    // ms.Position = 0;

                    //   sheet.Dispose();
                    //workbook.Dispose();//一般只用写这一个就OK了，他会遍历并释放所有资源，但当前版本有问题所以只释放sheet

                    HttpContext curContext = HttpContext.Current;

                    // 设置编码和附件格式
                    // curContext.Response.ContentType = "application/vnd.ms-excel";
                    //curContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //curContext.Response.ContentEncoding = Encoding.UTF8;
                    //curContext.Response.Charset = "";
                    //curContext.Response.AppendHeader("Content-Disposition",
                    //    "attachment;filename=" + HttpUtility.UrlEncode(saveFileName, Encoding.UTF8));

                    //curContext.Response.BinaryWrite(ms.GetBuffer());
                    //curContext.Response.End();

                    //curContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //curContext.Response.ContentEncoding = Encoding.UTF8;
                    //curContext.Response.AddHeader("Content-Disposition", string.Format("attachment;filename=" + HttpUtility.UrlEncode(saveFileName, Encoding.UTF8) + ".xlsx"));
                    //curContext.Response.AddHeader("Content-Length", ms.ToArray().Length.ToString());
                    //curContext.Response.BinaryWrite(ms.ToArray());

                    Encoding encoding;
                    string outputFileName = null;
                    string browser = HttpContext.Current.Request.UserAgent.ToUpper();
                    if (browser.Contains("MS") == true && browser.Contains("IE") == true)
                    {
                        outputFileName = HttpUtility.UrlEncode(saveFileName);
                        
                       
                        encoding = System.Text.Encoding.Default;
                        outputFileName = outputFileName.Replace("+", "%20");  
                    }
                    else if (browser.Contains("FIREFOX") == true)
                    {
                        outputFileName = saveFileName;
                        encoding = System.Text.Encoding.GetEncoding("GB2312");
                       
                    }
                    else
                    {
                        outputFileName = HttpUtility.UrlEncode(saveFileName);
                        encoding = System.Text.Encoding.Default;
                        outputFileName = outputFileName.Replace("+", "%20");  
                    }
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.ContentType = "application/ms-excel";
                    HttpContext.Current.Response.Buffer = true;
                    HttpContext.Current.Response.ContentEncoding = encoding;
                    HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment; filename=\"{0}.xlsx\"", string.IsNullOrEmpty(outputFileName) ? DateTime.Now.ToString("yyyyMMddHHmmssfff") : outputFileName));
                  

                    HttpContext.Current.Response.BinaryWrite(ms.ToArray());
                    curContext.Response.Flush();
                    HttpContext.Current.Response.End();
                    ms.Close();
                    ms.Dispose();
                }

                // }
                return true;
            }
            catch 
            {
                Console.WriteLine("文件保存失败！" + saveFileName);
                return false;
            }

        }

        /// <summary> 
        /// 从DataSet导出到MemoryStream流2007,通过模板导出，导出格式可控
        /// </summary> 
        /// <param name="saveFileName">文件保存路径</param> 
        /// <param name="sheetName">Excel文件中的Sheet名称</param> 
        /// <param name="ds">存储数据的DataSet</param> 
        /// <param name="startRow">从哪一行开始写入，从0开始</param> 
        /// <param name="excelPath">excel模板路径</param> 
        public static bool CreateExcel2007ByTemplate(string saveFileName, string sheetName, DataSet ds, int startRow, string excelPath)
        {
            try
            {
                if (startRow < 0) startRow = 0;
                IWorkbook wb;
                //XSSFWorkbook wb = new XSSFWorkbook();
                //往模板中写入数据
                using (var stream = new FileStream(excelPath, FileMode.Open, FileAccess.Read))
                {
                    IRow row;
                    ICell cell;
                    wb = WorkbookFactory.Create(stream);

                    //ISheet sheet = wb.CreateSheet(sheetName);
                    var sheet = wb.GetSheetAt(0);//得到里面第一个sheet 
                    //sheet.SetColumnWidth(0, 50 * 256); 
                    //sheet.SetColumnWidth(1, 100 * 256); 
                    int j;
                    //int maxLength = 0;
                    //int curLength = 0;
                    string columnValue;
                    DataRow dr;
                    DataTable dt = ds.Tables[0];

                    #region 创建每一行
                    //ICellStyle style2 = wb.CreateCellStyle();//样式 
                    //style2.WrapText = true;//自动换行
                    //style2.Alignment = HorizontalAlignment.CENTER;//文字水平对齐方式 
                    //style2.VerticalAlignment = VerticalAlignment.CENTER;//文字垂直对齐方式 
                    for (int i = startRow; i < ds.Tables[0].Rows.Count; i++)
                    {
                        dr = ds.Tables[0].Rows[i];
                        // sheet.SetRowBreak(i);
                        row = sheet.CreateRow(i + startRow + 1);//创建第i行 
                        for (j = 0; j < dt.Columns.Count; j++)
                        {
                            columnValue = dr[j].ToString();
                            //curLength = Encoding.Default.GetByteCount(columnValue.ToString());
                            //  maxLength = (maxLength < curLength ? curLength : maxLength);  //设置表格默认宽度
                            //int colounwidth = 256 * maxLength;
                            //sheet.SetColumnWidth(j, colounwidth);
                            cell = row.CreateCell(j);//创建第i行的第j列 
                            cell.SetCellType(CellType.STRING);
                            cell.SetCellValue(columnValue.ToString());
                            //cell.SetCellValue(columnValue.ToString());
                        }
                    }

                    #endregion

                    //using (FileStream fs = new FileStream(saveFileName, FileMode.OpenOrCreate, FileAccess.Write))//生成文件在服务器上 
                    //{
                    //wb.Write(fs);
                    //Console.WriteLine("文件保存成功！" + saveFileName);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        //  ms.Position = 0;
                        wb.Write(ms);
                        ms.Flush();
                        // ms.Position = 0;

                        //   sheet.Dispose();
                        //workbook.Dispose();//一般只用写这一个就OK了，他会遍历并释放所有资源，但当前版本有问题所以只释放sheet

                        HttpContext curContext = HttpContext.Current;

                        curContext.Response.AddHeader("Content-Disposition", string.Format("attachment;filename=" + HttpUtility.UrlEncode(saveFileName) + ".xls"));
                        curContext.Response.AddHeader("Content-Length", ms.ToArray().Length.ToString());
                        curContext.Response.BinaryWrite(ms.ToArray());

                        curContext.Response.Flush();

                        ms.Close();
                        ms.Dispose();
                    }

                    // }
                    return true;
                }
            }
            catch 
            {
                Console.WriteLine("文件保存失败！" + saveFileName);
                return false;
            }

        }
        #endregion


        #region （重写）从DataSet导出到MemoryStream流2007
        /// <summary> 
        /// 从DataSet导出到MemoryStream流2007 
        /// </summary> 
        /// <param name="saveFileName">文件保存路径</param> 
        /// <param name="sheetName">Excel文件中的Sheet名称</param> 
        /// <param name="ds">存储数据的DataSet</param> 
        /// <param name="startRow">从哪一行开始写入，从0开始</param> 
        /// <param name="datatypes">DataSet中的各列对应的数据类型</param> 
        public static bool ToCreateExcel2007(string saveFileName, string sheetName, DataSet ds, int startRow, params NpoiDataType[] datatypes)
        {
            try
            {
                if (startRow < 0) startRow = 0;

                XSSFWorkbook wb = new XSSFWorkbook();
                ISheet sheet = wb.CreateSheet(sheetName);
                //sheet.SetColumnWidth(0, 50 * 256); 
                //sheet.SetColumnWidth(1, 100 * 256); 
                IRow row;
                ICell cell;
                DataRow dr;
                int j;
                int maxLength = 0;
                int curLength = 0;
                object columnValue;
                DataTable dt = ds.Tables[0];
                if (datatypes.Length < dt.Columns.Count)
                {
                    datatypes = new NpoiDataType[dt.Columns.Count];
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        string dtcolumntype = dt.Columns[i].DataType.Name.ToLower();
                        switch (dtcolumntype)
                        {
                            case "string": datatypes[i] = NpoiDataType.String;
                                break;
                            case "datetime": datatypes[i] = NpoiDataType.Datetime;
                                break;
                            case "boolean": datatypes[i] = NpoiDataType.Bool;
                                break;
                            case "double": datatypes[i] = NpoiDataType.Numeric;
                                break;
                            default: datatypes[i] = NpoiDataType.String;
                                break;
                        }
                    }
                }

                #region 创建表头
                row = sheet.CreateRow(0);//创建第i行 
                ICellStyle style1 = wb.CreateCellStyle();//样式 
                IFont font1 = wb.CreateFont();//字体 

                font1.Color = HSSFColor.WHITE.index;//字体颜色 
                font1.Boldweight = (short)FontBoldWeight.BOLD;//字体加粗样式 
                //style1.FillBackgroundColor = HSSFColor.WHITE.index;//GetXLColour(wb, LevelOneColor);// 设置图案色 
                style1.FillForegroundColor = HSSFColor.GREEN.index;//GetXLColour(wb, LevelOneColor);// 设置背景色 
                style1.FillPattern = FillPatternType.SOLID_FOREGROUND;
                style1.SetFont(font1);//样式里的字体设置具体的字体样式 
                style1.Alignment = HorizontalAlignment.CENTER;//文字水平对齐方式 
                style1.VerticalAlignment = VerticalAlignment.CENTER;//文字垂直对齐方式 
                row.HeightInPoints = 25;
                for (j = 0; j < dt.Columns.Count; j++)
                {
                    columnValue = dt.Columns[j].ColumnName;
                    curLength = Encoding.Default.GetByteCount(columnValue.ToString());
                    maxLength = 20;  //设置单元格宽度
                    // maxLength = (maxLength < curLength ? curLength : maxLength);
                    int colounwidth = 256 * maxLength;
                    sheet.SetColumnWidth(j, colounwidth);
                    try
                    {
                        cell = row.CreateCell(j);//创建第0行的第j列 
                        cell.CellStyle = style1;//单元格式设置样式 

                        try
                        {
                            //cell.SetCellType(CellType.STRING); 
                            cell.SetCellValue(columnValue.ToString());
                        }
                        catch { }

                    }
                    catch
                    {
                        continue;
                    }
                }
                #endregion

                #region 创建每一行
                ICellStyle style2 = wb.CreateCellStyle();//样式 
                style2.WrapText = true;//自动换行
                style2.Alignment = HorizontalAlignment.CENTER;//文字水平对齐方式 
                style2.VerticalAlignment = VerticalAlignment.CENTER;//文字垂直对齐方式 
                //设置边框格式
                style2.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
                style2.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
                style2.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
                style2.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
                for (int i = startRow; i < ds.Tables[0].Rows.Count; i++)
                {
                    dr = ds.Tables[0].Rows[i];
                    // sheet.SetRowBreak(i);
                    row = sheet.CreateRow(i + 1);//创建第i行 
                    for (j = 0; j < dt.Columns.Count; j++)
                    {
                        columnValue = dr[j];
                        curLength = Encoding.Default.GetByteCount(columnValue.ToString());
                        //  maxLength = (maxLength < curLength ? curLength : maxLength);  //设置表格默认宽度
                        int colounwidth = 256 * maxLength;
                        sheet.SetColumnWidth(j, colounwidth);
                        try
                        {
                            cell = row.CreateCell(j);//创建第i行的第j列 
                            // cell.CellStyle.WrapText = false;//设置自动换行
                            cell.CellStyle = style2;//单元格式设置样式 
                            #region 插入第j列的数据
                            try
                            {
                                NpoiDataType dtype = datatypes[j];
                                switch (dtype)
                                {
                                    case NpoiDataType.String:
                                        {
                                            //cell.SetCellType(CellType.STRING); 
                                            cell.SetCellValue(columnValue.ToString());
                                        } break;
                                    case NpoiDataType.Datetime:
                                        {
                                            // cell.SetCellType(CellType.STRING); 
                                            cell.SetCellValue(columnValue.ToString());
                                        } break;
                                    case NpoiDataType.Numeric:
                                        {
                                            //cell.SetCellType(CellType.NUMERIC); 
                                            cell.SetCellValue(Convert.ToDouble(columnValue));
                                        } break;
                                    case NpoiDataType.Bool:
                                        {
                                            //cell.SetCellType(CellType.BOOLEAN); 
                                            cell.SetCellValue(Convert.ToBoolean(columnValue));
                                        } break;
                                    case NpoiDataType.Richtext:
                                        {
                                            // cell.SetCellType(CellType.FORMULA); 
                                            cell.SetCellValue(columnValue.ToString());
                                        } break;
                                }
                            }
                            catch
                            {
                                //cell.SetCellType(HSSFCell.CELL_TYPE_STRING); 
                                cell.SetCellValue(columnValue.ToString());
                            }
                            #endregion

                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
                #endregion

                //using (FileStream fs = new FileStream(saveFileName, FileMode.OpenOrCreate, FileAccess.Write))//生成文件在服务器上 
                //{
                //wb.Write(fs);
                //Console.WriteLine("文件保存成功！" + saveFileName);

                using (MemoryStream ms = new MemoryStream())
                {
                    //  ms.Position = 0;
                    wb.Write(ms);
                    ms.Flush();
                    // ms.Position = 0;

                    //   sheet.Dispose();
                    //workbook.Dispose();//一般只用写这一个就OK了，他会遍历并释放所有资源，但当前版本有问题所以只释放sheet

                    HttpContext curContext = HttpContext.Current;

                    // 设置编码和附件格式
                    // curContext.Response.ContentType = "application/vnd.ms-excel";
                    //curContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //curContext.Response.ContentEncoding = Encoding.UTF8;
                    //curContext.Response.Charset = "";
                    //curContext.Response.AppendHeader("Content-Disposition",
                    //    "attachment;filename=" + HttpUtility.UrlEncode(saveFileName, Encoding.UTF8));

                    //curContext.Response.BinaryWrite(ms.GetBuffer());
                    //curContext.Response.End();
                    curContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    curContext.Response.ContentEncoding = Encoding.UTF8;
                    //如果是IE浏览器就转码
                    if (curContext.Request.UserAgent.ToLower().IndexOf("msie") > -1)
                    {
                        curContext.Response.AddHeader("Content-Disposition", string.Format("attachment;filename=" + HttpUtility.UrlEncode(saveFileName, Encoding.UTF8) + ".xlsx"));
                    }
                    else//否则就解码
                    {
                        curContext.Response.AddHeader("Content-Disposition", string.Format("attachment;filename=" + HttpUtility.UrlDecode(saveFileName, Encoding.UTF8) + ".xlsx"));
                    }
                    curContext.Response.AddHeader("Content-Length", ms.ToArray().Length.ToString());
                    curContext.Response.BinaryWrite(ms.ToArray());
                    curContext.Response.Flush();
                    ms.Close();
                    ms.Dispose();
                }

                // }
                return true;
            }
            catch 
            {
                Console.WriteLine("文件保存失败！" + saveFileName);
                return false;
            }

        }

        /// <summary> 
        /// 从DataSet导出到MemoryStream流2007,通过模板导出，导出格式可控
        /// </summary> 
        /// <param name="saveFileName">文件保存路径</param> 
        /// <param name="sheetName">Excel文件中的Sheet名称</param> 
        /// <param name="ds">存储数据的DataSet</param> 
        /// <param name="startRow">从哪一行开始写入，从0开始</param> 
        /// <param name="excelPath">excel模板路径</param> 
        public static bool ToCreateExcel2007ByTemplate(string saveFileName, string sheetName, DataSet ds, int startRow, string excelPath)
        {
            try
            {
                if (startRow < 0) startRow = 0;
                IWorkbook wb;
                //XSSFWorkbook wb = new XSSFWorkbook();
                //往模板中写入数据
                using (var stream = new FileStream(excelPath, FileMode.Open, FileAccess.Read))
                {
                    IRow row;
                    ICell cell;
                    wb = WorkbookFactory.Create(stream);

                    //ISheet sheet = wb.CreateSheet(sheetName);
                    var sheet = wb.GetSheetAt(0);//得到里面第一个sheet 
                    //sheet.SetColumnWidth(0, 50 * 256); 
                    //sheet.SetColumnWidth(1, 100 * 256); 
                    int j;
                    //int maxLength = 0;
                    //int curLength = 0;
                    string columnValue;
                    DataRow dr;
                    DataTable dt = ds.Tables[0];

                    #region 创建每一行
                    ICellStyle style2 = wb.CreateCellStyle();//样式 
                    style2.WrapText = true;//自动换行
                    style2.Alignment = HorizontalAlignment.CENTER;//文字水平对齐方式 
                    style2.VerticalAlignment = VerticalAlignment.CENTER;//文字垂直对齐方式 

                    //设置边框格式
                    style2.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
                    style2.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
                    style2.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
                    style2.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;

                    for (int i = startRow; i < ds.Tables[0].Rows.Count; i++)
                    {
                        dr = ds.Tables[0].Rows[i];
                        // sheet.SetRowBreak(i);
                        row = sheet.CreateRow(i + startRow + 1);//创建第i行 
                        for (j = 0; j < dt.Columns.Count; j++)
                        {
                            columnValue = dr[j].ToString();
                            //curLength = Encoding.Default.GetByteCount(columnValue.ToString());
                            //  maxLength = (maxLength < curLength ? curLength : maxLength);  //设置表格默认宽度
                            //int colounwidth = 256 * maxLength;
                            //sheet.SetColumnWidth(j, colounwidth);
                            cell = row.CreateCell(j);//创建第i行的第j列 
                            cell.SetCellType(CellType.STRING);
                            cell.SetCellValue(columnValue.ToString());
                            //cell.SetCellValue(columnValue.ToString());
                            cell.CellStyle = style2;
                        }
                    }

                    #endregion

                    //using (FileStream fs = new FileStream(saveFileName, FileMode.OpenOrCreate, FileAccess.Write))//生成文件在服务器上 
                    //{
                    //wb.Write(fs);
                    //Console.WriteLine("文件保存成功！" + saveFileName);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        //  ms.Position = 0;
                        wb.Write(ms);
                        ms.Flush();
                        // ms.Position = 0;

                        //   sheet.Dispose();
                        //workbook.Dispose();//一般只用写这一个就OK了，他会遍历并释放所有资源，但当前版本有问题所以只释放sheet

                        HttpContext curContext = HttpContext.Current;
                        curContext.Response.ContentType = "application/vnd.ms-excel";

                        curContext.Response.HeaderEncoding = Encoding.UTF8;
                        curContext.Response.ContentEncoding = Encoding.UTF8;
                        //如果是IE浏览器就转码
                        if (curContext.Request.UserAgent.ToLower().IndexOf("msie") > -1)
                        {
                            curContext.Response.AddHeader("Content-Disposition", string.Format("attachment;filename=" + HttpUtility.UrlEncode(saveFileName, Encoding.UTF8) + ".xls"));
                        }
                        else//否则就解码
                        {
                            curContext.Response.AddHeader("Content-Disposition", string.Format("attachment;filename=" + HttpUtility.UrlDecode(saveFileName, Encoding.UTF8) + ".xls"));
                        }
                        //curContext.Response.AddHeader("Content-Disposition", string.Format("attachment;filename=" + HttpUtility.UrlEncode(saveFileName) + ".xls"));
                        curContext.Response.AddHeader("Content-Length", ms.ToArray().Length.ToString());
                        curContext.Response.BinaryWrite(ms.ToArray());
                        curContext.Response.Flush();
                        ms.Close();
                        ms.Dispose();
                    }

                    // }
                    return true;
                }
            }
            catch 
            {
                Console.WriteLine("文件保存失败！" + saveFileName);
                return false;
            }

        }
        #endregion

        private static short GetXLColour(HSSFWorkbook workbook, Color systemColour)
        {
            short s = 0;
            HSSFPalette xlPalette = workbook.GetCustomPalette();
            HSSFColor xlColour = xlPalette.FindColor(systemColour.R, systemColour.G, systemColour.B);
            if (xlColour == null)
            {
                if (NPOI.HSSF.Record.PaletteRecord.STANDARD_PALETTE_SIZE < 255)
                {
                    if (NPOI.HSSF.Record.PaletteRecord.STANDARD_PALETTE_SIZE < 64)
                    {
                        xlColour = xlPalette.AddColor(systemColour.R, systemColour.G, systemColour.B);
                    }
                    else
                    {
                        xlColour = xlPalette.FindSimilarColor(systemColour.R, systemColour.G, systemColour.B);
                    }
                    s = xlColour.GetIndex();
                }
            }
            else
                s = xlColour.GetIndex();
            return s;
        }

        #region 读Excel-根据NpoiDataType创建的DataTable列的数据类型
        /// <summary> 
        /// 读Excel-根据NpoiDataType创建的DataTable列的数据类型 
        /// </summary> 
        /// <param name="datatype"></param> 
        /// <returns></returns> 
        private static Type GetDataTableType(NpoiDataType datatype)
        {
            Type tp = typeof(string);//Type.GetType("System.String") 
            switch (datatype)
            {
                case NpoiDataType.Bool:
                    tp = typeof(bool);
                    break;
                case NpoiDataType.Datetime:
                    tp = typeof(DateTime);
                    break;
                case NpoiDataType.Numeric:
                    tp = typeof(double);
                    break;
                case NpoiDataType.Error:
                    tp = typeof(string);
                    break;
                case NpoiDataType.Blank:
                    tp = typeof(string);
                    break;
            }
            return tp;
        }
        #endregion

        #region 读Excel-得到不同数据类型单元格的数据
        /// <summary> 
        /// 读Excel-得到不同数据类型单元格的数据 
        /// </summary> 
        /// <param name="datatype">数据类型</param> 
        /// <param name="row">数据中的一行</param> 
        /// <param name="column">哪列</param> 
        /// <returns></returns> 
        private static object GetCellData(NpoiDataType datatype, IRow row, int column)
        {
            if (row.GetCell(column) == null)
            {
                return null;
            }
            switch (datatype)
            {
                case NpoiDataType.String:
                    try
                    {
                        return row.GetCell(column).StringCellValue;
                    }
                    catch
                    {
                        try
                        {
                            return row.GetCell(column).StringCellValue;
                        }
                        catch
                        {
                            return row.GetCell(column).NumericCellValue;
                        }
                    }
                case NpoiDataType.DateTimeString:
                    try
                    {
                        var data = row.GetCell(column);
                        if (data.CellType == CellType.NUMERIC)
                        {
                            return data.DateCellValue;
                        }
                        else
                        {
                            var str = data.StringCellValue;
                            DateTime dt = DateTime.Now;
                            bool flag = DateTime.TryParse(str, out dt);
                            if (flag)
                                return dt;
                            else
                                return null;
                        }
                    }
                    catch
                    {
                        return null;
                    }
                case NpoiDataType.Bool:
                    try { return row.GetCell(column).BooleanCellValue; }
                    catch
                    {
                        return row.GetCell(column).StringCellValue;
                    }
                case NpoiDataType.Datetime:
                    try { return row.GetCell(column).DateCellValue; }
                    catch
                    {
                        return row.GetCell(column).StringCellValue;
                    }
                case NpoiDataType.Numeric:
                    try { return row.GetCell(column).NumericCellValue; }
                    catch
                    {
                        return row.GetCell(column).StringCellValue;
                    }
                case NpoiDataType.Richtext:
                    try { return row.GetCell(column).RichStringCellValue; }
                    catch
                    {
                        return row.GetCell(column).StringCellValue;
                    }
                case NpoiDataType.Error:
                    try { return row.GetCell(column).ErrorCellValue; }
                    catch
                    {
                        return row.GetCell(column).StringCellValue;
                    }
                case NpoiDataType.Blank:
                    try { return row.GetCell(column).StringCellValue; }
                    catch
                    {
                        return "";
                    }
                default: return "";
            }
        }
        #endregion

        #region 获取单元格数据类型
        /// <summary> 
        /// 获取单元格数据类型 
        /// </summary> 
        /// <param name="hs"></param> 
        /// <returns></returns> 
        private static NpoiDataType GetCellDataType(ICell hs)
        {
            NpoiDataType dtype;
            DateTime t1;
            string cellvalue = "";
            
            if (hs == null) return NpoiDataType.String;

            switch (hs.CellType)
            {
                case CellType.BLANK:
                    dtype = NpoiDataType.String;
                    cellvalue = hs.StringCellValue;
                    break;
                case CellType.BOOLEAN:
                    dtype = NpoiDataType.Bool;
                    break;
                case CellType.NUMERIC:
                    dtype = NpoiDataType.Numeric;
                    cellvalue = hs.NumericCellValue.ToString();
                    break;
                case CellType.STRING:
                    dtype = NpoiDataType.String;
                    cellvalue = hs.StringCellValue;
                    break;
                case CellType.ERROR:
                    dtype = NpoiDataType.Error;
                    break;
                case CellType.FORMULA:
                default:
                    dtype = NpoiDataType.Datetime;
                    break;
            }
            if (cellvalue != "" && DateTime.TryParse(cellvalue, out t1)) dtype = NpoiDataType.Datetime;
            return dtype;
        }
        #endregion

        #region 将数据写入到本地模板中 + RenderToTemplate
        /// <summary>
        /// 将数据写入到本地模板中
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="ds"></param>
        /// <param name="startRow"></param>
        /// <param name="excelPath"></param>
        /// <returns></returns>
        public static bool RenderToTemplate(string savePath, DataSet ds, int startRow, string excelPath)
        {
            try
            {
                if (startRow < 0) startRow = 0;
                IWorkbook wb;
                //往模板中写入数据
                using (var stream = new FileStream(excelPath, FileMode.Open, FileAccess.Read))
                {
                    IRow row;
                    ICell cell;
                    wb = WorkbookFactory.Create(stream);

                    var sheet = wb.GetSheetAt(0);//得到里面第一个sheet 
                    int j;
                    string columnValue;
                    DataRow dr;
                    DataTable dt = ds.Tables[0];

                    #region 创建每一行
                    for (int i = startRow; i < ds.Tables[0].Rows.Count; i++)
                    {
                        dr = ds.Tables[0].Rows[i];
                        row = sheet.CreateRow(i + startRow + 1);//创建第i行 
                        for (j = 0; j < dt.Columns.Count; j++)
                        {
                            columnValue = dr[j].ToString();
                            cell = row.CreateCell(j);//创建第i行的第j列 
                            cell.SetCellType(CellType.STRING);
                            cell.SetCellValue(columnValue.ToString());
                        }
                    }

                    #endregion

                    using (FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))//生成文件在服务器上 
                    {
                        wb.Write(fs);
                        //Console.WriteLine("文件保存成功！" + saveFileName);
                    }
                    return true;
                }
            }
            catch
            {
                //Console.WriteLine("文件保存失败！" + saveFileName);
                return false;
            }
        } 
        #endregion
    }

    #region 枚举(Excel单元格数据类型)
    /// <summary> 
    /// 枚举(Excel单元格数据类型) 
    /// </summary> 
    public enum NpoiDataType
    {
        /// <summary> 
        /// 字符串类型-值为1 
        /// </summary> 
        String,
        /// <summary> 
        /// 布尔类型-值为2 
        /// </summary> 
        Bool,
        /// <summary> 
        /// 时间类型-值为3 
        /// </summary> 
        Datetime,
        /// <summary> 
        /// 数字类型-值为4 
        /// </summary> 
        Numeric,
        /// <summary> 
        /// 复杂文本类型-值为5 
        /// </summary> 
        Richtext,
        /// <summary> 
        /// 空白 
        /// </summary> 
        Blank,
        /// <summary> 
        /// 错误 
        /// </summary> 
        Error,
        /// <summary>
        /// 可空的时间类型
        /// </summary>
        DateTimeString
    }
    #endregion

}