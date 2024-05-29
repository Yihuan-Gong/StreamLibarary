using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StreamLibarary
{
    internal class CsvReader
    {
        static ConcurrentDictionary<string, int> headIndexMapping = new ConcurrentDictionary<string, int>();

        // ORM => Object Relation Mapping

        public static List<T> ReadCSV<T>(string path)
            where T : class, new()
        {
            if (!CsvFileManager.IsPathValid(path, false, false))
                return new List<T>();

            // 選擇讀取模式
            CsvMode mode = CsvModeSelector.ModeSelector<T>(path);
            if (mode == CsvMode.Header)
                headIndexMapping = CsvHeaderManager.ReadHeader(path);

            // 逐筆讀取資料
            var reader = new StreamReader(path);
            List<T> datas = new List<T>();
            if (mode == CsvMode.Header)
                reader.ReadLine(); // 如果是header mode的話，掠過header列(第一列)
            while (!reader.EndOfStream)
            {
                T data = ReadLineCSV<T>(reader, mode);
                datas.Add(data);
            }
            reader.Close();

            return datas;
        }

        public static List<T> ReadCSV<T>(string path, int startLineNum, int lineCount)
            where T : class, new()
        {
            if (!CsvFileManager.IsPathValid(path, false, false))
                return new List<T>();

            // 選擇讀取模式
            CsvMode mode = CsvModeSelector.ModeSelector<T>(path);
            if (mode == CsvMode.Header)
                headIndexMapping = CsvHeaderManager.ReadHeader(path);

            // 逐筆讀取資料
            var reader = new StreamReader(path);
            List<T> datas = new List<T>();
            if (mode == CsvMode.Header)
                reader.ReadLine(); // 如果是header mode的話，掠過header列(第一列)

            int i = 0;
            while (!reader.EndOfStream)
            {
                i++;
                if (i < startLineNum)
                    continue;
                if (i > startLineNum + lineCount)
                    break;

                T data = ReadLineCSV<T>(reader, mode);
                datas.Add(data);


            }
            reader.Close();

            return datas;
        }


        private static T ReadLineCSV<T>(StreamReader reader, CsvMode csvMode)
            where T : class, new()
        {
            T data = new T();
            string[] rawData = reader.ReadLine().Split(',');

            // 取得資料的所有欄位名稱(properties)
            PropertyInfo[] properties = data.GetType().GetProperties();

            // 逐"欄位"讀取資料，所有欄位讀取完成後，會產生一個T data
            for (int i = 0; i < properties.Length; i++)
            {
                int rawDataIndex = 0;
                PropertyInfo property = properties[i];

                // 取得該欄位儲存在csv內的index
                switch (csvMode)
                {
                    case CsvMode.Header:
                        rawDataIndex = headIndexMapping[property.Name];
                        break;
                    case CsvMode.IndexAttribute:
                        rawDataIndex = property.GetCustomAttribute<IndexAttribute>().Index;
                        break;
                    case CsvMode.ByOrder:
                        rawDataIndex = i;
                        break;
                }

                string value = rawData[rawDataIndex];
                object propertyValue = (value == null) ? null : Convert.ChangeType(value, property.PropertyType);
                property.SetValue(data, propertyValue);
            }

            return data;
        }
    }
}
