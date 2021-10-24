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
    }

    public interface IFileHandler
    {
        void OutputCsv(List<AvgResult> list, string fileName);
        void OutputString(List<double> list, string fileName);
    }
}
