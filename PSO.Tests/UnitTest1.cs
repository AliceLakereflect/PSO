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
        [Fact]
        public void Readtxt()
        {
            var expectedResult = new List<int> { 56, 56823 , 5682377 };
            
            var result = fileHandler.Readtxt("testTxt");
            Assert.Equal(expectedResult[0],result.Dequeue());
            Assert.Equal(expectedResult[1],result.Dequeue());
            Assert.Equal(expectedResult[2],result.Dequeue());
        }
    }
}
