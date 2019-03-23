using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChuanYe.Utils
{
    public class ExcelHelper
    {
        public static DataTable ExcelToDataTable(string fileName, FileStream fs)
        {
            DataTable dt = new DataTable();
            IWorkbook wk = null;
            if (fileName.IndexOf(".xlsx") != -1)
            {
                //把xlsx文件中的数据写入wk中
                wk = new XSSFWorkbook(fs);
            }
            else
            {
                //把xls文件中的数据写入wk中
                wk = new HSSFWorkbook(fs);
            }
            fs.Close();
            ISheet sheet = wk.GetSheetAt(0);

            return dt;
        }

 

        public static DataTable UltraExcelToDataTable(string fileName, Stream fs)
        {
            DataTable dt = new DataTable();
            IWorkbook wk = null;
            if (fileName.IndexOf(".xlsx") != -1)
            {
                //把xlsx文件中的数据写入wk中
                wk = new XSSFWorkbook(fs);
            }
            else
            {
                //把xls文件中的数据写入wk中
                wk = new HSSFWorkbook(fs);
            }
            fs.Close();
            ISheet sheet = wk.GetSheetAt(0);
            dt = UltraExcelToDataTable(sheet);
            return dt;
        }

        public static DataTable UltraExcelToDataTable(ISheet sheet)
        {
            DataTable dt = new DataTable();
            string FLIGHT_Date = string.Empty;
            bool isCreated = false;
            for (int i = 0; i < sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                //判定是否有时间点,查询
                if (row != null && row.Cells.Count > 0)
                {
                    short fcNum = row.FirstCellNum;
                    if (fcNum > 0)
                    {
                        //抓取时间
                        var cell = row.GetCell(row.FirstCellNum);
                        if (cell != null)
                        {
                            string value = cell.StringCellValue;
                            //字符串是否有括号
                            if (value.IndexOf("(") > -1 && value.IndexOf(")") > -1)
                            {
                                value = value.TrimStart('(');
                                FLIGHT_Date = value.TrimEnd(')');
                            }
                        }
                    }
                    else//数据主体
                    {
                        if (!isCreated)
                        {
                            dt.Columns.Add("ID", typeof(string));
                            dt.Columns.Add("A_FLIGHT_NO", typeof(string));//进港航班
                            dt.Columns.Add("A_TASK_CODE", typeof(string));//任务代码
                            dt.Columns.Add("A_AIRCRAFT_TYPE_IATA", typeof(string));//机型
                            dt.Columns.Add("A_AC_REG_NO", typeof(string));//机号
                            dt.Columns.Add("A_ORIGIN_AIRPORT_IATA", typeof(string));//起飞代码
                            dt.Columns.Add("A_STA", typeof(string));//计划到港对应 到港预飞
                            dt.Columns.Add("A_ETA", typeof(string));//预计到港
                            dt.Columns.Add("A_DEST_AIRPORT_IATA", typeof(string));//本场


                            dt.Columns.Add("D_FLIGHT_NO", typeof(string));//离港航班
                            dt.Columns.Add("D_TASK_CODE", typeof(string));//任务代码
                            dt.Columns.Add("D_AIRCRAFT_TYPE_IATA", typeof(string));//机型
                            dt.Columns.Add("D_AC_REG_NO", typeof(string));//机号
                            dt.Columns.Add("D_DEST_AIRPORT_IATA", typeof(string));//落地代码
                            dt.Columns.Add("D_STD", typeof(string));//计划离港
                            dt.Columns.Add("D_ETD", typeof(string));//预计离港
                            dt.Columns.Add("OPERATION_DATE", typeof(string));//运营日
                            isCreated = !isCreated;
                        }
                        //判定是否是列头
                        int numFlag = 0;
                        var isNum = int.TryParse(row.GetCell(0).ToString(), out numFlag);
                        if (!isNum)
                        {
                            continue;
                        }
                        var lcNum = row.LastCellNum;
                        var dataRow = dt.NewRow();
                        int m = 0;
                        for (int j = 0; j < lcNum; j++)
                        {
                            if (row.GetCell(j) != null
                                     && !string.IsNullOrWhiteSpace(row.GetCell(j).ToString()))
                            {
                                row.GetCell(j).SetCellType(CellType.String);
                                dataRow[m] = row.GetCell(j).StringCellValue;
                                m++;
                            }
                        }
                        dataRow["OPERATION_DATE"] = FLIGHT_Date;
                        dt.Rows.Add(dataRow);
                    }
                }
            }
            //进行DataTable里的时间字段进行调整
            foreach (DataRow dr in dt.Rows)
            {
                string A_STA = dr["A_STA"].ToString();
                A_STA = FLIGHT_Date + " " + A_STA.Substring(0, 2) + ":" + A_STA.Substring(2, 2) + ":00";
                dr["A_STA"] = A_STA;

                string A_ETA = dr["A_ETA"].ToString();
                A_ETA = FLIGHT_Date + " " + A_ETA.Substring(0, 2) + ":" + A_ETA.Substring(2, 2) + ":00";
                dr["A_ETA"] = A_ETA;

                string D_STD = dr["D_STD"].ToString();
                D_STD = FLIGHT_Date + " " + D_STD.Substring(0, 2) + ":" + D_STD.Substring(2, 2) + ":00";
                dr["D_STD"] = D_STD;

                string D_ETD = dr["D_ETD"].ToString();
                D_ETD = FLIGHT_Date + " " + D_ETD.Substring(0, 2) + ":" + D_ETD.Substring(2, 2) + ":00";
                dr["D_ETD"] = D_ETD;
            }
            dt.AcceptChanges();
            return dt;
        }

        /// <summary>
        /// Datable导出成Excel
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="file">导出路径(包括文件名与扩展名)</param>
        public static void WanHuaTableToExcel(DataTable dt, string file,string title="",string footer="")
        {
            IWorkbook workbook;
            string fileExt = Path.GetExtension(file).ToLower();
            if (fileExt == ".xlsx") { workbook = new XSSFWorkbook(); } else if (fileExt == ".xls") { workbook = new HSSFWorkbook(); } else { workbook = null; }
            if (workbook == null) { return; }
            ISheet sheet = string.IsNullOrEmpty(dt.TableName) ? workbook.CreateSheet("Sheet1") : workbook.CreateSheet(dt.TableName);
            sheet.DefaultColumnWidth=18;
            //留空一行


            int allLine = 0;
            #region 标题
            //标题
            ICellStyle cellstyleTitle = workbook.CreateCellStyle();
            cellstyleTitle.VerticalAlignment = VerticalAlignment.Center;
            cellstyleTitle.Alignment = HorizontalAlignment.Center;
            IRow rowTitle = sheet.CreateRow(0);
            ICell cellTitle = rowTitle.CreateCell(0);
            cellTitle.SetCellValue(title);
            cellTitle.CellStyle = cellstyleTitle;
            sheet.AddMergedRegion(new CellRangeAddress(0, 1, 0, dt.Columns.Count!=0?dt.Columns.Count-1:1));
            allLine = allLine + 2;
            #endregion

            #region 表格第一行
            //表格第一行
            IRow row = sheet.CreateRow(allLine);
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                string cellText = dt.Columns[j].ColumnName; 
                row.CreateCell(j).SetCellValue(cellText);
            }
            allLine = allLine + 1;
            #endregion

            #region 表格内容

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IRow row1 = sheet.CreateRow(i + allLine);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ICell cell = row1.CreateCell(j);
                    cell.SetCellValue(dt.Rows[i][j].ToString());
                }
            }
            allLine = allLine + dt.Rows.Count;
            #endregion


            #region 表格结尾
            ICellStyle cellstyleFooter = workbook.CreateCellStyle();
            cellstyleFooter.VerticalAlignment = VerticalAlignment.Center;
            cellstyleFooter.Alignment = HorizontalAlignment.Right;
            IRow rowFooter = sheet.CreateRow(allLine);
            ICell cellFooter = rowFooter.CreateCell(0);
            cellFooter.SetCellValue(footer);
            cellFooter.CellStyle = cellstyleFooter;
            sheet.AddMergedRegion(new CellRangeAddress(allLine, allLine + 1, 0, dt.Columns.Count != 0 ? dt.Columns.Count - 1 : 1));
            #endregion


            //转为字节数组  
            MemoryStream stream = new MemoryStream();
            workbook.Write(stream);
            var buf = stream.ToArray();

            //保存为Excel文件  
            using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                fs.Write(buf, 0, buf.Length);
                fs.Flush();
            }
        }


        public static void DataTableExcel(DataTable dataTable, string fileName, string sheetName)
        {
            IWorkbook workBook = new XSSFWorkbook();
            ISheet sheet = workBook.CreateSheet(sheetName);
            IRow header = sheet.CreateRow(0);
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                ICell cell = header.CreateCell(i);
                cell.SetCellValue(dataTable.Columns[i].ColumnName);
            }
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                IRow row = sheet.CreateRow(i + 1);
            }
            //转为字节数组  
            MemoryStream stream = new MemoryStream();
            workBook.Write(stream);
            var buf = stream.ToArray();

            //保存为Excel文件  
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                fs.Write(buf, 0, buf.Length);
                fs.Flush();
                stream.Dispose();
            }
        }

        /// <summary>
        /// 获取单元格类型
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private static object GetValueType(ICell cell)
        {
            if (cell == null)
                return null;
            switch (cell.CellType)
            {
                case CellType.Blank: //BLANK:  
                    return null;
                case CellType.Boolean: //:  
                    return cell.BooleanCellValue;
                case CellType.Numeric: //NUMERIC:  
                    return cell.NumericCellValue;
                case CellType.String: //STRING:  
                    return cell.StringCellValue;
                case CellType.Error: //ERROR:  
                    return cell.ErrorCellValue;
                case CellType.Formula: //FORMULA:  
                default:
                    return "=" + cell.CellFormula;
            }
        }
    }
}
