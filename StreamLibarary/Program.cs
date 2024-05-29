using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamLibarary
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //CsvHelper.WriteLineCSV("dir1/data5.csv", new Student { Id = 5, Name = "William" });
            //CsvHelper.WriteLineCSV("dir1/data5.csv", new Student { Id = 6, Name = "Louis" });

            CsvHelper.WriteLineCSV("data5.csv", new RecordModel
            {
                DateTime = "20240130",
                Price = "100",
                Type = "食",
                Content = "便當",
                PayMethod = "",
                Property = "",
            });

            foreach (var data in CsvHelper.ReadCSV<RecordModel>("data5.csv"))
            {
                Console.WriteLine(data.DateTime);
                Console.WriteLine(data.Price);
                Console.WriteLine(data.Type);
                Console.WriteLine(data.Content);
                Console.WriteLine(data.PayMethod);
                Console.WriteLine(data.Property);
            }

            //foreach (var data in CsvHelper.ReadCSV<Student>("dir1/data5.csv"))
            //{
            //    Console.WriteLine(data.Id);
            //    Console.WriteLine(data.Name);
            //}

            Console.ReadKey();
        }
    }
}
