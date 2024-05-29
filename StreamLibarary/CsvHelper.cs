using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Configuration;

namespace StreamLibarary
{
    public static class CsvHelper
    {
        public static List<T> ReadCSV<T>(string path)
            where T : class, new()
        {
            return CsvReader.ReadCSV<T>(path);
        }

        public static List<T> ReadCSV<T>(string path, int startLineNum, int lineCount)
            where T : class, new()
        {
            return CsvReader.ReadCSV<T>(path, startLineNum, lineCount);
        }


        public static void WriteLineCSV<T>(string path, T data, bool append = true, bool createDir = false)
            where T : class, new()
        {
            CsvWriter.WriteLineCSV(path, data, append, createDir);
        }

        public static void WriteCSV<T>(string path, List<T> datas, bool append = true, bool createDir = false)
            where T : class, new()
        {
            CsvWriter.WriteCSV(path, datas, append, createDir);
        }
    }
}
