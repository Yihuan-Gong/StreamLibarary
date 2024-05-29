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
    internal static class CsvHeaderManager
    {
        static ConcurrentDictionary<string, int> headIndexMapping = new ConcurrentDictionary<string, int>();
        static object readHeaderLock = new object();
        static object writeHeaderLock = new object();

        public static ConcurrentDictionary<string, int> ReadHeader(StreamReader reader)
        {
            headIndexMapping = new ConcurrentDictionary<string, int>();

            // 讀取檔案第一列
            reader.DiscardBufferedData();
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            string[] header = reader.ReadLine().Split(',');

            lock (readHeaderLock)
            {
                for (int i = 0; i < header.Length; i++)
                {
                    if (headIndexMapping.ContainsKey(header[i]))
                        continue;
                    headIndexMapping.TryAdd(header[i], i);
                }
            }

            return headIndexMapping;
        }

        public static ConcurrentDictionary<string, int> ReadHeader(string fileName)
        {
            var reader = new StreamReader(fileName);
            ConcurrentDictionary<string, int> result = ReadHeader(reader);
            reader.Close();
            return result;
        }


        public static bool HasHeader<T>(string fileName)
            where T : class, new()
        {
            var reader = new StreamReader(fileName);
            bool result = HasHeader<T>(reader);
            reader.Close();
            return result;
        }

        public static bool HasHeader<T>(StreamReader reader)
            where T : class, new()
        {
            var data = new T();

            // 讀取檔案第一列
            reader.DiscardBufferedData();
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            string firstRow = reader.ReadLine();

            if (firstRow == null)
                return false;

            HashSet<string> firstRowHash = firstRow.Split(',').ToHashSet();
            HashSet<string> propertiesNameHash = data.GetType().GetProperties().Select(x => x.Name).ToHashSet();

            bool result = propertiesNameHash.SetEquals(firstRowHash);
            return result;
        }

        public static void WriteHeader<T>(StreamWriter writer)
            where T : class, new()
        {
            T data = new T();
            PropertyInfo[] properties = data.GetType().GetProperties();
            string header = string.Empty;

            foreach (PropertyInfo property in properties)
            {
                header += property.Name;
                header += ",";
            }

            writer.WriteLine(header.TrimEnd(','));
        }

        public static void WriteHeader<T>(string fileName)
            where T : class, new()
        {
            lock (writeHeaderLock)
            {
                using (var writer = new StreamWriter(fileName, false))
                {
                    WriteHeader<T>(writer);
                    writer.Flush();
                }
            }
        }
    }
}
