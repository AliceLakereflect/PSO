using System.Collections.Generic;
using Xunit;

namespace PSO.Tests
{
    public class FileHandlerTest
    {
        IFileHandler fileHandler = new FileHandler();
        public FileHandlerTest()
        {
                
        }
        [Fact]
        public void OutputCsv()
        {
            var list = new List<AvgResult>();
            list.Add(new AvgResult());
            fileHandler.OutputCsv(list, "test");
        }
    }
}
