using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StreamLibarary
{
    internal class CsvWriter
    {
        static ConcurrentDictionary<string, int> headIndexMapping = new ConcurrentDictionary<string, int>();

        public static void WriteLineCSV<T>(string path, T data, bool append = true, bool createDir = false)
           where T : class, new()
        {
            WriteCSV(path, new List<T> { data }, append, createDir);
        }

        public static void WriteCSV<T>(string path, List<T> datas, bool append = true, bool createDir = false)
            where T : class, new()
        {
            try
            {
                if (!CsvFileManager.IsPathValid(path, createDir: createDir))
                    throw new Exception("Path invalid");

                // 檔案是空的或是需要複寫檔案 => 寫入header
                if (CsvFileManager.IsFileDoseNotExistOrEmpty(path) || !append)
                {
                    CsvHeaderManager.WriteHeader<T>(path);
                }

                // 決定寫入模式
                CsvMode csvMode = CsvModeSelector.ModeSelector<T>(path);
                if (csvMode == CsvMode.Header)
                    headIndexMapping = CsvHeaderManager.ReadHeader(path);

                // 寫入資料
                var writer = new StreamWriter(path, true);
                foreach (var data in datas)
                {
                    WriteLineCSVByData(writer, data, csvMode);
                }
                writer.Close();
            }
            finally
            {
            }
        }



        private static void WriteLineCSVByData<T>(StreamWriter writer, T data, CsvMode csvMode)
        {
            PropertyInfo[] properties = data.GetType().GetProperties();
            var dataStringArr = new string[properties.Length];

            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];
                string propertyName = property.Name;
                object value = property.GetValue(data);
                string propertyValue = (value == null) ? string.Empty : value.ToString();
                int index = 0;

                switch (csvMode)
                {
                    case CsvMode.Header:
                        index = headIndexMapping[propertyName];
                        break;
                    case CsvMode.IndexAttribute:
                        index = property.GetCustomAttribute<IndexAttribute>().Index;
                        break;
                    case CsvMode.ByOrder:
                        index = i;
                        break;
                }

                dataStringArr[index] = propertyValue;
            }

            writer.WriteLine(StrArrToLineCSV(dataStringArr));
        }

        private static string StrArrToLineCSV(string[] strArr)
        {
            string lineCSV = string.Empty;

            foreach (string str in strArr)
            {
                lineCSV += str;
                lineCSV += ",";
            }

            return lineCSV.Substring(0, lineCSV.Length - 1);
        }
    }
}
