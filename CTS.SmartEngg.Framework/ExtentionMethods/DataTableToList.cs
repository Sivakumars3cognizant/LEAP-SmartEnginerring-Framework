// Copyright (c) Cognizant. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace CTS.SmartEngg.Framework
{
    public static class DataTableToList
    {
        public static List<T> ToDTList<T>(DataTable dt)
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
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (string.Compare(pro.Name, column.ColumnName, true) == 0 && dr[column.ColumnName] != DBNull.Value)
                    {
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
    }
}
