using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace PSO
{
    public class FileHandler : IFileHandler
    {
        public FileHandler()
        {
        }
        public void OutputCsv(List<AvgResult> list, string fileName)
        {
            var path = Path.Combine(Environment.CurrentDirectory, $"{fileName}.csv");
            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<AvgResult>();
                csv.NextRecord();
                foreach (var item in list)
                {
                    csv.WriteRecord(item);
                    csv.NextRecord();
                }
            }
        }
        public void OutputString(List<double> list, string fileName)
        {
            var path = Path.Combine(Environment.CurrentDirectory, $"{fileName}.csv");
            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                foreach (var item in list)
                {
                    csv.WriteRecord(item);
                    csv.NextRecord();
                }
            }
        }

        public Queue<int> Readtxt(string fileName)
        {
            var result = new Queue<int>();
            var path = Path.Combine(Environment.CurrentDirectory, $"{fileName}.txt");
            using (var sr = new StreamReader(path))
            {
                while (sr.Peek() >= 0)
                {
                    var random = sr.ReadLine();
                    result.Enqueue(Int32.Parse(random));
                }
            }
            return result;
        }
    }

    public interface IFileHandler
    {
        void OutputCsv(List<AvgResult> list, string fileName);
        void OutputString(List<double> list, string fileName);
        Queue<int> Readtxt(string fileName);
    }
}
