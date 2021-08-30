// Copyright (c) Cognizant. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using System.IO;
using System.Text;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using System.Text.RegularExpressions;
using System.Collections;

namespace CTS.SmartEngg.Framework
{
    public class DownloadExcel
    {
        /// <summary>
        /// This Method Used to Download excel from Dataset
        /// </summary>
        /// <param name="_DataSet">Dataset Values</param>
        /// <param name="FileName">FileName</param>
        /// <returns></returns>
        public virtual string ToExcelSheetByDataSet(DataSet _DataSet, string fileName)
        {            
            using (var workbook = SpreadsheetDocument.Create
               (fileName, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = workbook.AddWorkbookPart();
                workbook.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();
                workbook.WorkbookPart.Workbook.Sheets = new DocumentFormat.OpenXml.Spreadsheet.Sheets();

                uint sheetId = 1;

                foreach (DataTable table in _DataSet.Tables)
                {
                    var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                    var sheetData  = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
                    sheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);

                    DocumentFormat.OpenXml.Spreadsheet.Sheets sheets =
                        workbook.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>();
                    string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

                    if (sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Count() > 0)
                    {
                        sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>()
                            .Select(s => s.SheetId.Value).Max() + 1;
                    }

                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet()
                    { Id = relationshipId, SheetId = sheetId, Name = table.TableName };
                    sheets.Append(sheet);

                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                    List<String> columns = new List<string>();
                    foreach (DataColumn column in table.Columns)
                    {
                        columns.Add(column.ColumnName);

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(column.ColumnName);
                        headerRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(headerRow);

                    foreach (DataRow dsrow in table.Rows)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        foreach (String col in columns)
                        {
                            DocumentFormat.OpenXml.Spreadsheet.Cell cell =
                                new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(dsrow[col].ToString()); //
                            newRow.AppendChild(cell);
                        }

                        sheetData.AppendChild(newRow);
                    }
                   
                  
                   
                }

               

            }
         
                                 
            var fileInfo = new FileInfo(fileName);
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("Error Occured while exporting to Excel");
            }
            

            return fileName;

        }

        /// <summary>
        /// Generate DataVaidation(Dropdown) in Excel Sheet
        /// </summary>
        /// <param name="filename">Filename</param>
        /// <param name="dropdownlistdata">Dataset</param>
        /// <param name="causecodeRange">CauseCode Dropdown Rang</param>
        /// <param name="ApproveMute">DropDownvalue</param>
        /// <returns></returns>
        public string DownloadExcelDropdown(string filename,string sheetName, DataSet dropdownlistdata,string approvedRange,string approveMute)
        {
            string resultsApproveOrMure = string.Empty;
            string rangeApproveOrMure = approvedRange;
            List<string> resultsApproveOrMures = new List<string>();

            DataValidation dataValidationsApproveORMute = new DataValidation();

            DataValidations dataValidations = new DataValidations();

            resultsApproveOrMure = approveMute;

            using (SpreadsheetDocument myDoc = SpreadsheetDocument.Create(filename, SpreadsheetDocumentType.Workbook))
            {

                WorkbookPart workbookpart = myDoc.AddWorkbookPart();
                workbookpart.Workbook = new Workbook();
                WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                SheetData sheetData = new SheetData();
                worksheetPart.Worksheet = new Worksheet(sheetData);
                Sheets sheets = myDoc.WorkbookPart.Workbook.AppendChild(new Sheets());

               
                sheets.AppendChild(new Sheet()
                {
                    Id = myDoc.WorkbookPart.GetIdOfPart(myDoc.WorkbookPart.WorksheetParts.First()),
                    SheetId = 1,
                    Name = sheetName
                });

                dataValidationsApproveORMute = new DownloadExcel().
                        DropDownListValidation(dropdownlistdata.Tables[0], rangeApproveOrMure, resultsApproveOrMure);

                dataValidations.Append(dataValidationsApproveORMute);
                worksheetPart.Worksheet.AppendChild(dataValidations);

            }
            var fileInfo = new FileInfo(filename);

            var filepath = fileInfo.FullName;

            if (!File.Exists(filename))
            {

            }
            return filepath;
        }
        public virtual void ExportDatatableToExcel(DataTable dtColumnMapping, string filePath)
        {

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                DataTableToCSV(dtColumnMapping, filePath);            
           
        }

        public virtual void DataTableToCSV(DataTable dt, string filePath)
        {
            StringBuilder text = new StringBuilder();
            
                IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().
                                                  Select(column => column.ColumnName);
                text.AppendLine(string.Join(",", columnNames));

                if (!Directory.Exists(filePath.Replace(Path.GetFileName(filePath), string.Empty)))
                {
                    Directory.CreateDirectory(filePath.Replace(Path.GetFileName(filePath), string.Empty));
                }
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {

                        IEnumerable<string> fields1 = row.ItemArray.Select(field1 => string.Concat("\"",
                        field1.ToString().Replace("\"", "\"\""), "\""));
                        text.AppendLine(string.Join(",", fields1));

                    }
                }
                File.WriteAllText(filePath, text.ToString());                      
        }
        /// <summary>
        /// Add Datavalidation In DropDown List
        /// </summary>
        /// <param name="datatable">Data Table</param>
        /// <returns>Data Validation With Formula</returns>
        public DataValidation DropDownListValidation(DataTable datatable,string range, string result)
        {          

            DataValidation dataValidationscausecode = new DataValidation()
            {
                Type = DataValidationValues.List,
                AllowBlank = true,
                SequenceOfReferences = new ListValue<StringValue>() { InnerText = range },
                Formula1 = new Formula1("\"" + result + "\"")
            };

            return dataValidationscausecode;
        }
        /// <summary>
        /// Function to create a excelsheet by DataTable.For each DataTable it will create a Sheet
        /// </summary>
        /// <param name="table"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public string ToExcelSheetByDataTable(DataTable table, DataSet ds, string FileName, string sheetName,
            int rowstoskip = 0, bool appendColumns = true)
        {
            using (var workbook = SpreadsheetDocument.Open(FileName.Replace("..", ""), true))
            {
                WorkbookPart wbp = workbook.WorkbookPart;

                Sheet s = null;
                if (string.IsNullOrEmpty(sheetName))
                {
                    s = (Sheet)wbp.Workbook.Sheets.Elements<Sheet>().First();

                }
                else
                {

                    if (wbp.Workbook.Sheets.Elements<Sheet>().Count(nm => nm.Name == sheetName) == 0)
                    {
                        workbook.Close();
                        return "";
                    }
                    else
                    {
                        s = (Sheet)wbp.Workbook.Sheets.Elements<Sheet>().Where(nm => nm.Name == sheetName).First();
                    }
                }

                WorksheetPart wsp = (WorksheetPart)workbook.WorkbookPart.GetPartById(s.Id.Value);
                SheetData sd = (SheetData)wsp.Worksheet.GetFirstChild<SheetData>();
                Worksheet ws = new Worksheet();
                workbook.Save();
                List<String> columns = new List<string>();

                DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                foreach (DataColumn column in table.Columns)
                {

                    columns.Add(column.ColumnName);

                    if (appendColumns)
                    {
                        Run run1 = new Run();
                        run1.Append(new Text(column.ColumnName));
                        RunProperties run1Properties = new RunProperties();
                        run1Properties.Append(new Bold());

                        run1Properties.Append(new Alignment()
                        {
                            Horizontal = HorizontalAlignmentValues.Center,
                            Vertical = VerticalAlignmentValues.Center
                        });
                        run1.RunProperties = run1Properties;


                        InlineString inlineString = new InlineString();
                        inlineString.Append(run1);


                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.InlineString;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(column.ColumnName);
                        cell.Append(inlineString);

                        headerRow.AppendChild(cell);
                    }

                }
                if (appendColumns)
                {
                    sd.AppendChild(headerRow);
                }
                foreach (DataRow dsrow in table.Rows)
                {


                    DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    foreach (String col in columns)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        string objDataType = dsrow[col].GetType().ToString();

                        if (objDataType.Contains(TypeCode.Int32.ToString()) || objDataType.Contains
                            (TypeCode.Int64.ToString())
                            || objDataType.Contains(TypeCode.Decimal.ToString()))
                        {
                            cell.CellValue = new CellValue(dsrow[col].ToString());
                            cell.DataType = new EnumValue<CellValues>(CellValues.Number);
                        }
                        else if (objDataType.Contains(TypeCode.DateTime.ToString()))
                        {

                            dsrow[col] = Convert.ToDateTime(dsrow[col]).ToShortDateString();
                            cell.CellValue = new CellValue(Convert.ToDateTime(dsrow[col]).ToShortDateString());
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);
                        }
                        else
                        {
                            if (Regex.IsMatch(dsrow[col].ToString(), @"^\d+$"))
                            {
                                cell.CellValue = new CellValue(dsrow[col].ToString());
                                cell.DataType = new EnumValue<CellValues>(CellValues.Number);
                            }
                            else
                            {
                                cell.CellValue = new CellValue(dsrow[col].ToString());
                                cell.DataType = new EnumValue<CellValues>(CellValues.String);
                            }
                        }
                        newRow.AppendChild(cell);
                    }

                    sd.AppendChild(newRow);
                }


                workbook.Save();


            }
            var fileInfo = new FileInfo(FileName);

            var filepath = fileInfo.FullName;
            if (!File.Exists(FileName.Replace("..", "")))
            {
                //throw new CustomException("Error Occured while exporting to Excel");
            }

            return FileName.Replace("..", "");
        }
    }
}
