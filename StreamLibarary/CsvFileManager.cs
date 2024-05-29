using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamLibarary
{
    internal static class CsvFileManager
    {
        static object readFileLock = new object();

        public static bool IsPathValid(string path, bool createDir = false, bool createNewFile = true)
        {
            string directory = Path.GetDirectoryName(path);
            string fileName = Path.GetFileName(path);

            if (fileName.Split('.')[1] != "csv")
            {
                Console.WriteLine("File should be .csv");
                return false;
            }

            if (directory != string.Empty)
            {
                if (!Directory.Exists(directory))
                {
                    if (!createDir) return false;

                    Directory.CreateDirectory(directory);
                }
            }

            if (!File.Exists(path))
            {
                if (!createNewFile) return false;

                FileStream fs = File.Create(path);
                fs.Close();
            }

            return true;
        }

        public static bool IsFileDoseNotExistOrEmpty(string fileName)
        {
            if (!File.Exists(fileName))
                return true;

            bool result;
            lock (readFileLock)
            {
                using (var reader = new StreamReader(fileName))
                {
                    result = (reader.ReadLine() == null);
                }
            }

            return result;
        }
    }
}
