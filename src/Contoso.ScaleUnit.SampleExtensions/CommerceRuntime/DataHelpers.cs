namespace ITS
{
    namespace CommerceRuntime
    {
		using System;
		using System.Collections.Generic;
		using System.ComponentModel;
        using System.Diagnostics;
        using System.IO;
		using System.Linq;
		using System.Reflection;
        using System.Runtime.InteropServices.ComTypes;
        using System.Xml.Linq;
        using System.Xml.Serialization;
		using Microsoft.Dynamics.Commerce.Runtime.Data.Types;
        using OfficeOpenXml;
        using OfficeOpenXml.Style;

        public static class DataHelpers
        {
            public static List<T> ConvertDataTable<T>(DataTable dt)
            {
                List<T> data = new List<T>();
                foreach (DataRow row in dt.Rows)
                {
                    T item = GetItem<T>(row);
                    data.Add(item);
                }
                return data;
            }

            private static T GetItem<T>(DataRow dr)
            {
                Type temp = typeof(T);
                T obj = Activator.CreateInstance<T>();

                foreach (DataColumn column in dr.Table.Columns)
                {
					foreach (PropertyInfo pro in temp.GetRuntimeProperties())
					{
						if (pro.Name == column.ColumnName)
						{
							#region Added by Shiva depending on requirement
							if (pro.PropertyType == typeof(bool) && 
								dr.Table.Columns.SingleOrDefault(s => s.ColumnName == column.ColumnName).DataType == typeof(int))
							{
								pro.SetValue(obj, Convert.ToBoolean(dr[column.ColumnName]));
								continue;
							}
							#endregion

							pro.SetValue(obj, dr[column.ColumnName], null);
						}
						else
						{
                            continue; 
						}
                    }
                }
                return obj;
            }

			public static DataTable ToDataTable<T>(this IList<T> data)
			{
				PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
				DataTable table = new DataTable();
				for (int i = 0; i < props.Count; i++)
				{
					PropertyDescriptor prop = props[i];
					table.Columns.Add(prop.Name, prop.PropertyType);
				}
				object[] values = new object[props.Count];
				foreach (T item in data)
				{
					for (int i = 0; i < values.Length; i++)
					{
						values[i] = props[i].GetValue(item);
					}
					table.Rows.Add(values);
				}
				return table;
			}

			public static T DeserializeXML<T>(string input) where T : class
			{
				T result;
                XmlSerializer ser = new XmlSerializer(typeof(T));
                using (StringReader sr = new StringReader(input))
                {
					#pragma warning disable CA5369 // Use XmlReader for 'XmlSerializer.Deserialize()'
                    result = ser.Deserialize(sr) as T;
					#pragma warning restore CA5369 // Use XmlReader for 'XmlSerializer.Deserialize()'
                }
				return result;
            }

			public static List<T> DeserializeXML<T>(IEnumerable<XElement> input)
			{
				List<T> data = new List<T>();
				XmlSerializer ser = new XmlSerializer(typeof(T));
				foreach(XElement element in input)
				{
					T item = (T)ser.Deserialize(element.CreateReader());
					data.Add(item);
					//using (StringReader sr = new StringReader(element.CreateReader().ToString()))
					//{
					//	T item = (T)ser.Deserialize(element.CreateReader());
					//	data.Add(item);
					//}
				}
				return data;
			}

			public static string SerializeXML<T>(T ObjectToSerialize)
			{
				XmlSerializer xmlSerializer = new XmlSerializer(ObjectToSerialize.GetType());

				using (StringWriter textWriter = new StringWriter())
				{
					xmlSerializer.Serialize(textWriter, ObjectToSerialize);
					return textWriter.ToString();
				}
			}

        }

        #region Customised Excel Generation

        /// <summary>
        /// This class contains methods that can be used for generation of Excel and PDF at runtime.
        /// By SA on 1-11-2023
        /// </summary>
        /// <typeparam name="T"></typeparam>

        public static class DataHelpers<T> where T : class
        {
            public static string GenerateExcelFileAsBase64Str(List<T> data,string fileName,bool saveToDisk = false)
            {
                string base64ExcelStr = string.Empty;
                using (ExcelPackage excel = new ExcelPackage())
                {
                    ExcelWorkbook workbook = excel.Workbook;
                    string Sheet1 = "Sheet1";
                    ExcelWorksheet worksheet = workbook.Worksheets.Add(Sheet1);
                    AddDataToWorkSheet(worksheet, data);
                    string file = string.Format("{0}.xlsx", fileName);
                    if (saveToDisk)
                    {
                        if (File.Exists(file))
                        {
                            File.Delete(file);
                        }
                        bool isExcelOpened = false;
                        foreach (Process item in Process.GetProcessesByName("EXCEL"))
                        {
                            isExcelOpened = true;
                        }
                        if (!isExcelOpened)
                        {
                            excel.SaveAs(new FileInfo(file));
                        }
                        else
                        {
                            byte[] excelBytes = excel.GetAsByteArray();
                            File.WriteAllBytes(file, excelBytes);
                        }
                        using (FileStream reader = new FileStream(file, FileMode.Open))
                        {
                            byte[] buffer = new byte[reader.Length];
                            reader.Read(buffer, 0, (int)reader.Length);
                            base64ExcelStr = Convert.ToBase64String(buffer);
                        }
                    }
                    else
                    {
                        byte[] excelBytes = excel.GetAsByteArray();
                        base64ExcelStr = Convert.ToBase64String(excelBytes);
                    }
                }
                return base64ExcelStr;
            }
            public static void AddDataToWorkSheet(ExcelWorksheet worksheet, List<T> data)
            {
                if (data.Count > 0)
                {
                    Type type = typeof(T);
                    List<PropertyInfo> props = type.GetRuntimeProperties().ToList();
                    int rowLength = data.Count;
                    int colLength = props.Count;
                    worksheet.Row(1).Style.Font.Bold = true;
                    worksheet.Row(1).Style.Font.Size = 15;
                    worksheet.Row(1).Style.Font.Color.SetColor(System.Drawing.Color.White);
                    ExcelRange firstRow = worksheet.Cells[1, 1, 1, colLength];
                    firstRow.Merge = true;
                    firstRow.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    firstRow.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Black);
                    firstRow.Value = worksheet.Name.ToUpper();

                    for (int j = 0; j <= colLength - 1; j++)
                    {
                        ExcelRange tableHeader = worksheet.Cells[2, j + 1];
                        tableHeader.Style.Font.Bold = true;
                        tableHeader.Style.Font.Size = 13;
                        tableHeader.Value = props[j].Name.ToUpper();
                    }
                    for (int i = 0; i < rowLength; i++)
                    {
                        if (i % 2 != 0)
                        {
                            worksheet.Row(3 + i).Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Row(3 + i).Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#efefef"));
                        }
                        for (int j = 0; j < colLength; j++)
                        {
                            ExcelRange columnRange = worksheet.Cells[3 + i, j + 1];
                            object value = props[j].GetValue(data[i]);
                            if (value == null)
                            {
                                continue;
                            }
                            if (value.GetType() == typeof(decimal))
                            {
                                columnRange.Style.Numberformat.Format = "0.000";
                            }
                            columnRange.Style.Font.Size = 11;
                            columnRange.Value = value;
                        }
                    }
                    ExcelRange fullRange = worksheet.Cells[1, 1, rowLength + 2, colLength];
                    fullRange.AutoFitColumns();
                    fullRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    fullRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    //ExcelTable tab = worksheet.Tables.Add(fullRange, worksheet.Name + "Table");
                    //tab.TableStyle = TableStyles.Medium2;
                }
            }
        }
        #endregion
    }
}
